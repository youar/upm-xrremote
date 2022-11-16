//-------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" createdby="gblikas">
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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace XRRemote
{
    public abstract class Server : MonoBehaviour, IConnection
    {
        private TcpListener tcpServer;
        private Thread tcpListenerThread;
        private TcpClient connectedTcpClient;

        private object connectionLock;
        
        public LogLevel logLevel = LogLevel.MINIMAL;

        public bool log => logLevel == LogLevel.MINIMAL || logLevel == LogLevel.VERBOSE;

        string playerName = string.Empty;

        private Queue<byte[]> messageQueue = new Queue<byte[]>();

        public ConnectionState _connectionState = ConnectionState.DISCONNECTED;

        protected ConnectionState connectionState {
            get {
                lock(connectionLock) {
                    return _connectionState;
                }
            }
            private set {
                lock (connectionLock) {
                    LastConnectionState = _connectionState;
                    _connectionState = value;
                }
            }
        }

        private ConnectionState LastConnectionState = ConnectionState.DISCONNECTED;

        public Action<ConnectionState> onConnection;

        public Action onDisconnection;

        private void Awake()
        {
            playerName = $"player_{SystemInfo.deviceUniqueIdentifier}_{DateTime.Now.ToString("hhmmss")}";
        }

        public bool Initialize()
        {
            try
            {
                connectionState = ConnectionState.DISCONNECTED;
                
                tcpListenerThread = new Thread (ListenForIncommingRequests);
                tcpListenerThread.IsBackground = true;
                tcpListenerThread.Start();
                
                onConnection += (status) => { if (log) Debug.Log(FormatConnectionMessage($"CONNECTION_STATUS value: {status}")); };
                onDisconnection += () => { if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: editor disconnected")); };

                return true;

            }
            catch (Exception e)
            {
                if (log)
                    Debug.LogException(e);
                return false;
            }
        }

        public void OnConnection()
        {
            if (log) Debug.Log(FormatConnectionMessage($"connection event: incoming"));
            onConnection?.Invoke(connectionState);
        }

        private void Update() {
            lock(tcpServer) {
                if (connectionState == ConnectionState.CONNECTED && LastConnectionState == ConnectionState.DISCONNECTED) {
                    OnConnection();
                }

                if (connectionState == ConnectionState.DISCONNECTED && LastConnectionState == ConnectionState.CONNECTED) {
                    OnDisconnection();
                }
            }
            
            lock (messageQueue) {
                while (messageQueue.Count > 0) {
                    byte[] message = messageQueue.Dequeue();
                    MessageReceived(message.Deserialize<object>());
                }
            }
        }

        public void OnDisconnection()
        {
            tcpServer?.Stop();
            tcpServer = null;
            onDisconnection?.Invoke();
        }
        
        public void Disconnect()
        {
            DisconnectAll();
        }

        public void DisconnectAll()
        {
            if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: starting disconnect"));
            connectionState = ConnectionState.DISCONNECTED;
        }

        public bool Send(object serializeableObject)
        {
            try
            {
                if (connectionState == ConnectionState.DISCONNECTED) return false;
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
                if (connectionState == ConnectionState.DISCONNECTED) return false;

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

        public abstract void MessageReceived(object obj);

        public void ToggleLogLevel(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        public string FormatConnectionMessage(string baseMessage)
        {
            if (connectionState == ConnectionState.DISCONNECTED) return string.Empty;
            return $"Server {(connectedTcpClient?.Client.RemoteEndPoint as IPEndPoint)?.Address}: {baseMessage}";
        }
        
        private void ListenForIncommingRequests () {
            try {
                tcpServer = TcpListener.Create(8053);
                tcpServer.Server.ReceiveTimeout = 4000;
                tcpServer.Start();
		    
                Debug.Log("Server is listening");
                connectionState = ConnectionState.CONNECTED;

                Byte[] bytes = new Byte[1024];
                
                while (true)
                {
                    if (!tcpServer.Pending()) continue;
                    using (connectedTcpClient = tcpServer.AcceptTcpClient())
                    {
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            if (!stream.DataAvailable) continue;
                            int length;
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                byte[] incomingData = new byte[length];
                                Array.Copy(bytes, 0, incomingData, 0, length);
                                
                                lock (messageQueue) {
                                    messageQueue.Enqueue(incomingData);
                                }

                                string clientMessage = Encoding.ASCII.GetString(incomingData);
                                Debug.Log("client message received as: " + clientMessage);
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException) {
                Debug.LogException(socketException);
                Disconnect();
            }
            catch (Exception e) {
                Debug.LogException(e);
                Disconnect();
            }
        }
    }
}