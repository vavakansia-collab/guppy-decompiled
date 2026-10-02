using StrategyLibrary;

namespace AccountsLibrary;

public class BetValue : BetStrategy
{
	public Value Value { get; set; }

	public BetValue(AccountSettings initAccountSettings, Bet initBet, Value initValue, StrategySettingsBase initStrategySettingsBase)
		: base(initAccountSettings, initBet, initValue, initStrategySettingsBase)
	{
		base.Bet = initBet;
		Value = initValue;
		base.StrategySettings = initStrategySettingsBase;
	}
}
