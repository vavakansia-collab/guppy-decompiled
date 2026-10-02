using BookmakerLibrary;

namespace StrategyLibrary;

public class ValueScore : StrategyBase
{
	public ValueScore()
	{
	}

	public ValueScore(Coupon coupon, Match match1, Match match2, int initIdCompareMatch, bool initIsOverturn1, bool initIsOverturn2, int initDifferencePoints, bool initIsTwoHalfPeriod, long initNumberSDP)
	{
		base.NumberSDP = initNumberSDP;
		base.Coupon = coupon;
		base.IdSport = match1.IdSport;
		base.IsCyber = match1.TypeMatch == TypesMatch.Cyber;
		base.IsShort = match1.TypeMatch == TypesMatch.Short;
		base.Match1NameProvider = match1.NameProvider;
		base.Match1Id = match1.IdMatch;
		base.Match1Championat = match1.NameChamp;
		base.Match1Team1 = match1.Team1;
		base.Match1Team2 = match1.Team2;
		base.Match1Score = match1.Score.StrScore;
		base.Match1IsOverturn = initIsOverturn1;
		base.Match1ElapsedTime = match1.ElapsedTime;
		base.Match2NameProvider = match2.NameProvider;
		base.Match2Id = match2.IdMatch;
		base.Match2Championat = match2.NameChamp;
		base.Match2Team1 = match2.Team1;
		base.Match2Team2 = match2.Team2;
		base.Match2Score = match2.Score.StrScore;
		base.Match2IsOverturn = initIsOverturn2;
		base.Match2ElapsedTime = match2.ElapsedTime;
		base.IdCompareMatch = initIdCompareMatch;
		base.DifferencePoints = initDifferencePoints;
		base.TimeLastScore = match2.Score.LastTimeUpdate;
		base.IsTwoHalfPeriod = initIsTwoHalfPeriod;
	}
}
