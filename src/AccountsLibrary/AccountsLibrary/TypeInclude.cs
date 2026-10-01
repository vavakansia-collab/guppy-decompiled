using System.ComponentModel;

namespace AccountsLibrary;

public enum TypeInclude
{
	[Description("Только выбранное")]
	Filter,
	[Description("Всё, кроме выбранного")]
	Except
}
