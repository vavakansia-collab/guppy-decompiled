using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary;
using Newtonsoft.Json;

namespace BookmakerLibrary;

public abstract class BookmakerCompany : Provider
{
	private static object lockLogs = new object();

	private static object lockAddCoupons = new object();

	public static object lockMatches = new object();

	private static readonly Regex regexSearch = new Regex("^[A-Za-z0-9\\._ -]+$");

	public WatsonProviderClient WatsonProviderClient = new WatsonProviderClient();

	public WatsonLineProviderClient WatsonLineProviderClient = new WatsonLineProviderClient();

	public Queue<Proxy> ListProxy = new Queue<Proxy>();

	public string FilterMatchesByName = "";

	public string Host { get; set; }

	public int IntervalUploadData { get; set; } = 1500;

	public int IntervalLineUpdate { get; set; } = 60000;

	public bool UseProxy { get; set; }

	public Proxy Proxy { get; set; } = null;

	public DateTime LastTimeChangeProxy { get; set; }

	public event OnUpdateDataLineHandler OnUpdateDataLine;

	public BookmakerCompany()
	{
	}

	public void Start()
	{
		base.Matches.Clear();
		SetProxies();
		Task.Run(delegate
		{
			string name = Assembly.GetEntryAssembly().GetName().Name;
			if (name != "LineServer")
			{
				WatsonCheckerClient.Setup(base.Name);
				WatsonCheckerClient.Start();
				WatsonProviderClient.Setup(this);
				WatsonProviderClient.Start();
			}
		});
		Thread.Sleep(2000);
		Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " Started " + base.Name);
		GetData();
	}

	public void LoadLine()
	{
		string text = "d:\\guppy.bet\\Line\\";
		if (Directory.Exists(text))
		{
			string text2 = text + base.Name + ".txt";
			if (!File.Exists(text2))
			{
				return;
			}
			DateTime now = DateTime.Now;
			string compressedText = File.ReadAllText(text2);
			string value = CommonStaticFunction.DecompressString(compressedText);
			List<MatchLine> source = JsonConvert.DeserializeObject<List<MatchLine>>(value, CommonStaticFunction.JsonSettings);
			base.MatchesLine.Clear();
			base.MatchesLine.AddRange(source.Where((MatchLine m) => m.StartTime > DateTime.Now.AddDays(-1.0)));
			Console.WriteLine($"{CommonStaticFunction.GetDateTimeStamp()} Загрузка файла {text2} {Math.Round(DateTime.Now.Subtract(now).TotalSeconds, 2)} сек.");
		}
		Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [BookmakerLibrary] " + base.Name + " Load privider data line.");
	}

	public abstract void GetData();

	public void CheckFinishedMatches()
	{
		if (base.Name.Contains("Scout"))
		{
			return;
		}
		foreach (Match item in base.Matches.Where((Match m) => m.FinishTime == DateTime.MinValue))
		{
			if (DateTime.Now.Subtract(item.LastTimeUpdate).TotalMinutes >= 20.0)
			{
				item.FinishTime = DateTime.Now;
			}
		}
		base.Matches.RemoveAll((Match m) => m.FinishTime != DateTime.MinValue && DateTime.Now.Subtract(m.LastTimeUpdate).TotalMinutes >= 3.0);
	}

	public void UploadData(Provider provider = null)
	{
		WatsonProviderClient.UploadData();
		base.NumberUpdate++;
	}

	public void UploadChecker()
	{
		if (!CommonStaticFunction.IsAfalika() && (base.Matches.Count > 0 || base.MatchesLine.Count > 0))
		{
			WatsonCheckerClient.UploadData();
		}
	}

	public void SetProxies()
	{
		Regex regex = new Regex("(?<ip>\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(?<port>\\d{1,5}):(?<username>\\w*):(?<password>\\w*)");
		if (regex.IsMatch(ProxyBL.Proxies))
		{
			foreach (System.Text.RegularExpressions.Match item2 in regex.Matches(ProxyBL.Proxies))
			{
				string value = item2.Groups["ip"].Value;
				int initPort = Convert.ToInt32(item2.Groups["port"].Value);
				string value2 = item2.Groups["username"].Value;
				string value3 = item2.Groups["password"].Value;
				Proxy item = new Proxy(value, initPort, value2, value3);
				ListProxy.Enqueue(item);
			}
		}
		ChangeProxy();
	}

	public void ChangeProxy()
	{
		if (UseProxy)
		{
			Proxy = ListProxy.Dequeue();
			ListProxy.Enqueue(Proxy);
			LastTimeChangeProxy = DateTime.Now;
		}
		else
		{
			Proxy = ListProxy.FirstOrDefault((Proxy pr) => pr.IsEmpty);
		}
	}

	public Match FindAddMatch(int idSport, long idMatch, int idChamp, string nameChamp, string team1, string team2, Dictionary<string, string> parameters = null, bool isLine = false)
	{
		Match match = base.Matches.SingleOrDefault((Match m) => m.IdMatch == idMatch);
		if (match != null)
		{
			return match;
		}
		if (!SportBL.CheckAvailabilitySportById(idSport))
		{
			return null;
		}
		if (string.IsNullOrEmpty(team1) || string.IsNullOrEmpty(team2))
		{
			return null;
		}
		if (!string.IsNullOrEmpty(FilterMatchesByName) && !nameChamp.ToUpper().Contains(FilterMatchesByName.ToUpper()) && !team1.ToUpper().Contains(FilterMatchesByName.ToUpper()) && !team2.ToUpper().Contains(FilterMatchesByName.ToUpper()))
		{
			return null;
		}
		lock (lockMatches)
		{
			Match match2 = new Match(base.Name);
			match2.IdSport = idSport;
			match2.IdMatch = idMatch;
			match2.IdChamp = idChamp;
			match2.NameChamp = nameChamp;
			match2.Team1 = team1;
			match2.Team2 = team2;
			match2.IsLine = isLine;
			match2.PeriodDurationMinutes = match2.Sport.PeriodDurationMinutesDefault;
			match2.OnUpdateMatchHandler += OnUpdateMatchHandler;
			if (parameters != null)
			{
				match2.Parameters = parameters;
			}
			if (IsCyber(match2))
			{
				match2.TypeMatch = TypesMatch.Cyber;
			}
			if (match2.TypeMatch != TypesMatch.Cyber && IsShort(match2))
			{
				match2.TypeMatch = TypesMatch.Short;
			}
			if ((idSport == 1 || idSport == 2) && match2.TypeMatch == TypesMatch.Regular)
			{
				return null;
			}
			if (match2.IdSport == 3 && (match2.NameChamp.Contains("12 мин") || match2.NameChamp.Contains("NBA") || match2.NameChamp.Contains("НБА") || match2.NameChamp.ToUpper().Contains("CDBL")))
			{
				match2.PeriodDurationMinutes = 12;
			}
			match2.AddMessageHandler += Match_AddMessageHandler;
			base.Matches.Add(match2);
			return match2;
		}
	}

	private void OnUpdateMatchHandler(Match match)
	{
		base.LastTimeUpdate = match.LastTimeUpdate;
	}

	private void Match_AddMessageHandler(string text)
	{
		AddMessage(text);
	}

	public void AddCoupon(Match match, int period, IssueType issueType, double issueValue, double payoutRatio, int gameTennis, bool availability, Dictionary<string, string> parameters = null, string comment = "", bool isLife = true)
	{
		lock (lockAddCoupons)
		{
			match.LastTimeUpdate = DateTime.Now;
			if (issueType == IssueType._empty || (isLife && (((issueType.ToString().Contains("OddsTeam_") || issueType.ToString().Contains("Total")) && issueValue - Math.Floor(issueValue) == 0.0) || ((issueType.ToString().Contains("OddsTeam_") || issueType.ToString().Contains("Total")) && !match.Sport.IsSlowTimeGoal && period == 0 && match.IdSport != 3))))
			{
				return;
			}
			Coupon coupon = match.Coupons.SingleOrDefault((Coupon c) => c.Period == period && c.IssueType == issueType && c.IssueValue == issueValue && c.GameTennis == gameTennis);
			Coupon coupon2 = new Coupon
			{
				LastTimeUpdate = DateTime.Now,
				PayoutRatio = payoutRatio,
				IssueType = issueType,
				IssueValue = issueValue,
				Period = period,
				GameTennis = gameTennis,
				Availability = availability,
				Comment = comment
			};
			if (coupon != null)
			{
				match.Coupons.Remove(coupon);
				if (coupon2.PayoutRatio == coupon.PayoutRatio && coupon2.Availability == coupon2.Availability)
				{
					coupon2.LastTimeUpdate = coupon.LastTimeUpdate;
				}
			}
			if (parameters != null)
			{
				coupon2.Parameters = parameters;
			}
			if (base.Name == "_1xbet")
			{
				if ((coupon2.IssueType == IssueType.PointP1 || coupon2.IssueType == IssueType.PointP2) && (match.Sport.IdSport == 4 || match.Sport.IdSport == 6 || match.Sport.IdSport == 10 || match.Sport.IdSport == 16 || match.Sport.IdSport == 29))
				{
					string text = issueValue.ToString();
					if (match.Sport.IdSport == 4)
					{
						coupon2.GameTennis = Convert.ToInt32(text.Remove(0, text.IndexOf(",") + 1));
					}
					if (text.IndexOf(",") > 0)
					{
						coupon2.IssueValue = Convert.ToDouble(text.Remove(text.IndexOf(",")));
					}
				}
				if (issueType.ToString().Contains("TennisGame"))
				{
					coupon2.GameTennis = (int)coupon2.IssueValue;
					coupon2.IssueValue = 0.0;
				}
			}
			match.Coupons.Add(coupon2);
			base.LastTimeUpdate = DateTime.Now;
		}
	}

	public void AddMessage(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		lock (lockLogs)
		{
			Console.WriteLine(DateTime.Now.ToString() + " " + text);
		}
	}

	public static bool IsCyber(Match match)
	{
		string text = Regex.Match(match.Team1, "\\((.*?)\\)").Value.Replace("(", "").Replace(")", "").Trim();
		string text2 = Regex.Match(match.Team2, "\\((.*?)\\)").Value.Replace("(", "").Replace(")", "").Trim();
		if (match.NameProvider == "LigaStavokRU")
		{
			text = text.Replace(" ", "-");
			text2 = text2.Replace(" ", "-");
		}
		if ((match.NameChamp.ToLower().Contains("кибер") || match.NameChamp.ToUpper().Contains("NHL 2") || match.NameChamp.ToUpper().Contains("NBA 2K") || match.NameChamp.ToLower().Contains("esports") || match.NameChamp.ToLower().Contains("fifa") || match.NameChamp.ToLower().Contains("cyber") || match.NameChamp.ToUpper().Contains("LEAGUE") || match.NameChamp.ToUpper().Contains("BATTLE") || (match.NameChamp.ToLower().Contains("лига про") && match.IdSport == 2) || match.NameChamp.ToUpper().Contains("H2H") || match.NameChamp.Contains("E-Basketball") || match.NameChamp.Contains("Кибер NBA") || match.NameChamp.Contains("ESB") || match.NameChamp.Contains("ESportsBattle") || match.NameChamp.Contains("H2H") || match.NameChamp.ToUpper().Contains("eFootball".ToUpper()) || match.NameChamp.ToUpper().Contains("FC 26")) && match.Team1.Contains("(") && match.Team1.Contains(")") && match.Team2.Contains("(") && match.Team2.Contains(")") && regexSearch.IsMatch(text) && regexSearch.IsMatch(text2))
		{
			return true;
		}
		return false;
	}

	public static bool IsShort(Match match)
	{
		if (match.NameChamp.ToLower().Contains("шорт") || match.NameChamp.ToLower().Contains("лига про") || match.NameChamp.ToLower().Contains("short") || match.NameChamp.ToLower().Contains("xhl") || match.NameChamp.Contains("ACL") || match.NameChamp.Contains("7x7") || match.NameChamp.Contains("5x5") || match.NameChamp.Contains("4x4") || match.NameChamp.Contains("3x3") || match.NameChamp.Contains("2x2") || match.NameChamp.Contains("3X3") || match.NameChamp.Contains("4X4") || match.NameChamp.Contains("5X5") || match.NameChamp.Contains("7X7") || match.NameChamp.Contains("MNHL") || match.NameChamp.Contains("Кипербол") || match.NameChamp.Contains("3HL") || match.Team1.ToLower().Contains("(люб)") || match.Team1.ToLower().Contains("любители") || match.Team1.ToLower().Contains("2x2") || match.Team1.ToLower().Contains("3x3") || match.Team1.ToLower().Contains("4x4") || match.Team1.ToLower().Contains("5x5") || match.Team1.ToLower().Contains("7x7"))
		{
			return true;
		}
		return false;
	}

	public virtual void GetFullCouponsMatch(IEnumerable<long> idMatches)
	{
	}

	public void UpdateLineMatches(List<MatchLineBase> _matches)
	{
		lock (lockProvider)
		{
			foreach (MatchLineBase matchBasic in _matches)
			{
				MatchLine matchLine = base.MatchesLine.SingleOrDefault((MatchLine m) => m.IdMatch == matchBasic.IdMatch);
				if (matchLine == null)
				{
					matchLine = new MatchLine(base.Name);
					matchLine.TimeCreate = DateTime.Now;
					matchLine.LastTimeUpdate = DateTime.Now;
					matchLine.StartTime = matchBasic.StartTime;
					matchLine.IdSport = matchBasic.IdSport;
					matchLine.NameChamp = matchBasic.NameChamp;
					matchLine.IdChamp = matchBasic.IdChamp;
					matchLine.IdMatch = matchBasic.IdMatch;
					matchLine.Team1 = matchBasic.Team1;
					matchLine.Team2 = matchBasic.Team2;
					matchLine.Url = matchBasic.Url;
					foreach (KeyValuePair<string, string> parameter in matchBasic.Parameters)
					{
						matchLine.SetParameter(parameter.Key, parameter.Value);
					}
					foreach (CouponBase coupon in matchBasic.Coupons)
					{
						CouponLine couponLine = new CouponLine
						{
							Availability = coupon.Availability,
							Period = coupon.Period,
							IssueType = coupon.IssueType,
							IssueValue = coupon.IssueValue,
							PayoutRatio = coupon.Coef,
							GameTennis = coupon.GameTennis
						};
						foreach (KeyValuePair<string, string> parameter2 in coupon.Parameters)
						{
							couponLine.SetParameter(parameter2.Key, parameter2.Value);
						}
						couponLine.HistoryCoefs.Add(new CouponLine.CoefFrame
						{
							Timestamp = DateTime.Now,
							PayoutRatio = coupon.Coef
						});
						matchLine.CouponsLine.Add(couponLine);
					}
					base.MatchesLine.Add(matchLine);
					continue;
				}
				matchLine.Url = matchBasic.Url;
				matchLine.StartTime = matchBasic.StartTime;
				foreach (CouponBase _coupon in matchBasic.Coupons)
				{
					CouponLine couponLine2 = matchLine.CouponsLine.SingleOrDefault((CouponLine c) => c.Period == _coupon.Period && c.IssueType == _coupon.IssueType && c.IssueValue == _coupon.IssueValue && c.GameTennis == _coupon.GameTennis);
					if (couponLine2 == null)
					{
						couponLine2 = new CouponLine
						{
							Availability = _coupon.Availability,
							Period = _coupon.Period,
							IssueType = _coupon.IssueType,
							IssueValue = _coupon.IssueValue,
							PayoutRatio = _coupon.Coef,
							GameTennis = _coupon.GameTennis
						};
						foreach (KeyValuePair<string, string> parameter3 in _coupon.Parameters)
						{
							couponLine2.SetParameter(parameter3.Key, parameter3.Value);
						}
						couponLine2.HistoryCoefs.Add(new CouponLine.CoefFrame
						{
							Timestamp = DateTime.Now,
							PayoutRatio = _coupon.Coef
						});
						matchLine.CouponsLine.Add(couponLine2);
					}
					else
					{
						if (!(Math.Abs(couponLine2.PayoutRatio - _coupon.Coef) > 0.1))
						{
							continue;
						}
						couponLine2.LastTimeUpdate = DateTime.Now;
						couponLine2.Availability = _coupon.Availability;
						couponLine2.PayoutRatio = _coupon.Coef;
						couponLine2.HistoryCoefs.Add(new CouponLine.CoefFrame
						{
							Timestamp = DateTime.Now,
							PayoutRatio = _coupon.Coef
						});
						couponLine2.Parameters.Clear();
						foreach (KeyValuePair<string, string> parameter4 in _coupon.Parameters)
						{
							couponLine2.SetParameter(parameter4.Key, parameter4.Value);
						}
					}
				}
				foreach (CouponLine couponLine3 in matchLine.CouponsLine)
				{
					CouponBase couponBase = matchBasic.Coupons.SingleOrDefault((CouponBase c) => c.Period == couponLine3.Period && c.IssueType == couponLine3.IssueType && c.IssueValue == couponLine3.IssueValue && c.GameTennis == couponLine3.GameTennis);
					if (couponBase == null && couponLine3.PayoutRatio != -1.0)
					{
						couponLine3.PayoutRatio = -1.0;
						couponLine3.HistoryCoefs.Add(new CouponLine.CoefFrame
						{
							Timestamp = DateTime.Now,
							PayoutRatio = -1.0
						});
					}
				}
			}
			base.LastTimeUpdate = DateTime.Now;
		}
		this.OnUpdateDataLine?.Invoke(this);
	}
}
