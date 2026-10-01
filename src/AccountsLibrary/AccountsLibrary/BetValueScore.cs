using StrategyLibrary;

namespace AccountsLibrary;

public class BetValueScore : BetStrategy
{
	public ValueScore Value { get; set; }

	public BetValueScore(AccountSettings initAccountSettings, Bet initBet, ValueScore initValueScore, StrategySettingsBase initStrategySettingsBase)
		: base(initAccountSettings, initBet, initValueScore, initStrategySettingsBase)
	{
		base.Bet = initBet;
		Value = initValueScore;
		base.StrategySettings = initStrategySettingsBase;
	}
}
