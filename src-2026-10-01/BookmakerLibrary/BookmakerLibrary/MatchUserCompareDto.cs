namespace BookmakerLibrary;

public class MatchUserCompareDto
{
	public Match Match;

	public int TotalCoupons;

	public int TotalCouponsUnAvailability;

	public MatchUserCompareDto()
	{
	}

	public MatchUserCompareDto(Match intiMatch)
	{
		Match = intiMatch;
	}
}
