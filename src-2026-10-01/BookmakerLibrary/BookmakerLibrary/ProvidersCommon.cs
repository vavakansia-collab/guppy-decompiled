using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BookmakerLibrary;

public static class ProvidersCommon
{
	public static object thislock;

	public static object lockNewProvider;

	public static OnStrategy1Delegate OnStrategy1;

	public static List<string> ProvidersNames { get; set; }

	static ProvidersCommon()
	{
		thislock = new object();
		lockNewProvider = new object();
		ProvidersNames = new List<string>();
		ProvidersNames.Add("_1Win");
		ProvidersNames.Add("_1xbet");
		ProvidersNames.Add("Marathon");
		ProvidersNames.Add("MarathonRU");
		ProvidersNames.Add("BaltbetRU");
		ProvidersNames.Add("BetBoom");
		ProvidersNames.Add("Betcity");
		ProvidersNames.Add("BetcityLine");
		ProvidersNames.Add("BeterCo");
		ProvidersNames.Add("Parimatch");
		ProvidersNames.Add("Tennisi");
		ProvidersNames.Add("FonbetRU");
		ProvidersNames.Add("FonbetRULine");
		ProvidersNames.Add("LigaStavokRU");
		ProvidersNames.Add("OlimpBet");
		ProvidersNames.Add("SetkaCupProvider");
		ProvidersNames.Add("Winline");
		ProvidersNames.Add("WinlineLine");
		ProvidersNames.Add("LeonRU");
		ProvidersNames.Add("ZenitWin");
		ProvidersNames.Sort();
	}

