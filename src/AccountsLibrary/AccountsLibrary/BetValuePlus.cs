using StrategyLibrary;

namespace AccountsLibrary;

public class BetValuePlus : BetStrategy
{
	public ValuePlus Value { get; set; }

	public BetValuePlus(AccountSettings initAccountSettings, Bet initBet, ValuePlus initValuePlus, StrategySettingsBase initStrategySettingsBase)
		: base(initAccountSettings, initBet, initValuePlus, initStrategySettingsBase)
	{
		base.Bet = initBet;
		Value = initValuePlus;
		base.StrategySettings = initStrategySettingsBase;
	}
}
