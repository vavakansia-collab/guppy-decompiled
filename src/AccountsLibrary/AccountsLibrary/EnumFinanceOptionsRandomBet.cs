using System.ComponentModel;

namespace AccountsLibrary;

public enum EnumFinanceOptionsRandomBet
{
	[Description("Интервал сумм")]
	Interval,
	[Description("Процент от баланса")]
	Percent
}
