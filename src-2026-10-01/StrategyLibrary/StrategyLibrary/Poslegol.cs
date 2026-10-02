using BookmakerLibrary;

namespace StrategyLibrary;

public class Poslegol : StrategyBase
{
	public Coupon CouponOverturn { get; set; }

	public Poslegol()
	{
	}

	public Poslegol(Coupon coupon, Match match, Match match2, int initIdCompareMatch, int initDifferencePoints, bool intiIsOverturn1, bool initIsOverturn2, long initNumberSDP)
	{
		base.NumberSDP = initNumberSDP;
		base.Coupon = coupon;
		base.IdSport = match.IdSport;
		base.IsCyber = match.TypeMatch == TypesMatch.Cyber;
		base.IsShort = match.TypeMatch == TypesMatch.Short;
		base.Match1NameProvider = match.NameProvider;
		base.Match1Id = match.IdMatch;
		base.Match1Championat = match.NameChamp;
		base.Match1Team1 = match.Team1;
		base.Match1Team2 = match.Team2;
		base.Match1Score = match.Score.StrScore;
		base.Match1IsOverturn = intiIsOverturn1;
		base.Match2NameProvider = match2.NameProvider;
		base.Match2Id = match2.IdMatch;
		base.Match2Championat = match2.NameChamp;
		base.Match2Team1 = match2.Team1;
		base.Match2Team2 = match2.Team2;
		base.Match2Score = match2.Score.StrScore;
		base.Match2IsOverturn = initIsOverturn2;
		base.IdCompareMatch = initIdCompareMatch;
		match2.Score.GetLastPointScore(base.Coupon.Period);
		base.DifferencePoints = initDifferencePoints;
		base.TimeLastScore = match2.Score.LastTimeUpdate;
	}

	public bool CheckDiffrentScore(int periodCoupon, Match matchMinScore, Match matchMaxScore)
	{
		PointScore lastPointScore = matchMinScore.Score.GetLastPointScore(periodCoupon);
		int periodScoreMatchMinScoreTeam1 = lastPointScore.ValuePointTeam1;
		int periodScoreMatchMinScoreTeam2 = lastPointScore.ValuePointTeam2;
		PointScore lastPointScore2 = matchMaxScore.Score.GetLastPointScore(periodCoupon);
		int periodScoreMatchMaxScoreTeam1 = lastPointScore2.ValuePointTeam1;
		int periodScoreMatchMaxScoreTeam2 = lastPointScore2.ValuePointTeam2;
		if (periodScoreMatchMinScoreTeam1 > periodScoreMatchMaxScoreTeam1 || periodScoreMatchMinScoreTeam2 > periodScoreMatchMaxScoreTeam2)
		{
			return true;
		}
		return false;
	}
}
