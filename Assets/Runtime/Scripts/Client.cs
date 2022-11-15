//-------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" createdby="gblikas">
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
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace XRRemote
{

#if UNITY_EDITOR
    public abstract class Client : MonoBehaviour, IConnection
    {
        private TcpClient tcpClient; 
        private Thread clientReceiveThread;
        
        public LogLevel logLevel = LogLevel.MINIMAL;
        
        public string IP = "10.0.0.98";

        private readonly int byteLimit = 1024;

        private Queue<byte[]> messageQueue = new Queue<byte[]>();

        public bool log
        {
            get
            {
                return logLevel == LogLevel.MINIMAL || logLevel == LogLevel.VERBOSE;
            }
        }

        string editorName = string.Empty; 

        public ConnectionState connectionState { get; private set; }

        public bool lastConnected = false;
        public bool connected
        {
            get {
                bool _connected = connectionState == ConnectionState.CONNECTED && tcpClient != null;
                if (!_connected && lastConnected)
                {
                    if (log)
                    {
                        Debug.Log($"NOT_CONNECTED");
                    }
                }
                lastConnected = _connected;
                return _connected;
            }
        }

        public Action<ConnectionState> onConnection;

        public Action onDisconnection;

        private void Awake()
        {
            editorName = $"{SystemInfo.deviceUniqueIdentifier}_{DateTime.Now.ToString("hhmmss")}";
        }

        private ConnectionState LastConnectionState = ConnectionState.DISCONNECTED;
        private void Update() {
            if (connectionState == ConnectionState.CONNECTED && LastConnectionState == ConnectionState.DISCONNECTED)
            {
                OnConnection();
            }
            if (connectionState == ConnectionState.DISCONNECTED && LastConnectionState == ConnectionState.CONNECTED)
            {
                OnDisconnection();
            }
            
            lock (messageQueue) {
                while (messageQueue.Count > 0) {
                    byte[] message = messageQueue.Dequeue();
                    MessageReceived(message.Deserialize<object>());
                }
            }
        }

        public bool Initialize()
        {
            try
            {
                connectionState = ConnectionState.DISCONNECTED;
			
                clientReceiveThread = new Thread (ListenForData); 			
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
                return true;
            }
            catch (Exception e)
            {
                if (log)
                    Debug.LogException(e);
            }   
            return false;
        }

        private void ListenForData() {
            try {
                tcpClient = new TcpClient(IP, 8053);
                OnConnection();

                Byte[] bytes = new Byte[byteLimit];
                while (true)
                {
                    using (NetworkStream stream = tcpClient.GetStream()) {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);

                            lock (messageQueue) {
                                messageQueue.Enqueue(incomingData);
                            }
                            
                            string serverMessage = Encoding.ASCII.GetString(incomingData);
                            Debug.Log("server message received as: " + serverMessage);
                        }
                    }
                }
            }
            catch (SocketException socketException) {
                Debug.LogException(socketException);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }
        
        public abstract void MessageReceived(object obj);

        public void OnConnection()
        {
            if (log) Debug.Log(FormatConnectionMessage($"connection event: incoming"));
            connectionState = ConnectionState.CONNECTED;
            onConnection?.Invoke(connectionState);
        }

        public void OnDisconnection()
        {
            if (!connected) return;
            onDisconnection?.Invoke(); 
        }

        public void Disconnect()
        {
            DisconnectAll();
        }

        public void DisconnectAll()
        {
            lock(tcpClient) {
                if (!connected) return;
                if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: Closing Connection"));
                connectionState = ConnectionState.DISCONNECTED;
#if UNITY_2017_1_OR_NEWER
                tcpClient?.Close();
#endif
            }
        }

        public bool Send(object serializeableObject)
        {
            if (!connected) return false;
            byte[] data = serializeableObject.SerializeToByteArray();
            return Send(data);
        }

        public bool Send(byte[] data)
        {
            if (!connected) return false;
            //if (log) Debug.Log(FormatConnectionMessage($"send event from {(tcpClient.Client.LocalEndPoint as IPEndPoint)?.Address} on channel {(tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address}"));
            try {
                NetworkStream stream = tcpClient.GetStream();
                if (stream.CanWrite) {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            return true;
        }

        public void ToggleLogLevel(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        public string FormatConnectionMessage(string baseMessage)
        {
            if (!connected) return baseMessage;//string.Empty;
            return $"Client {IP}: {baseMessage}";
        }
    }
#endif

}
