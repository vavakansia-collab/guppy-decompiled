using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary;
using Newtonsoft.Json;
using WatsonTcp;

namespace BookmakerLibrary;

public class WatsonLineProviderClient
{
	private static string host = "localhost";

	private const int port = 9011;

	private DateTime LastTimePing = DateTime.MinValue;

	private WatsonTcpClient WatsonTcpClient { get; set; }

	private Provider Provider { get; set; }

	private ProtobufBC ProtobufBC { get; set; }

	public event Action OnSuccessUpdate;

	public event Action OnDisconnectServer;

	public void Setup(Provider initProvider)
	{
		Provider = initProvider;
		ProtobufBC = new ProtobufBC(Provider);
		if (CommonStaticFunction.IsMainServer)
		{
			host = "192.168.0.1";
		}
		WatsonTcpClient = new WatsonTcpClient(host, 9011);
		WatsonTcpClient.Events.ServerConnected += ServerConnected;
		WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
		WatsonTcpClient.Events.MessageReceived += MessageReceived;
		WatsonTcpClient.Callbacks.SyncRequestReceivedAsync = SyncRequestReceivedAsync;
	}

	public bool Start()
	{
		Task.Run((Action)PingWorker);
		Task.Run((Action)ChackWorker);
		return Connect();
	}

	private async Task<SyncResponse> SyncRequestReceivedAsync(SyncRequest req)
	{
		string json = Encoding.UTF8.GetString(req.Data);
		AppExchange appExchange = JsonConvert.DeserializeObject<AppExchange>(json);
		if (appExchange.TypeData == TypesData.ActivateMatches)
		{
			JsonConvert.DeserializeObject<IEquatable<long>>(appExchange.DataStr);
			return new SyncResponse(req, "OK");
		}
		return null;
	}

	private void ServerConnected(object sender, ConnectionEventArgs e)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.Provider;
		appExchange.TypeData = TypesData.SetClient;
		appExchange.DataStr = Provider.Name;
		string message = JsonConvert.SerializeObject(appExchange);
		SendMessage(message);
	}

	private void ServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		this.OnDisconnectServer?.Invoke();
		Thread.Sleep(5000);
		Connect();
	}

	private void MessageReceived(object sender, MessageReceivedEventArgs req)
	{
		string text = Encoding.UTF8.GetString(req.Data);
		if (text.StartsWith("PING"))
		{
			LastTimePing = DateTime.Now;
		}
		else
		{
			string text2 = Encoding.UTF8.GetString(req.Data);
		}
	}

	public bool Connect()
	{
		bool flag = true;
		try
		{
			if (!CommonStaticFunction.IsAfalika())
			{
				Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} [AppExchangeClient.Connect] Try connect to {host}:{9011}...");
			}
			WatsonTcpClient.Connect();
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [" + Provider.Name + ".Connect] Connected");
			return true;
		}
		catch (Exception ex)
		{
			if (!CommonStaticFunction.IsAfalika())
			{
				Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [" + Provider.Name + ".Connect] Server connection refused. " + ex.Message);
			}
			Thread.Sleep(5000);
			Setup(Provider);
			Connect();
			return false;
		}
	}

	private void Disconnect()
	{
		try
		{
			WatsonTcpClient.Disconnect();
		}
		catch
		{
		}
	}

	public SyncResponse SendAndWaitMessage(string message, int millisec = 5000)
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

	public void SendMessage(string message)
	{
		if (WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(message);
		}
	}

	public void SendMessage(byte[] bytes)
	{
		if (WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(bytes);
		}
	}

	public void UploadData(MatchLineDto matchLineDto)
	{
		if (WatsonTcpClient == null || !WatsonTcpClient.Connected)
		{
			return;
		}
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.Provider;
		appExchange.TypeData = TypesData.ProviderMatchesLine;
		appExchange.DataStr = JsonConvert.SerializeObject(matchLineDto);
		string message = JsonConvert.SerializeObject(appExchange);
		SyncResponse syncResponse = SendAndWaitMessage(message, 15000);
		if (syncResponse == null)
		{
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " Ошибка обновления данных на сервер: не получен ответ или null");
			return;
		}
		string text = Encoding.UTF8.GetString(syncResponse.Data);
		if (text == "OK")
		{
			this.OnSuccessUpdate?.Invoke();
		}
		else
		{
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " Ошибка обновления данных на сервер: " + text);
		}
	}

	private void PingWorker()
	{
		while (true)
		{
			try
			{
				SendMessage("PING1");
			}
			catch
			{
			}
			Thread.Sleep(5000);
		}
	}

	private void ChackWorker()
	{
		while (true)
		{
			if (LastTimePing != DateTime.MinValue && DateTime.Now.Subtract(LastTimePing).TotalSeconds > 30.0)
			{
				Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [" + Provider.Name + "] Disconnect from Server.");
				Disconnect();
				Connect();
			}
			Thread.Sleep(10000);
		}
	}
}
