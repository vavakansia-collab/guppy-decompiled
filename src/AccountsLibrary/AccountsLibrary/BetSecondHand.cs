using BookmakerLibrary;

namespace AccountsLibrary;

public class BetSecondHand
{
	public Bet Bet { get; set; } = null;

	public AccountSettings AccountSettings { get; set; } = null;

	public string MatchNameProvider { get; set; }

	public long MatchId { get; set; }

	public string MatchChampionat { get; set; }

	public string MatchTeam1 { get; set; }

	public string MatchTeam2 { get; set; }

	public string MatchScore { get; set; }

	public double Percent { get; set; } = 0.0;

	public string InfoHeader { get; set; } = "";

	public BetSecondHand()
	{
	}

	public BetSecondHand(Bet initBet, AccountSettings initAccountSettings, Match match)
	{
		Bet = initBet;
		AccountSettings = initAccountSettings;
		MatchNameProvider = match.NameProvider;
		MatchId = match.IdMatch;
		MatchChampionat = match.NameChamp;
		MatchTeam1 = match.Team1;
		MatchTeam2 = match.Team2;
		MatchScore = match.Score.StrScore;
	}
}
