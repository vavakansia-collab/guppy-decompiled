using System;
using BookmakerLibrary;

namespace StrategyLibrary;

public class Fork : StrategyBase
{
	public Coupon Coupon2 { get; set; }

	public int DifferencePointsTeams { get; set; }

	public double ProfitPercentage { get; set; }

	public Fork()
	{
	}

	public Fork(Coupon initCoupon1, Match match1, Coupon initCoupon2, Match match2, int initIdCompareMatch, bool initIsOverturn1, bool initIsOverturn2, int initDifferencePoints, double initDifferenceValue, double initProfit, bool initIsTwoHalfPeriod, long initNumberSDP)
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
		PointScore lastPointScore = match2.Score.GetLastPointScore(base.Coupon.Period);
		base.TimeLastScore = match2.Score.LastTimeUpdate;
		base.IsTwoHalfPeriod = initIsTwoHalfPeriod;
		DifferencePointsTeams = Math.Abs(lastPointScore.ValuePointTeam1 - lastPointScore.ValuePointTeam2);
		ProfitPercentage = initProfit;
	}

	public static double GetProfitPercentage(double coef1, double coef2)
	{
		double num = 1.0 / coef1;
		double num2 = 1.0 / coef2;
		double num3 = num + num2;
		return Math.Round((1.0 / num3 - 1.0) * 100.0, 2);
	}
}
