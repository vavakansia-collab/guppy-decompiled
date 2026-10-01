using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WatsonTcp;

namespace CommonLibrary;

public class WatsonAppCheckerClient
{
	private string NameApp { get; set; } = "";

	private WatsonTcpClient WatsonTcpClient { get; set; }

	public bool Start(string nameApp)
	{
		if (CommonStaticFunction.IsAfalika())
		{
			return true;
		}
		NameApp = nameApp;
		WatsonTcpClient = new WatsonTcpClient("localhost", 9010);
		WatsonTcpClient.Events.ServerConnected += ServerConnected;
		WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
		WatsonTcpClient.Events.MessageReceived += MessageReceived;
		return Connect();
	}

	private void ServerConnected(object sender, ConnectionEventArgs e)
	{
		string message = JsonConvert.SerializeObject(new AppExchange
		{
			TypeProject = TypesProject.Provider,
			TypeData = TypesData.SetClient,
			DataStr = NameApp
		});
		SendMessage(message);
	}

	private void ServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		Thread.Sleep(5000);
		Connect();
	}

	public bool Connect()
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
						Console.WriteLine(NameApp + " Ошибка подключения с AppChecker. " + ex.Message);
					}
					Thread.Sleep(10000);
					continue;
				}
				break;
			}
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " " + NameApp + ". Соединение с AppChecker установлено");
		});
		return true;
	}

	private void MessageReceived(object sender, MessageReceivedEventArgs e)
	{
		Encoding.UTF8.GetString(e.Data);
	}

	public void SendMessage(string message)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["projectName"] = NameApp;
		if (WatsonTcpClient != null && WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(message, dictionary);
		}
	}
}