	public static Provider UpdateDataProvider(Provider provider, Provider newProvider)
	{
		DateTime now = DateTime.Now;
		lock (provider.lockProvider)
		{
			provider.NumberUpdate = newProvider.NumberUpdate;
			provider.LastTimeUpdate = DateTime.Now;
			foreach (Match match2 in newProvider.Matches)
			{
				Match match = GetMatchProvider(match2.IdMatch, provider);
				if (match == null)
				{
					match = new Match(match2.NameProvider);
					match.TimeCreate = match2.TimeCreate;
					match.IdMatch = match2.IdMatch;
					match.IdSport = match2.IdSport;
					match.PeriodDurationMinutes = match2.PeriodDurationMinutes;
					provider.Matches.Add(match);
				}
				if ((!string.IsNullOrEmpty(match.Team1) && match.Team1 != match2.Team1) || (!string.IsNullOrEmpty(match.Team2) && match.Team2 != match2.Team2))
				{
					try
					{
						match.Coupons.Clear();
						provider.Matches.Remove(match);
					}
					catch
					{
					}
					continue;
				}
				match.Team1 = match2.Team1;
				match.Team2 = match2.Team2;
				match.Team1FirstName = match2.Team1FirstName;
				match.Team1LastName = match2.Team1LastName;
				match.Team2FirstName = match2.Team2FirstName;
				match.Team2LastName = match2.Team2LastName;
				match.IdChamp = match2.IdChamp;
				match.LastTimeUpdate = match2.LastTimeUpdate;
				match.NameChamp = match2.NameChamp;
				match.FinishTime = match2.FinishTime;
				match.TypeMatch = match2.TypeMatch;
				match.Url = match2.Url;
				match.ElapsedTime = match2.ElapsedTime;
				string strScore = match.Score.StrScore;
				match.Score.IsScoreUpdated = match.Score.StrScore != match2.DecomposedScore(match2.Score.StrScore);
				foreach (PointScore newPointScore in match2.Score.PointsScore)
				{
					if (match.Score.PointsScore.SingleOrDefault((PointScore ps) => ps.GUID.ToString() == newPointScore.GUID.ToString()) == null)
					{
						match.Score.PointsScore.Add(newPointScore);
					}
				}
				if (match.Score.IsScoreUpdated)
				{
					match.Score.StrScore = match2.Score.StrScore;
					match.Score.Server = match2.Score.Server;
					match.UpdatePointsScore(match2.Score.StrScore, match2.Score.LastTimeUpdate);
					match.Score.LastTimeUpdate = match2.Score.LastTimeUpdate;
				}
				UpdateCoupons(match, match2.Coupons);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> parameter in match2.Parameters)
				{
					dictionary.Add(parameter.Key, parameter.Value);
				}
				match.Parameters = dictionary;
			}
			List<Match> list = new List<Match>();
			foreach (Match match3 in provider.Matches)
			{
				Match matchProvider = GetMatchProvider(match3.IdMatch, newProvider);
				if (matchProvider == null)
				{
					match3.FinishTime = DateTime.Now;
					if (!(DateTime.Now.Subtract(match3.LastTimeUpdate).TotalMinutes < 10.0))
					{
						list.Add(match3);
					}
				}
			}
			foreach (Match item in list)
			{
				provider.Matches.Remove(item);
			}
		}
		return provider;
	}

	public static Match GetMatchProvider(List<Provider> providers, string nameProvider, long idMatch)
	{
		lock (lockNewProvider)
		{
			Provider provider = GetProvider(nameProvider, providers);
			if (provider != null)
			{
				lock (provider.lockProvider)
				{
					return GetMatchProvider(idMatch, provider);
				}
			}
			return null;
		}
	}

	public static Provider GetProvider(string nameProvider, List<Provider> providers)
	{
		Provider provider = null;
		foreach (Provider provider2 in providers)
		{
			if (provider2.Name == nameProvider)
			{
				return provider2;
			}
		}
		if (provider == null)
		{
			provider = new Provider(nameProvider);
			lock (lockNewProvider)
			{
				providers.Add(provider);
			}
		}
		return provider;
	}

	public static List<Provider> GetCopyProviders(List<Provider> providers, string nameBC, int idSport, bool includeScoreHistoty = true, bool includeCoupons = true)
	{
		List<Provider> list = new List<Provider>();
		lock (lockNewProvider)
		{
			foreach (Provider provider in providers)
			{
				if (string.IsNullOrEmpty(nameBC) || !(provider.Name != nameBC))
				{
					Provider copyProvider = GetCopyProvider(provider, idSport, includeScoreHistoty, includeCoupons);
					list.Add(copyProvider);
				}
			}
		}
		return list;
	}

	public static Provider GetCopyProvider(Provider provider, int idSport, bool includeScoreHistoty = true, bool includeCoupons = true)
	{
		Provider provider2 = new Provider(provider.Name);
		if (provider != null)
		{
			provider2.NumberUpdate = provider.NumberUpdate;
			provider2.LastTimeUpdate = provider.LastTimeUpdate;
			lock (provider.lockProvider)
			{
				foreach (Match match in provider.Matches)
				{
					if (idSport == 0 || match.IdSport == idSport)
					{
						Match copyMatch = GetCopyMatch(match, includeScoreHistoty, includeCoupons);
						provider2.Matches.Add(copyMatch);
					}
				}
			}
		}
		return provider2;
	}

	public static ProviderUserCompareDto GetCopyProviderUserCompareDto(Provider provider, int idSport)
	{
		ProviderUserCompareDto providerUserCompareDto = new ProviderUserCompareDto(provider.Name);
		if (provider != null)
		{
			providerUserCompareDto.LastTimeUpdate = provider.LastTimeUpdate;
			lock (provider.lockProvider)
			{
				foreach (Match match in provider.Matches)
				{
					if (idSport == 0 || match.IdSport == idSport)
					{
						MatchUserCompareDto copyMatchUserCompareDto = GetCopyMatchUserCompareDto(match);
						providerUserCompareDto.MatchesDto.Add(copyMatchUserCompareDto);
					}
				}
			}
		}
		return providerUserCompareDto;
	}

	public static Provider GetCopyProvider(List<Provider> providers, string nameProvider, int idSport, bool includeScoreHistoty = true, bool includeCoupons = true)
	{
		Provider provider = new Provider(nameProvider);
		Provider provider2 = GetProvider(nameProvider, providers);
		if (provider2 != null)
		{
			provider.NumberUpdate = provider2.NumberUpdate;
			provider.LastTimeUpdate = provider2.LastTimeUpdate;
			lock (provider2.lockProvider)
			{
				foreach (Match match in provider2.Matches)
				{
					if (idSport == 0 || match.IdSport == idSport)
					{
						Match copyMatch = GetCopyMatch(match, includeScoreHistoty, includeCoupons);
						provider.Matches.Add(copyMatch);
					}
				}
			}
		}
		return provider;
	}

	public static Match GetCopyMatch(Match match, bool includeScoreHistoty = true, bool includeCoupons = true)
	{
		Match match2 = new Match(match.NameProvider);
		match2.TimeCreate = match.TimeCreate;
		match2.IdMatch = match.IdMatch;
		match2.IdSport = match.IdSport;
		match2.Team1 = match.Team1;
		match2.Team2 = match.Team2;
		match2.Team1FirstName = match.Team1FirstName;
		match2.Team1LastName = match.Team1LastName;
		match2.Team2FirstName = match.Team2FirstName;
		match2.Team2LastName = match.Team2LastName;
		match2.IdChamp = match.IdChamp;
		match2.LastTimeUpdate = match.LastTimeUpdate;
		match2.NameChamp = match.NameChamp;
		match2.FinishTime = match.FinishTime;
		match2.TypeMatch = match.TypeMatch;
		match2.Url = match.Url;
		match2.ElapsedTime = match.ElapsedTime;
		match2.PeriodDurationMinutes = match.PeriodDurationMinutes;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> parameter in match.Parameters)
		{
			dictionary.Add(parameter.Key, parameter.Value);
		}
		match2.Parameters = dictionary;
		List<Coupon> list = new List<Coupon>();
		if (includeCoupons)
		{
			foreach (Coupon coupon2 in match.Coupons)
			{
				Coupon coupon = new Coupon();
				coupon.Availability = coupon2.Availability;
				coupon.Comment = coupon2.Comment;
				coupon.GameTennis = coupon2.GameTennis;
				coupon.IssueType = IssueTypeBL.GetIssueTypeByStr(coupon2.IssueType.ToString());
				coupon.IssueValue = coupon2.IssueValue;
				coupon.LastTimeUpdate = coupon2.LastTimeUpdate;
				coupon.PayoutRatio = coupon2.PayoutRatio;
				coupon.Period = coupon2.Period;
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> parameter2 in coupon2.Parameters)
				{
					dictionary2.Add(parameter2.Key, parameter2.Value);
				}
				coupon.Parameters = dictionary2;
				list.Add(coupon);
			}
		}
		match2.Coupons = list;
		match2.Score.IsScoreUpdated = match.Score.IsScoreUpdated;
		match2.Score.StrScore = match.Score.StrScore;
		match2.Score.Server = match.Score.Server;
		match2.Score.LastTimeUpdate = match.Score.LastTimeUpdate;
		if (includeScoreHistoty)
		{
			match2.Score.PointsScore.Clear();
			foreach (PointScore item in match.Score.PointsScore)
			{
				PointScore pointScore = new PointScore();
				pointScore.GUID = item.GUID;
				pointScore.IsDefault = item.IsDefault;
				pointScore.IsScoreDown = item.IsScoreDown;
				pointScore.NumberGetTeam = item.NumberGetTeam;
				pointScore.Period = item.Period;
				pointScore.TimeCreate = item.TimeCreate;
				pointScore.TimeServerLoad = item.TimeServerLoad;
				pointScore.ValuePointTeam1 = item.ValuePointTeam1;
				pointScore.ValuePointTeam2 = item.ValuePointTeam2;
				pointScore.IsAutoPasteScore = item.IsAutoPasteScore;
				match2.Score.PointsScore.Add(pointScore);
				foreach (PointSubScore item2 in item.PointsSubScore)
				{
					PointSubScore pointSubScore = new PointSubScore();
					pointSubScore.GUID = item2.GUID;
					pointSubScore.IsDefault = item2.IsDefault;
					pointSubScore.IsScoreDown = item2.IsScoreDown;
					pointSubScore.TimeCreate = item2.TimeCreate;
					pointSubScore.NumberGetTeam = item2.NumberGetTeam;
					pointSubScore.NumberPointGame = item2.NumberPointGame;
					pointSubScore.ValueSubScoreTeam1 = item2.ValueSubScoreTeam1;
					pointSubScore.ValueSubScoreTeam2 = item2.ValueSubScoreTeam2;
					pointScore.PointsSubScore.Add(pointSubScore);
				}
			}
		}
		return match2;
	}

	public static MatchUserCompareDto GetCopyMatchUserCompareDto(Match match)
	{
		Match copyMatch = GetCopyMatch(match, includeScoreHistoty: false);
		MatchUserCompareDto matchUserCompareDto = new MatchUserCompareDto(copyMatch);
		matchUserCompareDto.TotalCoupons = copyMatch.Coupons.Count;
		matchUserCompareDto.TotalCouponsUnAvailability = copyMatch.Coupons.Count((Coupon c) => !c.Availability);
		return matchUserCompareDto;
	}

	public static void UpdateCoupons(Match match, List<Coupon> newCoupons)
	{
		List<Coupon> list = new List<Coupon>();
		foreach (Coupon newCoupon in newCoupons)
		{
			Coupon coupon = null;
			foreach (Coupon coupon2 in match.Coupons)
			{
				if (coupon2.IssueType == newCoupon.IssueType && coupon2.IssueValue == newCoupon.IssueValue && coupon2.GameTennis == newCoupon.GameTennis && coupon2.Period == newCoupon.Period)
				{
					coupon = coupon2;
					break;
				}
			}
			if (coupon != null && coupon.Availability && coupon.IssueType.ToString().Contains("VictoryTeam_") && coupon.PayoutRatio > 1.01 && (match.IdSport == 6 || match.IdSport == 10) && coupon.Period > match.Period && coupon.PayoutRatio <= 2.5 && Math.Abs(newCoupon.PayoutRatio - coupon.PayoutRatio) >= 0.5 && !match.NameChamp.Contains("CTT 21") && DateTime.Now.Subtract(match.LastTimeUpdate).TotalSeconds < 60.0 && !match.NameProvider.Contains("Marathon") && !IsFinishPeriod(match.Period, match))
			{
				OnStrategy1?.Invoke(match, coupon, newCoupon);
			}
			if (coupon == null)
			{
				coupon = new Coupon
				{
					LastTimeUpdate = newCoupon.LastTimeUpdate
				};
				match.Coupons.Add(coupon);
			}
			if (coupon.PayoutRatio != newCoupon.PayoutRatio || coupon.Availability != newCoupon.Availability)
			{
				coupon.LastTimeUpdate = newCoupon.LastTimeUpdate;
			}
			coupon.Period = newCoupon.Period;
			coupon.IssueType = newCoupon.IssueType;
			coupon.IssueValue = newCoupon.IssueValue;
			coupon.GameTennis = newCoupon.GameTennis;
			coupon.Availability = newCoupon.Availability;
			coupon.PayoutRatio = newCoupon.PayoutRatio;
			coupon.Parameters = newCoupon.Parameters;
			coupon.Comment = newCoupon.Comment;
			list.Add(coupon);
		}
		List<Coupon> listRemoveCoupons = new List<Coupon>();
		foreach (Coupon coupon3 in match.Coupons)
		{
			bool flag = false;
			foreach (Coupon item in list)
			{
				if (item.Period == coupon3.Period && item.IssueType == coupon3.IssueType && item.IssueValue == coupon3.IssueValue && item.GameTennis == coupon3.GameTennis)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				listRemoveCoupons.Add(coupon3);
			}
		}
		match.Coupons.RemoveAll((Coupon c) => listRemoveCoupons.Contains(c));
	}

	public static Match GetMatchProvider(long idMatch, Provider provider)
	{
		foreach (Match match in provider.Matches)
		{
			if (match.IdMatch == idMatch)
			{
				return match;
			}
		}
		return null;
	}

	public static bool IsFinishPeriod(int period, Match match)
	{
		if (match.Sport.IsSlowTimeGoal)
		{
			return false;
		}
		PointScore lastPointScore = match.Score.GetLastPointScore(period);
		ArrayList maxTotalTeamPointPeriod = GetMaxTotalTeamPointPeriod(match, period);
		int num = (int)maxTotalTeamPointPeriod[0];
		int num2 = (int)maxTotalTeamPointPeriod[1];
		if (Math.Max(lastPointScore.ValuePointTeam1, lastPointScore.ValuePointTeam2) >= num && Math.Abs(lastPointScore.ValuePointTeam1 - lastPointScore.ValuePointTeam2) >= num2)
		{
			return true;
		}
		return false;
	}

	private static ArrayList GetMaxTotalTeamPointPeriod(Match match, int period)
	{
		int num = 999;
		int num2 = 999;
		if (period <= 0)
		{
			return new ArrayList { num, num2 };
		}
		if (match.Sport.IdSport == 6)
		{
			if (period < 5)
			{
				return new ArrayList { 25, 2 };
			}
			if (period == 5)
			{
				return new ArrayList { 15, 2 };
			}
		}
		else if (match.Sport.IdSport == 29)
		{
			if (period < 3)
			{
				return new ArrayList { 21, 2 };
			}
			if (period == 3)
			{
				return new ArrayList { 15, 2 };
			}
		}
		else if (match.Sport.IdSport == 10)
		{
			if (period < 1)
			{
				return new ArrayList { 999, 999 };
			}
			if (!match.NameChamp.Contains("до 7") && !match.NameChamp.Contains("до 15"))
			{
				return new ArrayList { 11, 2 };
			}
		}
		else if (match.Sport.IdSport == 16 && !match.NameChamp.Contains("до 11") && !match.NameChamp.Contains("до 15"))
		{
			return new ArrayList { 21, 2 };
		}
		return new ArrayList { num, num2 };
	}
}
