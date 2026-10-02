using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountsManager;
using CommonLibrary;
using CompareLibrary;
using Newtonsoft.Json;
using SnapshotsLibrary;
using WatsonTcp;

namespace SnapshotsViewer;

public static class WatsonClientExchange
{
	private static string host;

	private const int port = 9010;

	public static WatsonTcpClient WatsonTcpClient { get; set; }

	static WatsonClientExchange()
	{
		host = "localhost";
		host = CommonStaticFunction.GetIpServer();
		if (Environment.MachineName == "STELLARBOUND")
		{
			host = "localhost";
		}
		if (Environment.MachineName.ToUpper() == "Server2696".ToUpper() || Environment.MachineName.ToUpper().Contains("Local".ToUpper()))
		{
			host = "localhost";
		}
		if (CommonStaticFunction.IsMainServer)
		{
			host = "localhost";
			host = "192.168.0.1";
		}
		WatsonTcpClient = new WatsonTcpClient(host, 9010);
		WatsonTcpClient.Events.ServerConnected += OnServerConnected;
		WatsonTcpClient.Events.ServerDisconnected += OnServerDisconnected;
		WatsonTcpClient.Events.MessageReceived += OnMessageReceived;
		WatsonTcpClient.Events.StreamReceived += OnStreamReceived;
		WatsonTcpClient.Events.ExceptionEncountered += OnExceptionEncountered;
		WatsonTcpClient.Callbacks.SyncRequestReceivedAsync = OnSyncRequestReceivedAsync;
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
					Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} [WatsonClientExchange.Connect] Try connect to {host}:{9010}...");
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

	public static List<Snapshot> GetSnapshotsByData(int idSport, DateTime dateTime)
	{
		AppExchangeSnapshots appExchange = new AppExchangeSnapshots();
		appExchange.TypeData = TypesSnapshotsData.GetSnapshotsByData;
		appExchange.DataStr = idSport + "_" + dateTime.ToString("d");
		SyncResponse syncResponse = SendAndWaitMessage(appExchange.Serialize());
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

	public static List<CompareMatch> GetCompareMatchesByData(int idSport, DateTime dateTime)
	{
		AppExchangeSnapshots appExchange = new AppExchangeSnapshots();
		appExchange.TypeData = TypesSnapshotsData.GetCompareMatchesByData;
		appExchange.DataStr = idSport + "_" + dateTime.ToString("d");
		SyncResponse syncResponse = SendAndWaitMessage(appExchange.Serialize());
		if (syncResponse == null)
		{
			return null;
		}
		appExchange = AppExchangeSnapshots.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<List<CompareMatch>>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static Snapshot GetSnapshotMatch(CompareMatch compareMatch)
	{
		AppExchangeSnapshots appExchange = new AppExchangeSnapshots();
		appExchange.TypeData = TypesSnapshotsData.GetSnapshotMatch;
		appExchange.DataStr = JsonConvert.SerializeObject(compareMatch);
		SyncResponse syncResponse = SendAndWaitMessage(appExchange.Serialize(), 25000);
		if (syncResponse == null)
		{
			return null;
		}
		appExchange = AppExchangeSnapshots.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
		if (appExchange == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<Snapshot>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}
}
