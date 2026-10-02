using StrategyLibrary;

namespace AccountsLibrary;

public class BetPoslegol : BetStrategy
{
	public Poslegol Poslegol { get; set; }

	public BetPoslegol(AccountSettings initAccountSettings, Bet initBet, Poslegol initCompliteCoupon, StrategySettingsBase initStrategySettingsBase)
		: base(initAccountSettings, initBet, initCompliteCoupon, initStrategySettingsBase)
	{
		base.Bet = initBet;
		Poslegol = initCompliteCoupon;
		base.StrategySettings = initStrategySettingsBase;
	}
}
