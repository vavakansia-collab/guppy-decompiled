using System.ComponentModel;

namespace AccountsLibrary;

public enum AccountCommandEnum
{
	[Description("Запуск")]
	Start,
	[Description("Остановка")]
	Stop,
	[Description("Запрос баланса")]
	GetBalance,
	[Description("Очистить данные")]
	ClearMessages,
	[Description("Сброс статуса Poslegol")]
	SetPoslegolStatusDefault,
	[Description("Сброс статуса Value")]
	SetValueStatusDefault,
	[Description("Сброс статуса Вилка")]
	SetForkStatusDefault,
	[Description("Сброс статуса Value+")]
	SetValuePlusStatusDefault,
	[Description("Сброс статуса ValueScore")]
	SetValueScoreStatusDefault,
	[Description("Поставить случайную ставку")]
	PutRandomBet
}
