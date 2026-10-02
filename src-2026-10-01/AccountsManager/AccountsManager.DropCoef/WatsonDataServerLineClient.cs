using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookmakerLibrary;
using CommonLibrary;
using Newtonsoft.Json;
using SnapshotsLibrary;
using WatsonTcp;

namespace AccountsManager.DropCoef;

public static class WatsonDataServerLineClient
{
	private static string Host;

	private const int port = 9011;

	public static WatsonTcpClient WatsonTcpClient { get; set; }

	static WatsonDataServerLineClient()
	{
		Host = "localhost";
		Host = CommonStaticFunction.GetIpServer();
		if (Environment.MachineName == "STELLARBOUND")
		{
			Host = "localhost";
		}
		if (Environment.MachineName.ToUpper() == "Server2696".ToUpper())
		{
			Host = "localhost";
			Host = "192.168.0.1";
		}
		if (Environment.MachineName.ToUpper().Contains("Local".ToUpper()))
		{
			Host = "192.168.0.1";
		}
		if (CommonStaticFunction.IsMainServer)
		{
			Host = "192.168.0.1";
		}
		WatsonTcpClient = new WatsonTcpClient(Host, 9011);
		WatsonTcpClient.Events.ServerConnected += OnServerConnected;
		WatsonTcpClient.Events.ServerDisconnected += OnServerDisconnected;
		WatsonTcpClient.Events.MessageReceived += OnMessageReceived;
		WatsonTcpClient.Events.StreamReceived += OnStreamReceived;
		WatsonTcpClient.Events.ExceptionEncountered += OnExceptionEncountered;
		WatsonTcpClient.Callbacks.SyncRequestReceivedAsync = OnSyncRequestReceivedAsync;
		Task.Run((Action)PingWorker);
	}

	public static bool Start()
	{
		return Connect();
	}

	public static bool Connect()
	{
		if (CommonStaticFunction.IsAfalika())
		{
			return false;
		}
		Task.Run(delegate
		{
			while (true)
			{
				try
				{
					Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} [WatsonClientExchange.Connect] Try connect to {Host}:{9011}...");
					try
					{
						WatsonTcpClient.Disconnect();
					}
					catch
					{
					}
					WatsonTcpClient.Connect();
				}
				catch (Exception ex)
				{
					Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [WatsonClientExchange.Connect] Server connection refused. " + ex.Message);
					Thread.Sleep(5000);
					continue;
				}
				break;
			}
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [WatsonClientExchange.Connect] Connected");
		});
		return true;
	}

	public static void Disconnect()
	{
		if (WatsonTcpClient.Connected)
		{
			WatsonTcpClient.Disconnect();
		}
	}

	private static void PingWorker()
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

	public static void SendMessage(string message)
	{
		if (WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(message);
		}
	}

	public static void SendMessage(byte[] bytes)
	{
		if (WatsonTcpClient.Connected)
		{
			WatsonTcpClient.SendAsync(bytes);
		}
	}

	public static SyncResponse SendAndWaitMessage(string message, int millisec = 5000)
	{
		SyncResponse response = null;
		try
		{
			response = WatsonTcpClient.SendAndWaitAsync(millisec, message).Result;
		}
		catch
		{
		}
		return response;
	}

	public static async Task<SyncResponse> SendSyncMessage(string message, int timeout = 5000)
	{
		SyncResponse response = null;
		try
		{
			response = await WatsonTcpClient.SendAndWaitAsync(timeout, message);
		}
		catch
		{
		}
		return response;
	}

	private static void OnExceptionEncountered(object sender, ExceptionEventArgs e)
	{
		Console.WriteLine("====================");
		Console.WriteLine($"[WatsonClientExchange.OnExceptionEncountered] {e.Exception}");
		Console.WriteLine("====================");
	}

	private static void OnStreamReceived(object sender, StreamReceivedEventArgs e)
	{
	}

	private static void OnServerConnected(object sender, ConnectionEventArgs e)
	{
		SendMessage(new AppExchange
		{
			TypeProject = TypesProject.AccountsCenter,
			TypeData = TypesData.SetClient,
			DataStr = Settings.AMSettings.OwnerAccounts.Name + ":" + ViewModelMainWindow.Version
		}.Serialize());
	}

	private static void OnServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		Thread.Sleep(5000);
		Connect();
	}

	private static void OnMessageReceived(object sender, MessageReceivedEventArgs req)
	{
		Encoding.UTF8.GetString(req.Data);
	}

	private static async Task<SyncResponse> OnSyncRequestReceivedAsync(SyncRequest req)
	{
		return new SyncResponse(req, "Hello back at you!");
	}

	public static List<Snapshot> GetSnapshotHeadersByData(int idSport, DateTime dateTime)
	{
		AppExchangeSnapshots appExchange = new AppExchangeSnapshots();
		appExchange.TypeData = TypesSnapshotsData.GetHeadersByData;
		appExchange.DataStr = idSport + "_" + dateTime.ToString("d");
		SyncResponse syncResponse = SendAndWaitMessage(appExchange.Serialize(), 30000);
		if (syncResponse == null)
		{
			return null;
		}
		appExchange = AppExchangeSnapshots.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<List<Snapshot>>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static async Task<(List<MatchLine>, List<MatchLineStat>, Dictionary<int, int>)> GetMatchesCurrent(string nameBC, int idSport, int interval)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeData = TypesData.GetMatchesCurrent;
		appExchange.DataStr = idSport + "_" + interval + "_" + nameBC;
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize(), 15000);
		if (syncResponse == null)
		{
			return (null, null, null);
		}
		appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return (null, null, null);
		}
		return JsonConvert.DeserializeObject<(List<MatchLine>, List<MatchLineStat>, Dictionary<int, int>)>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static async Task<(List<MatchLine>, List<MatchLineStat>, Dictionary<int, int>)> GetMatchesAtData(string nameBC, int idSport, DateTime dateStartPoint)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeData = TypesData.GetMatchesAtData;
		appExchange.DataStr = idSport + "_" + dateStartPoint.ToString() + "_" + nameBC;
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize(), 20000);
		if (syncResponse == null)
		{
			return (null, null, null);
		}
		appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return (null, null, null);
		}
		return JsonConvert.DeserializeObject<(List<MatchLine>, List<MatchLineStat>, Dictionary<int, int>)>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static async Task<MatchLine> GetMatch(string nameBC, int idSport, long idMatch, DateTime dataStartTime)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeData = TypesData.GetMatch;
		appExchange.DataStr = nameBC + "_" + idSport + "_" + idMatch + "_" + dataStartTime.ToShortDateString();
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize());
		if (syncResponse == null)
		{
			return null;
		}
		appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<MatchLine>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static async Task<(List<MatchLine>, List<MatchLineStat>)> GetMatches(List<MatchDto> matchesDto)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeData = TypesData.GetMatches;
		appExchange.DataStr = JsonConvert.SerializeObject(matchesDto);
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize());
		if (syncResponse == null)
		{
			return (null, null);
		}
		appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return (null, null);
		}
		return JsonConvert.DeserializeObject<(List<MatchLine>, List<MatchLineStat>)>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}
}
