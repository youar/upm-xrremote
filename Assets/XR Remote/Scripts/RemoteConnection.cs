//-------------------------------------------------------------------------------------------------------
// <copyright file="RemoteConnection.cs" createdby="gblikas">
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
using UnityEngine.Events;

namespace XRRemote
{

#if UNITY_EDITOR
    using UnityEditor.Networking.PlayerConnection;
    using UnityEngine.Networking.PlayerConnection;

    public class RemoteConnection : MonoBehaviour, IConnection
    {
        public LogLevel logLevel = LogLevel.MINIMAL;

        public bool log
        {
            get
            {
                return logLevel == LogLevel.MINIMAL || logLevel == LogLevel.VERBOSE;
            }
        }

        string editorName = string.Empty; 

        EditorConnection editorConnection { get; set; }

        public string name { get { return editorConnection == null || string.IsNullOrEmpty(editorConnection.name) ? string.Empty : editorConnection.name; } }

        public ConnectionState connectionState { get; private set; }

        public bool connected
        {
            get
            {
                bool _connected = connectionState == ConnectionState.CONNECTED && editorConnection != null;
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

        public Action onDisconnection; 

        private void Awake()
        {
            editorName = $"{SystemInfo.deviceUniqueIdentifier}_{DateTime.Now.ToString("hhmmss")}";
        }

        public bool Initialize()
        {
            try
            {
                connectionState = ConnectionState.DISCONNECTED;

                editorConnection = EditorConnection.instance;
                editorConnection.name = editorName;

                editorConnection.RegisterConnection(OnConnection);
                editorConnection.RegisterDisconnection(OnDisconnection);

                return true;

            }
            catch (Exception e)
            {
                if (log)
                    Debug.LogErrorFormat($"DeviceConnection {editorConnection.name}: failed {e.Message}");
                return false;
            }
        }

        public void RegisterMethod(Guid socketChannel, UnityAction<MessageEventArgs> callback)
        {
            if (!connected) throw new ArgumentException("NOT_CONNECTED reason: unknown");
            editorConnection.Register(socketChannel, callback);
            if (log) Debug.Log(FormatConnectionMessage($"registered to channel {socketChannel.ToString()}"));
        }

        public void OnConnection(int playerID)
        {
            if (log) Debug.Log(FormatConnectionMessage($"connection event: incoming id {playerID}"));
            connectionState = ConnectionState.CONNECTED;
            onConnection?.Invoke(connectionState);
        }

        public void OnDisconnection(int playerID)
        {
            if (!connected) return;
            //DisconnectAll();
            connectionState = ConnectionState.DISCONNECTED;
            onDisconnection?.Invoke(); 
        }

        public void Disconnect()
        {
            DisconnectAll();
        }

        public void DisconnectAll()
        {
            if (!connected) return;
            if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: {editorConnection.name} left"));
#if UNITY_2017_1_OR_NEWER
            editorConnection?.DisconnectAll();
#endif
        }

        public bool Send(Guid socketChannel, object serializeableObject)
        {
            if (!connected) return false;
            byte[] data = serializeableObject.SerializeToByteArray();
            return Send(socketChannel, data);
        }

        public bool Send(Guid socketChannel, byte[] data)
        {
            if (!connected) return false;
            if (log) Debug.Log(FormatConnectionMessage($"send event from {editorConnection.name} on channel {socketChannel}"));
            return editorConnection.TrySend(socketChannel, data);
        }

        public void ToggleLogLevel(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        public string FormatConnectionMessage(string baseMessage)
        {
            if (!connected) return string.Empty;
            return $"RemoteConnection {editorConnection.name}: {baseMessage}";
        }
    }
#endif

}
