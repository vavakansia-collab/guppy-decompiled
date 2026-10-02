using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Starksoft.Aspen.Proxy;

public class ExtendedTcpClient
{
	public delegate void OnDataReceivedHandler(byte[] data);

	public class TcpStream
	{
		private NetworkStream stream;

		private SslStream secureStream;

		private bool isSecure;

		public TcpStream(NetworkStream stream)
		{
			this.stream = stream;
		}

		public TcpStream(SslStream stream)
		{
			secureStream = stream;
			isSecure = true;
		}

		public void Write(byte[] buffer)
		{
			if (!isSecure)
			{
				stream.Write(buffer, 0, buffer.Length);
			}
			else
			{
				secureStream.Write(buffer, 0, buffer.Length);
			}
		}

		public int Read(byte[] buffer)
		{
			if (!isSecure)
			{
				return stream.Read(buffer, 0, buffer.Length);
			}
			return secureStream.Read(buffer, 0, buffer.Length);
		}
	}

	public const int BUFFER_SIZE = 1048576;

	private TcpClient client;

	private Stream stream;

	private bool isSecure;

	private string address;

	private string host;

	private string proxyAddress;

	private string proxyUsername;

	private string proxyPassword;

	private int port;

	private int proxyPort;

	public event OnDataReceivedHandler OnDataReceived;

	public ExtendedTcpClient(string address, int port, bool isSecure = false, string host = null, string proxyAddress = null, int proxyPort = -1, string proxyUsername = null, string proxyPassword = null)
	{
		if (host == null)
		{
			host = null;
		}
		this.address = address;
		this.host = host;
		this.port = port;
		this.isSecure = isSecure;
		this.proxyAddress = proxyAddress;
		this.proxyPort = proxyPort;
		this.proxyUsername = proxyUsername;
		this.proxyPassword = proxyPassword;
	}

	public void Connect()
	{
		if (client != null && client.Connected)
		{
			throw new Exception("Client already connected");
		}
		if (proxyAddress != null)
		{
			if (proxyUsername == null)
			{
				client = new HttpProxyClient(proxyAddress, proxyPort).CreateConnection(address, port);
			}
			else
			{
				client = new HttpProxyClient(proxyAddress, proxyPort, proxyUsername, proxyPassword).CreateConnection(address, port);
			}
		}
		else
		{
			client = new TcpClient();
			client.Connect(address, port);
		}
		stream = client.GetStream();
		if (isSecure)
		{
			SslStream sslStream = new SslStream(stream);
			X509CertificateCollection collection = new X509CertificateCollection();
			sslStream.AuthenticateAsClient(host, collection, SslProtocols.Tls12, checkCertificateRevocation: false);
			stream = sslStream;
		}
		new Thread(ListenWorker).Start();
	}

	public void Close()
	{
		client.Close();
	}

	private void ListenWorker()
	{
		byte[] buffer = new byte[1048576];
		while (client.Connected)
		{
			int count = stream.Read(buffer, 0, buffer.Length);
			if (count != 0)
			{
				byte[] data = new byte[count];
				for (int i = 0; i < count; i++)
				{
					data[i] = buffer[i];
				}
				this.OnDataReceived?.Invoke(data);
			}
		}
	}

	public void Send(string data)
	{
		Send(Encoding.UTF8.GetBytes(data));
	}

	public void Send(byte[] data)
	{
		stream.Write(data, 0, data.Length);
	}

	public void SendHTTP(string method, string url, List<string> headers)
	{
		string head = method + " " + url + " HTTP/1.1\r\nHost: " + host + "\r\n";
		foreach (string item in headers)
		{
			head = head + item + "\r\n";
		}
		head = head.TrimEnd() + "\r\n\r\n";
		Send(head);
	}

	private void Create()
	{
		SslStream sslStream = new SslStream(new HttpProxyClient("127.0.0.1", 8000).CreateConnection("www.google.com", 443).GetStream());
		sslStream.AuthenticateAsClient("www.google.com");
		string h = "GET / HTTP/1.1\r\nHost: www.google.com\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/109.0\r\nAccept: */*\r\n\r\n";
		byte[] b = Encoding.UTF8.GetBytes(h);
		byte[] bb = new byte[1024];
		sslStream.Write(b, 0, b.Length);
		sslStream.Read(bb, 0, bb.Length);
		Encoding.UTF8.GetString(bb);
	}
}
