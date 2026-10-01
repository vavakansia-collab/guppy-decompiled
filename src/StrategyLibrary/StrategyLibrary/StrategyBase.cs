using System;
using System.Collections.Generic;
using BookmakerLibrary;

namespace StrategyLibrary;

public class StrategyBase
{
	private double differenceValue;

	private double timeLife;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public long NumberSDP { get; set; }

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public string Name { get; set; }

	public Coupon Coupon { get; set; }

	public string Match1NameProvider { get; set; }

	public long Match1Id { get; set; }

	public string Match1Championat { get; set; }

	public string Match1Team1 { get; set; }

	public string Match1Team2 { get; set; }

	public string Match1Score { get; set; }

	public bool Match1IsOverturn { get; set; }

	public TimeSpan Match1ElapsedTime { get; set; }

	public bool IsCyber { get; set; }

	public bool IsShort { get; set; }

	public int IdSport { get; set; }

	public string Match2NameProvider { get; set; }

	public long Match2Id { get; set; }

	public string Match2Championat { get; set; }

	public string Match2Team1 { get; set; }

	public string Match2Team2 { get; set; }

	public string Match2Score { get; set; }

	public bool Match2IsOverturn { get; set; }

	public TimeSpan Match2ElapsedTime { get; set; }

	public bool IsTwoHalfPeriod { get; set; }

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

	public string Discription { get; set; } = "";

	public string Comment { get; set; } = "";

	public DateTime TimeLastScore { get; set; }

	public int DifferencePoints { get; set; }

	public double DifferenceValue
	{
		get
		{
			return differenceValue;
		}
		set
		{
			differenceValue = Math.Abs(value);
		}
	}

	public double TimeLife
	{
		get
		{
			return timeLife;
		}
		set
		{
			timeLife = Math.Round(value, 2);
		}
	}

	public int IdCompareMatch { get; set; }
}
