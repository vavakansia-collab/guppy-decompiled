using System.ComponentModel;

namespace AccountsManager.DropCoef;

public enum TimeType
{
	[Description("Предстоящие матчи")]
	Current,
	[Description("Архивные данные")]
	Arhive
}
