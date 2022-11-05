//-------------------------------------------------------------------------------------------------------
// <copyright file="DeviceConnection.cs" createdby="gblikas">
// 
// XR Remote
// Copyright(C) 2020  YOUAR, INC.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// https://www.gnu.org/licenses/agpl-3.0.html
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see
// <http://www.gnu.org/licenses/>.
//
// </copyright>
//-------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRRemote
{
    using UnityEngine.Events;
    using UnityEngine.Networking.PlayerConnection;

    public class DeviceConnection : MonoBehaviour, IConnection
    {

        public LogLevel logLevel = LogLevel.MINIMAL;

        public bool log
        {
            get
            {
                return logLevel == LogLevel.MINIMAL || logLevel == LogLevel.VERBOSE;
            }
        }

        string playerName = string.Empty;

        PlayerConnection playerConnection { get; set; }

        public string name { get { return playerConnection == null || string.IsNullOrEmpty(playerConnection.name) ? string.Empty : playerConnection.name; } }

        public ConnectionState connectionState { get; private set; }

        public bool connected
        {
            get
            {
                bool _connected = connectionState == ConnectionState.CONNECTED && playerConnection != null;
                if (!_connected)
                {
                    if (log)
                    {
                        Debug.Log($"NOT_CONNECTED");
                    }
                }
                return _connected;
            }
        }

        public Action<ConnectionState> onConnection;

        public Action<int> onDisconnection;

        private void Awake()
        {
            playerName = $"player_{SystemInfo.deviceUniqueIdentifier}_{DateTime.Now.ToString("hhmmss")}";
        }

        public bool Initialize()
        {
            try
            {
                connectionState = ConnectionState.DISCONNECTED;

                playerConnection = PlayerConnection.instance;
                playerConnection.name = playerName;

                playerConnection.RegisterConnection(OnConnection);
                playerConnection.RegisterDisconnection(OnDisconnection);

                onConnection += (status) => { if (log) Debug.Log(FormatConnectionMessage($"CONNECTION_STATUS value: {status}")); };
                onDisconnection += (id) => { if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: editor disconnected, playerId {id}")); };

                return true;

            }
            catch (Exception e)
            {
                if (log)
                    Debug.LogErrorFormat($"DeviceConnection {playerConnection.name}: failed {e.Message}");
                return false;
            }

        }

        public void RegisterMethod(Guid socketChannel, UnityAction<MessageEventArgs> callback)
        {
            if (!connected) throw new ArgumentException("NOT_CONNECTED reason: unknown");
            playerConnection.Register(socketChannel, callback);
            if (log) Debug.Log(FormatConnectionMessage($"registered to channel {socketChannel.ToString()}"));
        }

        public void OnConnection(int playerID)
        {
            if (log) Debug.Log(FormatConnectionMessage($"connection event: incoming id {playerID}"));
            connectionState = ConnectionState.CONNECTED;
            if (!connected)
            {
                if (log) Debug.LogError($"CONNECTION_EVENT value: failure to connect");
                return;
            }
            onConnection?.Invoke(connectionState);
        }

        public void OnDisconnection(int playerID)
        {
            if (!connected) return;
            //DisconnectAll();
            connectionState = ConnectionState.DISCONNECTED;
            onDisconnection?.Invoke(playerID);
        }

        public void DisconnectAll()
        {
            if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: starting disconnect"));
            if (!connected) return;
            if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: {playerConnection.name} left the lobby"));
#if UNITY_2017_1_OR_NEWER
            playerConnection?.DisconnectAll();
#endif
        }

        public bool Send(Guid socketChannel, object serializeableObject)
        {
            try
            {
                if (!connected) return false;
                byte[] data = serializeableObject.SerializeToByteArray();
                return Send(socketChannel, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"EXCEPTION reason: {e.Message}");
                return false;
            }
        }

        public bool Send(Guid socketChannel, byte[] data)
        {
            try
            {
                if (!connected) return false;
                if (log) Debug.Log(FormatConnectionMessage($"send event from {playerConnection.name} on channel {socketChannel}"));
                return playerConnection.TrySend(socketChannel, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"EXCEPTION reason: {e.Message}");
                return false;
            }
        }

        public void ToggleLogLevel(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        public void Disconnect()
        {
            DisconnectAll();
        }

        public string FormatConnectionMessage(string baseMessage)
        {
            if (!connected) return string.Empty;
            return $"DeviceConnection {playerConnection.name}: {baseMessage}";
        }
    }

}