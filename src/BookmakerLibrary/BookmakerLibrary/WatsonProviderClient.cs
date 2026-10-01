using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary;
using Newtonsoft.Json;
using WatsonTcp;

namespace BookmakerLibrary;

public class WatsonProviderClient
{
	private static string host = "localhost";

	private const int port = 9001;

	private DateTime LastTimePing = DateTime.MinValue;

	private bool NeedFullSnapshot = true;

	private long Sequence = 0L;

	private Dictionary<long, Match> LastState = new Dictionary<long, Match>();

	private WatsonTcpClient WatsonTcpClient { get; set; }

	private Provider Provider { get; set; }

	private ProtobufBC ProtobufBC { get; set; }

	public void Setup(Provider initProvider)
	{
		Provider = initProvider;
		ProtobufBC = new ProtobufBC(Provider);
		if (CommonStaticFunction.IsMainServer)
		{
			host = "192.168.0.1";
		}
		WatsonTcpClient = new WatsonTcpClient(host, 9001);
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
		NeedFullSnapshot = true;
	}

	private void ServerDisconnected(object sender, DisconnectionEventArgs e)
	{
		Thread.Sleep(5000);
		NeedFullSnapshot = true;
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
				Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} [AppExchangeClient.Connect] Try connect to {host}:{9001}...");
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

	public void UploadData()
	{
		if (WatsonTcpClient == null || !WatsonTcpClient.Connected)
		{
			return;
		}
		DateTime now = DateTime.Now;
		byte[] array = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			for (int i = 0; i < 3; i++)
			{
				try
				{
					ProtobufBC.Serializer.Serialize((Stream)memoryStream, Provider, (object)null);
					array = memoryStream.ToArray();
				}
				catch (Exception ex)
				{
					Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [" + Provider.Name + ".UploadData] Exception: " + ex.Message);
					Thread.Sleep(10);
					continue;
				}
				break;
			}
		}
		if (array == null)
		{
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [WatsonProviderClient] Serialization failed in 3 attempts. Upload skiped");
			return;
		}
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.Provider;
		appExchange.TypeData = TypesData.ProviderMatches;
		appExchange.DataByte = array;
		string message = JsonConvert.SerializeObject(appExchange);
		SendMessage(message);
		int count = Provider.Matches.Count;
		int num = Provider.Matches.SelectMany((Match m) => m.Coupons).Count();
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

	private void PushState()
	{
		AppExchange appExchange = new AppExchange();
		appExchange.TypeProject = TypesProject.Provider;
		appExchange.TypeData = TypesData.ProviderMatchesDelta;
		List<Match> matches = Provider.Matches;
		if (NeedFullSnapshot)
		{
			SyncMessage value = new SyncMessage
			{
				NameProvider = Provider.Name,
				Mode = SyncMode.FullSnapshot,
				Sequence = ++Sequence,
				SentAtUtc = DateTime.Now,
				FullData = matches
			};
			appExchange.DataStr = JsonConvert.SerializeObject(value);
			string message = JsonConvert.SerializeObject(appExchange);
			SendMessage(message);
			LastState = matches.ToDictionary((Match x) => x.IdMatch, (Match x) => x.Clone());
			NeedFullSnapshot = false;
			return;
		}
		List<MatchDeltaMessage> list = BuildDeltas(matches);
		if (list.Count != 0)
		{
			SyncMessage value2 = new SyncMessage
			{
				NameProvider = Provider.Name,
				Mode = SyncMode.Delta,
				Sequence = ++Sequence,
				SentAtUtc = DateTime.UtcNow,
				Deltas = list
			};
			appExchange = new AppExchange();
			appExchange.TypeProject = TypesProject.Provider;
			appExchange.TypeData = TypesData.ProviderMatchesDelta;
			appExchange.DataStr = JsonConvert.SerializeObject(value2);
			string message = JsonConvert.SerializeObject(appExchange);
			SendMessage(message);
			LastState = matches.ToDictionary((Match x) => x.IdMatch, (Match x) => x.Clone());
		}
	}

	private List<MatchDeltaMessage> BuildDeltas(List<Match> currentMatches)
	{
		List<MatchDeltaMessage> list = new List<MatchDeltaMessage>();
		foreach (Match currentMatch in currentMatches)
		{
			if (!LastState.TryGetValue(currentMatch.IdMatch, out var value))
			{
				list.Add(new MatchDeltaMessage
				{
					IdMatch = currentMatch.IdMatch.ToString(),
					IsNew = true,
					Changes = new Dictionary<string, object>
					{
						["Team1"] = currentMatch.Team1,
						["Team2"] = currentMatch.Team2,
						["Score"] = currentMatch.Score.StrScore,
						["Coupons"] = JsonConvert.SerializeObject(currentMatch.Coupons)
					}
				});
			}
			else
			{
				MatchDeltaMessage deltaMatch = GetDeltaMatch(currentMatch, value);
				if (deltaMatch.Changes.Count > 0)
				{
					list.Add(deltaMatch);
				}
			}
		}
		return list;
	}

	private MatchDeltaMessage GetDeltaMatch(Match current, Match previous)
	{
		MatchDeltaMessage matchDeltaMessage = new MatchDeltaMessage
		{
			IdMatch = current.IdMatch.ToString()
		};
		if (current.Team1 != previous.Team1)
		{
			matchDeltaMessage.Changes["Team1"] = current.Team1;
		}
		if (current.Team2 != previous.Team2)
		{
			matchDeltaMessage.Changes["Team2"] = current.Team2;
		}
		if (current.Score.StrScore != previous.Score.StrScore)
		{
			matchDeltaMessage.Changes["Score"] = current.Score.StrScore;
		}
		if (!AreOutcomesEqual(current.Coupons, previous.Coupons))
		{
			matchDeltaMessage.Changes["Coupons"] = JsonConvert.SerializeObject(current.Coupons);
		}
		return matchDeltaMessage;
	}

	private bool AreOutcomesEqual(List<Coupon> coupons1, List<Coupon> coupons2)
	{
		if (coupons1.Count != coupons2.Count)
		{
			return true;
		}
		foreach (Coupon coupon1 in coupons1)
		{
			if (coupons2.SingleOrDefault((Coupon c) => c.Availability == coupon1.Availability && c.Period == coupon1.Period && c.IssueType == coupon1.IssueType && c.IssueValue == coupon1.IssueValue && c.GameTennis == coupon1.GameTennis && c.PayoutRatio == coupon1.PayoutRatio) == null)
			{
				return true;
			}
		}
		return true;
	}
}
