using System.ComponentModel;

namespace AccountsLibrary;

public enum TypeBet
{
	[Description("Не определено")]
	_empty,
	[Description("Рандомная ставка")]
	Random,
	[Description("Послегол")]
	Poslegol,
	[Description("Вилуи")]
	Value,
	[Description("Вилка")]
	Fork,
	[Description("Вилуи+")]
	ValuePlus,
	[Description("ВилуиScore")]
	ValueScore
}
