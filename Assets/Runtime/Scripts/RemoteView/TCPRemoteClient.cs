using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPRemoteClient : MonoBehaviour {  	
	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
	private byte[] message;

	public delegate void OnMessageRecieved(byte[] bytes);

	public OnMessageRecieved OnMessageRecievedEvent;
	private bool _reieved = false;
	
	public string IP = "10.0.0.98";
	void Start () {
		ConnectToTcpServer();     
	}  	

	void Update () {         
		if (Input.GetMouseButtonDown(0)) {  
			Debug.Log("tap!");
			SendMessage();         
		}

		if (_reieved)
		{
			_reieved = false;
			
		}
	}  	

	
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.LogException(e); 		
		} 	
	}  	

	
	
	private void ListenForData() {
		try
		{
			socketConnection = new TcpClient(IP, 8053);
			Byte[] bytes = new Byte[1024];
			while (true)
			{

				Debug.Log("delete me : connected? "+socketConnection.Connected.ToString());
				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;

					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
#if true
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
#else
						lock (message)
						{
							message = incommingData;
							_reieved = true;
						}
#endif
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
	}  	
	/// <summary>
	/// ///////////////// DO it
	/// </summary>
	///
	///
	
	private void SendMessage() {         
		if (socketConnection == null) {             
			return;         
		}  		
		try { 			

			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				string clientMessage = "This is a message from one of your clients."; 				

				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				

				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	} 
}