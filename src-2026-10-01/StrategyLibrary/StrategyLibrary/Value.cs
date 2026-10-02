using System;
using BookmakerLibrary;

namespace StrategyLibrary;

public class Value : StrategyBase
{
	public Coupon Coupon2 { get; set; }

	public double Percentage { get; set; }

	public int DifferencePointsTeams { get; set; }

	public double PercentVilka { get; set; }

	public Value()
	{
	}

	public Value(Coupon initCoupon1, Match match1, Coupon initCoupon2, Match match2, int initIdCompareMatch, bool initIsOverturn1, bool initIsOverturn2, int initDifferencePoints, double initDifferenceValue, double initPercentage, bool initIsTwoHalfPeriod, double initPercentVilka, long initNumberSDP)
	{
		base.NumberSDP = initNumberSDP;
		base.Coupon = initCoupon1;
		base.IdSport = match1.IdSport;
		base.IsCyber = match1.TypeMatch == TypesMatch.Cyber;
		base.IsShort = match1.TypeMatch == TypesMatch.Short;
		base.Match1NameProvider = match1.NameProvider;
		base.Match1Id = match1.IdMatch;
		base.Match1Championat = match1.NameChamp;
		base.Match1Team1 = match1.Team1;
		base.Match1Team2 = match1.Team2;
		base.Match1Score = match1.Score.StrScore;
		base.Match1ElapsedTime = match1.ElapsedTime;
		Coupon2 = initCoupon2;
		base.Match2NameProvider = match2.NameProvider;
		base.Match2Id = match2.IdMatch;
		base.Match2Championat = match2.NameChamp;
		base.Match2Team1 = match2.Team1;
		base.Match2Team2 = match2.Team2;
		base.Match2Score = match2.Score.StrScore;
		base.Match2IsOverturn = initIsOverturn2;
		base.Match2ElapsedTime = match2.ElapsedTime;
		base.IdCompareMatch = initIdCompareMatch;
		base.Match1IsOverturn = initIsOverturn1;
		base.DifferencePoints = initDifferencePoints;
		base.DifferenceValue = initDifferenceValue;
		Percentage = initPercentage;
		PointScore lastPointScoreMatch2 = match2.Score.GetLastPointScore(base.Coupon.Period);
		base.TimeLastScore = match2.Score.LastTimeUpdate;
		base.IsTwoHalfPeriod = initIsTwoHalfPeriod;
		DifferencePointsTeams = Math.Abs(lastPointScoreMatch2.ValuePointTeam1 - lastPointScoreMatch2.ValuePointTeam2);
		PercentVilka = initPercentVilka;
	}
}
