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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

//using UnityEngine.Networking.PlayerConnection;

namespace XRRemote
{
    using UnityEngine.Events;
 //   using UnityEngine.Networking.PlayerConnection;

    public class DeviceConnection : MonoBehaviour, IConnection
    {
        private TcpListener tcpListener;
        private Thread tcpListenerThread;  	
        private TcpClient connectedTcpClient;
        
        public LogLevel logLevel = LogLevel.MINIMAL;

        public bool log
        {
            get
            {
                return logLevel == LogLevel.MINIMAL || logLevel == LogLevel.VERBOSE;
            }
        }

        string playerName = string.Empty;

    //    PlayerConnection playerConnection { get; set; }

    //    public string name { get { return playerConnection == null || string.IsNullOrEmpty(playerConnection.name) ? string.Empty : playerConnection.name; } }

        public ConnectionState connectionState { get; private set; }

        public bool connected
        {
            get
            {
                bool _connected = connectionState == ConnectionState.CONNECTED;
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

                /*
                tcpListenerThread = new Thread (new ThreadStart( ListenForIncommingRequests)); 		
                tcpListenerThread.IsBackground = true; 		
                tcpListenerThread.Start();
                */
                
                StartCoroutine(ListenForIncommingRequests());
                
                onConnection += (status) => { if (log) Debug.Log(FormatConnectionMessage($"CONNECTION_STATUS value: {status}")); };
                onDisconnection += (id) => { if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: editor disconnected, playerId {id}")); };

                return true;

            }
            catch (Exception e)
            {
                if (log)
                    Debug.LogException(e);
                return false;
            }
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
        //    if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: {playerConnection.name} left the lobby"));
#if UNITY_2017_1_OR_NEWER
        //    playerConnection?.DisconnectAll();
#endif
        }

        public bool Send(object serializeableObject)
        {
            try
            {
                if (!connected) return false;
                byte[] data = serializeableObject.SerializeToByteArray();
                return Send(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"EXCEPTION reason: {e.Message}");
                return false;
            }
        }
        
        public bool Send(byte[] data)
        {
            try
            {
                if (!connected) return false;

                NetworkStream stream = connectedTcpClient.GetStream();
                if (stream.CanWrite)
                {

                    stream.Write(data, 0, data.Length);
                    Debug.Log("Server sent his message - should be received by client");

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"EXCEPTION reason: {e.Message}");
            }
            return false;
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
            return $"DeviceConnection {(connectedTcpClient?.Client.RemoteEndPoint as IPEndPoint)?.Address}: {baseMessage}";
        }
        
        //TCP connections
        private IEnumerator ListenForIncommingRequests () {
            try
            {
                //	tcpListener = TcpListener.Create(8052); // new TcpListener(IPAddress.Parse(IP), 8052);//5555);//
		
                tcpListener = TcpListener.Create(8053);//new TcpListener(IPAddress.Parse("127.0.0.1"), 8053);
                if(!tcpListener.Server.IsBound)
                    tcpListener.Start();
			
                Debug.Log("Server is listening");
                Byte[] bytes = new Byte[1024];
                
                while (true)
                {
                    if (tcpListener.Pending())
                    {
                        using (connectedTcpClient = tcpListener.AcceptTcpClient())
                        {
                            using (NetworkStream stream = connectedTcpClient.GetStream())
                            {
                                int length;

                                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                                {
                                    var incommingData = new byte[length];
                                    Array.Copy(bytes, 0, incommingData, 0, length);

                                    string clientMessage = Encoding.ASCII.GetString(incommingData);
                                    Debug.Log("client message received as: " + clientMessage);
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.LogException(socketException);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                tcpListener.Stop();
            }

            yield return null;
        }

        private bool AcceptTcpClient()
        {
            return tcpListener.Pending();
        }
    }

}