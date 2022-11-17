using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;  

public class TCPRemoteServer : MonoBehaviour {  	

	private TcpListener tcpListener;
	private Thread tcpListenerThread;  	
	private TcpClient connectedTcpClient;
	
	void Start () { 		
	
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
	}  	
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("tap!");
			SendMessage();
		} 	
	}
	
	private void OnDisable()
	{
		if (tcpListener != null && tcpListener.Server.IsBound)
		{
			Debug.Log("boop!");
			tcpListener.Stop();
		}
	}

	private void ListenForIncommingRequests () {
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
	}  	

	private void SendMessage() { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			

			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {                 
				string serverMessage = String.Empty; 			

#if true
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
			//	stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);       
				stream.Write(new byte[0], 0, 0);//serverMessageAsByteArray.Length);               

#else
				if (GameObjectTracker.Instance != null)
				{
					TCPPacket packet = new TCPPacket(GameObjectTracker.Instance.pose);
					byte[] result = packet.AsByte();
					stream.Write(result, 0, result.Length);
				}

				#endif
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	} 
}