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
using UnityEngine.Assertions;

namespace XRRemote
{

#if UNITY_EDITOR
    public abstract class Client : MonoBehaviour, IConnection
    {
        private TcpClient tcpClient; 
        private ThreadWatcher clientReceiveThread;
        private readonly object connectionLock = new object();
        private readonly object tcpLock = new object();
        
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
            editorName = $"{SystemInfo.deviceUniqueIdentifier}_{DateTime.Now.ToString("hhmmss")}";
        }

        protected void Update() {
            if (connectionState == ConnectionState.CONNECTED && LastConnectionState == ConnectionState.DISCONNECTED)
            {
                OnConnection();
                connectionState = ConnectionState.CONNECTED;
            }
            if (connectionState == ConnectionState.DISCONNECTED && LastConnectionState == ConnectionState.CONNECTED)
            {
                OnDisconnection();
                connectionState = ConnectionState.DISCONNECTED;
            }
            
            lock (messageQueue) {
                while (messageQueue.Count > 0) {
                    byte[] message = messageQueue.Dequeue();
                    MessageReceived(message.Deserialize<object>());
                }
            }
        }

        protected void OnDisable()
        {
            DisconnectAll();
        }

        private void OnApplicationQuit() {
            clientReceiveThread = null;
            tcpClient?.Close();
        }

        public bool Initialize()
        {
            
            if (clientReceiveThread != null && clientReceiveThread.thread.IsAlive)
            {
                Debug.Log("thread is already initialized");
                return true;
            }
            try
            {
                connectionState = ConnectionState.DISCONNECTED;

                Thread thread = new Thread(ListenForData) {IsBackground = true};
                clientReceiveThread = new ThreadWatcher(thread);
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
                Debug.Log($"Attempting connection to {IP}:{8053}... | Timeout 400ms");
                tcpClient = new TcpClient(IP, 8053);
                connectionState = ConnectionState.CONNECTED;
                Debug.Log($"Connected");
            }
            catch (SocketException socketException) {
                Debug.LogException(socketException);
                return;
            }
            catch (Exception e) {
                Debug.LogException(e);
                //Disconnect();
            }
            Byte[] bytes = new Byte[byteLimit];

            while (true) {
                try {
                    using (NetworkStream stream = tcpClient.GetStream()) {
                        int length;
                        while ((length = stream.Read(bytes, 0, 4)) != 0) {
                            try { 
                                var destinationArray = new byte[4];
                                Array.Copy(bytes, 0, destinationArray, 0, length);
                                int packetLength = BitConverter.ToInt32(destinationArray, 0);
                                
                                var incomingData = new byte[packetLength];
                                int remainder = packetLength;
                                int index = 0;
                                while (packetLength != index) {
                                    length = stream.Read(bytes, 0, Math.Min(remainder, bytes.Length));
                                    remainder -= length;
                                    
                                    Array.Copy(bytes, 0, incomingData, index, length);
                                    
                                    index += length;
                                }
                                lock (messageQueue) {
                                    Assert.AreEqual(index, packetLength);
                                    messageQueue.Enqueue(incomingData);
                                }
                            }
                            catch (ArgumentOutOfRangeException e) {
                                Debug.LogException(e);
                                throw;
                            }
                            catch (ArgumentException e) {
                                Debug.LogException(e);
                                throw;
                            }
                        }
                    }
                }
                catch (SocketException socketException) {
                    Debug.LogException(socketException);
                    //Disconnect();
                    break;
                }
                catch (Exception e) {
                    Debug.LogException(e);
                    break;
                }
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
            if (connectionState == ConnectionState.DISCONNECTED) return;
            onDisconnection?.Invoke();
        }

        public void Disconnect()
        {
            DisconnectAll();
        }

        public void DisconnectAll()
        {
            if (connectionState == ConnectionState.DISCONNECTED) return;
            if (log) Debug.Log(FormatConnectionMessage($"DISCONNECTION_EVENT reason: Closing Connection"));
            connectionState = ConnectionState.DISCONNECTED;
        }

        public bool Send(object serializeableObject)
        {
            if (connectionState == ConnectionState.DISCONNECTED) return false;
            byte[] data = serializeableObject.SerializeToByteArray();
            return Send(data);
        }

        public bool Send(byte[] data) 
        {
            if (connectionState == ConnectionState.DISCONNECTED) return false;
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
            if (connectionState == ConnectionState.DISCONNECTED) return baseMessage;//string.Empty;
            return $"Client {IP}: {baseMessage}";
        }
    }
#endif

}
