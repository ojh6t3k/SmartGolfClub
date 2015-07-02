using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/Communication/CommWiFi")]
	public class CommWiFi : CommObject
	{
		public string ipAddress = "192.168.240.1"; // Arduino Yun default IP
		public int port = 5555; // Arduino Yun Bridge Port

		public enum LibraryClass
		{
			Serial,
			Bridge
		}
		public LibraryClass usingLibrary = LibraryClass.Bridge;
		public int baudrate = 115200;
		public string streamClass = "Serial";

		public string errorMessage;

		private Socket _socket;

		void Awake()
		{
		}		

		public override void Open()
		{
			try
			{
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_socket.NoDelay = true;
				_socket.ReceiveBufferSize = 4096;
				_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

				SocketAsyncEventArgs e = new SocketAsyncEventArgs();
				e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
				e.UserToken = _socket;			
				e.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
				_socket.ConnectAsync(e);
			}
			catch(Exception e)
			{
				Debug.Log(e);
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}
		
		public override void Close()
		{
			try
			{
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Close();
			}
			catch(Exception)
			{
			}
		}
		
		public override void Write(byte[] bytes)
		{
			try
			{
				_socket.Send(bytes);
			}
			catch(Exception e)
			{
				Debug.Log("send error");
				Debug.Log(e);
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
			}
		}
		
		public override byte[] Read()
		{
			List<byte> bytes = new List<byte>();

			try
			{
				if(_socket.Available > 0)
				{
					byte[] rcvData = new byte[256];
					int count = _socket.Receive(rcvData);
					for(int i=0; i<count; i++)
						bytes.Add(rcvData[i]);
				}
			}
			catch(Exception)
			{
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
			}

			return bytes.ToArray();
		}
		
		public override bool IsOpen
		{
			get
			{
				if(_socket == null)
					return false;
				
				return _socket.Connected;
			}
		}

		private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
			if(_socket.Connected == true)
			{
				if(OnOpened != null)
					OnOpened(this, null);
			}
			else
			{
				errorMessage = e.SocketError.ToString();
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}

		public override string[] SketchIncludes ()
		{
			List<string> includes = new List<string>();
			switch(usingLibrary)
			{
			case LibraryClass.Serial:
				break;

			case LibraryClass.Bridge:
				includes.Add("#include <Bridge.h>");
				includes.Add("#include <YunServer.h>");
				includes.Add("#include <YunClient.h>");
				break;
			}
			return includes.ToArray();
		}

		public override string SketchDeclaration ()
		{
			StringBuilder source = new StringBuilder();
			switch(usingLibrary)
			{
			case LibraryClass.Serial:
				break;
				
			case LibraryClass.Bridge:
				source.AppendLine("YunServer server;");
				break;
			}
			return source.ToString();
		}

		public override string SketchSetup ()
		{
			StringBuilder source = new StringBuilder();
			switch(usingLibrary)
			{
			case LibraryClass.Serial:
				if(streamClass.Equals("Serial") == true || streamClass.Equals("Serial0") == true)
				{
					source.AppendLine(string.Format("  UnityApp.begin({0:d});", baudrate));
				}
				else
				{
					source.AppendLine(string.Format("  {0}.begin({1:d});", streamClass, baudrate));
					source.AppendLine(string.Format("  UnityApp.begin((Stream*)&{0});", streamClass));
				}
				break;
				
			case LibraryClass.Bridge:
				source.AppendLine("  Bridge.begin();");
				source.AppendLine("  server.noListenOnLocalhost();");
				source.AppendLine("  server.begin();");
				source.AppendLine("  UnityApp.begin();");
				break;
			}
			return source.ToString();
		}

		public override string SketchLoop ()
		{
			StringBuilder source = new StringBuilder();
			switch(usingLibrary)
			{
			case LibraryClass.Serial:
				source.AppendLine("  UnityApp.process();");
				break;
				
			case LibraryClass.Bridge:
				source.AppendLine("  YunClient client = server.accept();");
				source.AppendLine("");
				source.AppendLine("  if(client)");
				source.AppendLine("  {");
				source.AppendLine("    while(client.connected())");
				source.AppendLine("      UnityApp.process((Stream*)&client);");
				source.AppendLine("    client.stop();");
				source.AppendLine("  }");
				source.AppendLine("  else");
				source.AppendLine("    UnityApp.process();");
				break;
			}
			return source.ToString();
		}
	}
}
