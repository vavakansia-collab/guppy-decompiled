using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountsLibrary;
using AccountsLibrary.CUPIS;
using CommonLibrary;
using Newtonsoft.Json;
using WatsonTcp;

namespace AccountsManager;

public static class AccountsCenterClient
{
	public static string Host;

	private const int port = 9004;

	private static ProtobufAM ProtobufAM { get; set; }

	public static WatsonTcpClient WatsonTcpClient { get; set; }

	static AccountsCenterClient()
	{
		Host = "localhost";
		ProtobufAM = new ProtobufAM();
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
		if (Environment.MachineName.ToUpper().Contains("Local".ToUpper()) || Environment.MachineName.ToUpper() == "WIN-5DTR86FQT17")
		{
			Host = "192.168.0.1";
		}
		if (CommonStaticFunction.IsMainServer)
		{
			Host = "192.168.0.1";
		}
		WatsonTcpClient = new WatsonTcpClient(Host, 9004);
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
						Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} [AccountsCenterClient.Connect] Try connect to {Host}:{9004}...");
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
						Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [AccountsCenterClient.Connect] Server connection refused. " + ex.Message);
					}
					Thread.Sleep(5000);
					continue;
				}
				break;
			}
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [AccountsCenterClient.Connect] Connected");
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
		SyncResponse response = null;
		try
		{
			response = WatsonTcpClient.SendAndWaitAsync(timeout, bytes).Result;
		}
		catch
		{
		}
		return response;
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

	public static async Task<SyncResponse> SendAndWaitMessage(string message, int millisec = 5000)
	{
		SyncResponse response = null;
		try
		{
			response = await WatsonTcpClient.SendAndWaitAsync(millisec, message);
		}
		catch
		{
		}
		return response;
	}

	public static async Task LoadStaticData()
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.AccountsCenter;
		appExchange.TypeData = TypesData.GetStaticData;
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize());
		if (syncResponse != null)
		{
			appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
			if (appExchange != null)
			{
				StaticDataExchange? staticDataExchange = JsonConvert.DeserializeObject<StaticDataExchange>(appExchange.DataStr);
				AccountsSettingsBL.Statuses = staticDataExchange.StatusesAccount;
				CurrencyBL.Currencies = staticDataExchange.Currencies;
				AccountsLibrary.ProxyBL.ListProxy = staticDataExchange.Proxies;
				CountriesBL.Countries = staticDataExchange.Countries;
			}
		}
	}

	public static async Task<(OwnerAccount, string)> Auth(string login, string password)
	{
		if (!WatsonTcpClient.Connected)
		{
			Connect();
			return (null, "Нет соединения с сервером");
		}
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.AccountsManager;
		appExchange.TypeData = TypesData.Auth;
		appExchange.DataStr = login + ":" + password + ":" + ViewModelMainWindow.Version;
		SyncResponse syncResponse = await SendAndWaitMessage(appExchange.Serialize());
		if (syncResponse == null)
		{
			return (null, "Не получен ответ от сервера");
		}
		string data = Encoding.UTF8.GetString(syncResponse.Data);
		if (string.IsNullOrEmpty(data))
		{
			return (null, "Нет данных от сервера");
		}
		appExchange = AppExchange.Deserialize(data);
		if (!appExchange.DataStr.StartsWith("OK"))
		{
			return (null, appExchange.DataStr);
		}
		try
		{
			OwnerAccount ownerAccount = JsonConvert.DeserializeObject<OwnerAccount>(appExchange.DataStr.Substring(3));
			return (ownerAccount, null);
		}
		catch (Exception ex)
		{
			return (null, "Неопознанная ошибка." + ex.Message);
		}
	}

	public static async Task<List<AccountSettings>> GetAccounts(List<string> guidsAccounts)
	{
		SyncResponse syncResponse = await SendAndWaitMessage(new AppExchange
		{
			TypeProject = TypesProject.AccountsCenter,
			TypeData = TypesData.GetAccounts,
			DataStr = JsonConvert.SerializeObject(guidsAccounts)
		}.Serialize(), 10000);
		if (syncResponse == null)
		{
			return null;
		}
		string data = Encoding.UTF8.GetString(syncResponse.Data);
		if (string.IsNullOrEmpty(data))
		{
			return null;
		}
		return JsonConvert.DeserializeObject<List<AccountSettings>>(CommonStaticFunction.DecompressString(AppExchange.Deserialize(data).DataStr));
	}

	private static void OnExceptionEncountered(object sender, ExceptionEventArgs e)
	{
		Console.WriteLine("====================");
		Console.WriteLine($"[AccountsCenterClient.OnExceptionEncountered] {e.Exception}");
		Console.WriteLine("====================");
	}

	private static void OnStreamReceived(object sender, StreamReceivedEventArgs e)
	{
	}

	private static void OnServerConnected(object sender, ConnectionEventArgs e)
	{
		Settings.ViewModelAuth?.OnServerConnected();
		ViewModelMainWindow.instance?.OnServerConnected();
	}

	private static void OnServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		ViewModelMainWindow.instance?.OnServerDisconnected();
		if (!ViewModelMainWindow.IsAppBlock)
		{
			Connect();
		}
	}

	private static void OnMessageReceived(object sender, MessageReceivedEventArgs req)
	{
		string json = Encoding.UTF8.GetString(req.Data);
		if (json.StartsWith("PING"))
		{
			return;
		}
		if (json.StartsWith("DisconnectClientDoubleEnter"))
		{
			ViewModelMainWindow.IsAppBlock = true;
			ViewModelMainWindow.instance?.OnServerDisconnectedDoubleEnterClient();
			return;
		}
		AppExchange appExchange = JsonConvert.DeserializeObject<AppExchange>(json);
		if (appExchange.TypeData == TypesData.UpdateAccounts)
		{
			if (appExchange.DataStr == "OK")
			{
				ChangesBL.ClearChanges();
				ViewModelMainWindow.instance.UnblockUI();
			}
			else if (appExchange.DataStr.StartsWith("OUTERUPDATE:"))
			{
				ViewModelMainWindow.instance.BlockUI();
				ChangesBL.ChangesReceived(JsonConvert.DeserializeObject<Changes>(appExchange.DataStr.Substring(12)));
				ViewModelMainWindow.instance.UnblockUI();
			}
		}
		else if (appExchange.TypeData == TypesData.GetCupisDataBase)
		{
			CupisDatabase data = JsonConvert.DeserializeObject<CupisDatabase>(CommonStaticFunction.InflateToString(Convert.FromBase64String(appExchange.DataStr)));
			ViewModelMainWindow.CupisDatabaseWindow.SetCupisDatabase(data.Clients.ToList());
		}
	}

	private static async Task<SyncResponse> OnSyncRequestReceivedAsync(SyncRequest req)
	{
		AppExchange appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(req.Data));
		if (appExchange.TypeData == TypesData.AccountCookie)
		{
			Guid guidAccount = Guid.Parse(appExchange.DataStr);
			AccountSettings accountSettings = ViewModelMainWindow.instance.AccountsSettingsVM.SingleOrDefault((AccountSettingsVM acVM) => acVM.AccountSettings.GUID.ToString() == guidAccount.ToString()).AccountSettings;
			string cookie = "";
			if (accountSettings != null)
			{
				cookie = ViewModelMainWindow.GetAccountCookie(accountSettings.GUID);
			}
			appExchange.DataStr = cookie;
			return new SyncResponse(req, appExchange.Serialize());
		}
		return new SyncResponse(req, "Hello back at you!");
	}

	public static void SendChanges()
	{
		try
		{
			SendMessage(new AppExchange
			{
				TypeProject = TypesProject.AccountsManager,
				TypeData = TypesData.UpdateAccounts,
				DataStr = JsonConvert.SerializeObject(ChangesBL.SessionChanges)
			}.Serialize());
		}
		catch
		{
		}
	}

	public static async Task<bool> IsAccountExistSyncRequestAsync(string login, string bcName, string guid)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.AccountsCenter;
		appExchange.TypeData = TypesData.IsAccountExist;
		appExchange.DataStr = login + "|" + bcName + "|" + guid;
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize());
		if (syncResponse == null)
		{
			return false;
		}
		appExchange = JsonConvert.DeserializeObject<AppExchange>(Encoding.UTF8.GetString(syncResponse.Data));
		return appExchange.DataStr.Split('|')[1] == "true";
	}

	public static async Task<Proxy> GetAvabilityProxyAccountSyncRequestAsync(string bookkmaker)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeData = TypesData.GetAvabilityProxyAccount;
		appExchange.DataStr = bookkmaker;
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize());
		if (syncResponse == null)
		{
			return null;
		}
		string json = Encoding.UTF8.GetString(syncResponse.Data);
		appExchange = JsonConvert.DeserializeObject<AppExchange>(json);
		_ = appExchange.DataStr;
		return AccountsLibrary.ProxyBL.ListProxy.FirstOrDefault((Proxy pr) => pr.GUID == Guid.Parse(appExchange.DataStr));
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

	public static async Task<List<AccountSettings>> SearchAccountsRequestAsync(string filter)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.AccountsCenter;
		appExchange.TypeData = TypesData.AccountsSearch;
		appExchange.DataStr = filter;
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
		return JsonConvert.DeserializeObject<List<AccountSettings>>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static void DatabaseRequest()
	{
		SendMessage(JsonConvert.SerializeObject(new AppExchange
		{
			TypeProject = TypesProject.AccountsCenter,
			TypeData = TypesData.GetCupisDataBase,
			DataStr = "GetCupisDatabase"
		}));
	}

	public static void UpdateOwners()
	{
		SendMessage(new AppExchange
		{
			TypeData = TypesData.UpdateOwners,
			DataStr = JsonConvert.SerializeObject(OwnersAccountBL.OwnersAccount)
		}.Serialize());
	}

	public static async Task OwnersRequestAsync()
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.AccountsCenter;
		appExchange.TypeData = TypesData.GetOwners;
		SyncResponse syncResponse = await SendSyncMessage(appExchange.Serialize());
		if (syncResponse != null)
		{
			appExchange = AppExchange.Deserialize(Encoding.UTF8.GetString(syncResponse.Data));
			if (appExchange != null)
			{
				OwnersAccountBL.OwnersAccount = JsonConvert.DeserializeObject<ObservableCollection<OwnerAccount>>(CommonStaticFunction.DecompressString(appExchange.DataStr));
			}
		}
	}

	public static void UpdateProxies()
	{
		SendMessage(new AppExchange
		{
			TypeData = TypesData.UpdateProxy,
			DataStr = JsonConvert.SerializeObject(AccountsLibrary.ProxyBL.ListProxy)
		}.Serialize());
	}

	public static async Task<List<AccountSettings>> SearchRequestAsync(string filter)
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.AccountsCenter;
		appExchange.TypeData = TypesData.AccountsSearch;
		appExchange.DataStr = filter;
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
		return JsonConvert.DeserializeObject<List<AccountSettings>>(CommonStaticFunction.DecompressString(appExchange.DataStr));
	}

	public static void SaveDatabaseRequest()
	{
		SendMessage(JsonConvert.SerializeObject(new AppExchange
		{
			TypeProject = TypesProject.AccountsCenter,
			TypeData = TypesData.SaveDataBaseAccounts
		}));
	}
}
