using StrategyLibrary;

namespace AccountsLibrary;

public class BetFork : BetStrategy
{
	public Fork Fork { get; set; }

	public BetSecondHand BetForkSecondHand { get; set; } = null;

	public BetFork(AccountSettings initAccountSettings, Bet initBet, Fork initFork, StrategySettingsBase initStrategySettingsBase)
		: base(initAccountSettings, initBet, initFork, initStrategySettingsBase)
	{
		base.Bet = initBet;
		Fork = initFork;
		base.StrategySettings = initStrategySettingsBase;
	}
}
