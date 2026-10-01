using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WatsonTcp;

namespace CommonLibrary;

public static class WatsonCheckerClient
{
	private static string host = "localhost";

	private const int port = 9010;

	private static DateTime LastTimeTryConnect = DateTime.MinValue;

	private static string NameApp { get; set; } = "";

	private static WatsonTcpClient WatsonTcpClient { get; set; }

	public static void Setup(string nameApp)
	{
		NameApp = nameApp;
		WatsonTcpClient = new WatsonTcpClient(host, 9010);
		WatsonTcpClient.Events.ServerConnected += ServerConnected;
		WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
		WatsonTcpClient.Events.MessageReceived += MessageReceived;
	}

	public static bool Start()
	{
		if (CommonStaticFunction.IsAfalika())
		{
			return true;
		}
		return Connect();
	}

	private static void ServerConnected(object sender, ConnectionEventArgs e)
	{
		SendMessage(JsonConvert.SerializeObject(new AppExchange
		{
			TypeProject = TypesProject.Provider,
			TypeData = TypesData.SetClient,
			DataStr = NameApp
		}));
	}

	private static void ServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		Connect();
	}

	private static void MessageReceived(object sender, MessageReceivedEventArgs e)
	{
		Encoding.UTF8.GetString(e.Data);
	}

	public static bool Connect()
	{
		Task.Run(delegate
		{
			while (!WatsonTcpClient.Connected)
			{
				try
				{
					WatsonTcpClient.Connect();
				}
				catch (Exception ex)
				{
					if (!CommonStaticFunction.IsAfalika())
					{
						Console.WriteLine(NameApp + " Ошибка подключения с Checker. " + ex.Message);
					}
					Thread.Sleep(10000);
					continue;
				}
				break;
			}
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " " + NameApp + ". Соединение с Checker установлено");
		});
		return true;
	}

	public static SyncResponse SendAndWaitMessage(string message, int millisec = 5000)
	{
		SyncResponse result = null;
		try
		{
			result = WatsonTcpClient.SendAndWaitAsync(millisec, message).Result;
		}
		catch
		{
		}
		return result;
	}

	public static void SendMessage(string message)
	{
		if (WatsonTcpClient != null && WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(message);
		}
	}

	public static void SendMessage(byte[] bytes)
	{
		if (WatsonTcpClient != null && WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(bytes);
		}
	}

	public static void UploadData()
	{
		SendMessage(JsonConvert.SerializeObject(new AppExchange
		{
			TypeProject = TypesProject.CupisBot,
			TypeData = TypesData.ClientUpdateChecker,
			DataStr = NameApp
		}, CommonStaticFunction.JsonSettings));
	}
}
