using System.ComponentModel;

namespace AccountsLibrary;

public enum TypeChangePayoutRatio
{
	[Description("Принимать любые изменения")]
	All,
	[Description("Принимать при подвышении")]
	OnlyUp,
	[Description("Не принимать изменения")]
	OnlyCurrent
}
