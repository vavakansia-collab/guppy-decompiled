using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountsLibrary;
using CommonLibrary;
using Newtonsoft.Json;
using WatsonTcp;

namespace AccountsManager;

public static class BettingCenterClient
{
	private static string host;

	private const int port = 9005;

	public static WatsonTcpClient WatsonTcpClient { get; set; }

	public static event Action OnServerBettingCenterConnected;

	static BettingCenterClient()
	{
		host = AccountsCenterClient.Host;
		WatsonTcpClient = new WatsonTcpClient(host, 9005);
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
		Task.Run(delegate
		{
			while (true)
			{
				try
				{
					if (!CommonStaticFunction.IsAfalika())
					{
						Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} [BettingCenterClient.Connect] Try connect to {host}:{9005}...");
					}
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
					if (!CommonStaticFunction.IsAfalika())
					{
						Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [BettingCenterClient.Connect] Server connection refused. " + ex.Message);
					}
					Thread.Sleep(5000);
					continue;
				}
				break;
			}
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [BettingCenterClient.Connect] Connected");
		});
		return true;
	}

	public static void Disconnect()
	{
		try
		{
			WatsonTcpClient.Disconnect();
		}
		catch
		{
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

	public static SyncResponse SendSyncMessage(byte[] bytes, int timeout = 5000)
	{
		SyncResponse result = null;
		try
		{
			result = WatsonTcpClient.SendAndWaitAsync(timeout, bytes).Result;
		}
		catch
		{
		}
		return result;
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

	private static void OnExceptionEncountered(object sender, ExceptionEventArgs e)
	{
		Console.WriteLine("====================");
		Console.WriteLine($"[BettingCenterClient.OnExceptionEncountered] {e.Exception}");
		Console.WriteLine("====================");
	}

	private static void OnStreamReceived(object sender, StreamReceivedEventArgs e)
	{
	}

	private static void OnServerConnected(object sender, ConnectionEventArgs e)
	{
		SendMessage(new AppExchange
		{
			TypeProject = TypesProject.AccountsManager,
			TypeData = TypesData.SetClient,
			DataStr = Settings.AMSettings.OwnerAccounts.Name + ":" + ViewModelMainWindow.Version
		}.Serialize());
		BettingCenterClient.OnServerBettingCenterConnected?.Invoke();
	}

	private static void OnServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		if (!ViewModelMainWindow.IsAppBlock)
		{
			Connect();
		}
	}

	private static void OnMessageReceived(object sender, MessageReceivedEventArgs req)
	{
		string text = Encoding.UTF8.GetString(req.Data);
		if (!text.StartsWith("PING"))
		{
			AppExchange appExchange = JsonConvert.DeserializeObject<AppExchange>(text);
			if (appExchange.TypeData == TypesData.UpdateStates)
			{
				_ = DateTime.Now;
				List<StateAccount> states = JsonConvert.DeserializeObject<List<StateAccount>>(appExchange.DataStr);
				ViewModelMainWindow.instance.UpdateStatesAccounts(states);
			}
		}
	}

	private static async Task<SyncResponse> OnSyncRequestReceivedAsync(SyncRequest req)
	{
		AppExchange appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(req.Data));
		if (appExchange.TypeData == TypesData.AccountCookie)
		{
			Guid guidAccount = Guid.Parse(appExchange.DataStr);
			AccountSettings accountSettings = ViewModelMainWindow.instance.AccountsSettingsVM.SingleOrDefault((AccountSettingsVM acVM) => acVM.AccountSettings.GUID.ToString() == guidAccount.ToString()).AccountSettings;
			string dataStr = "";
			if (accountSettings != null)
			{
				dataStr = ViewModelMainWindow.GetAccountCookie(accountSettings.GUID);
			}
			appExchange.DataStr = dataStr;
			return new SyncResponse(req, appExchange.Serialize());
		}
		return new SyncResponse(req, "Hello back at you!");
	}

	public static void PutCommand(AccountCommand command)
	{
		SendMessage(new AppExchange
		{
			TypeProject = TypesProject.AccountsCenter,
			TypeData = TypesData.ExecuteCommand,
			DataStr = JsonConvert.SerializeObject(command)
		}.Serialize());
	}

	public static void UpdateOwners()
	{
		SendMessage(new AppExchange
		{
			TypeData = TypesData.UpdateOwners,
			DataStr = JsonConvert.SerializeObject(OwnersAccountBL.OwnersAccount)
		}.Serialize());
	}

	public static async Task<List<StateAccount>> GetStatesAccountsAsync(List<string> guidsAccounts)
	{
		SyncResponse syncResponse = await SendSyncMessage(new AppExchange
		{
			TypeProject = TypesProject.AccountsManager,
			TypeData = TypesData.GetStatesAccounts,
			DataStr = JsonConvert.SerializeObject(guidsAccounts)
		}.Serialize());
		if (syncResponse == null)
		{
			return null;
		}
		return JsonConvert.DeserializeObject<List<StateAccount>>(CommonStaticFunction.DecompressString(Encoding.UTF8.GetString(syncResponse.Data)));
	}
}
