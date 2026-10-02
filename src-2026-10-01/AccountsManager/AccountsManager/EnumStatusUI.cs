using System.ComponentModel;

namespace AccountsManager;

public enum EnumStatusUI
{
	[Description("Запуск приложения")]
	IsStarting,
	[Description("Готов")]
	IsReady,
	[Description("Соединение с сервером...")]
	Connecting,
	[Description("Соединение установлено")]
	Connected,
	[Description("Обновление данных...")]
	Updating,
	[Description("Инициализация...")]
	LoadingStaticData,
	[Description("Закгрузка аккаунтов...")]
	LoadingAccounts,
	[Description("Закгрузка аккаунтов завершена...")]
	LoadedAccounts,
	[Description("Загрузка БД ЦУПИС...")]
	LoadingCupis,
	[Description("Поиск аккаунтов...")]
	Searching
}
