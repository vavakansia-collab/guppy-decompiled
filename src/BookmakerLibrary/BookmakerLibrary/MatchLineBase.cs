using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class MatchLineBase
{
	public string NameProvider;

	public List<CouponBase> Coupons = new List<CouponBase>();

	public int IdSport;

	public long IdMatch;

	public int IdChamp;

	public string NameChamp;

	public string Team1;

	public string Team2;

	public DateTime TimeCreate = DateTime.Now;

	public DateTime StartTime = DateTime.MinValue;

	public string Url;

	public Dictionary<string, string> Parameters = new Dictionary<string, string>();

	public MatchLineBase(string initNameProvider)
	{
		NameProvider = initNameProvider;
	}

	public bool IsEquals(MatchLine _match)
	{
		return NameProvider == _match.NameProvider && IdMatch == _match.IdMatch;
	}

	public override string ToString()
	{
		Sport sportByID = SportBL.GetSportByID(IdSport);
		return StartTime.ToString() + " " + NameProvider + ". " + ((!NameChamp.Contains(sportByID.FullName) && !NameChamp.Contains(sportByID.ShortName)) ? (sportByID.FullName + ". ") : "") + NameChamp + " " + Team1 + " - " + Team2;
	}
}
