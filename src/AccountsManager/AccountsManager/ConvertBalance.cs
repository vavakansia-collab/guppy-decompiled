using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertBalance : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		AccountSettingsVM accountSettingsVM = values[0] as AccountSettingsVM;
		if (accountSettingsVM.StateVM.State == null || !accountSettingsVM.StateVM.State.IsRunning)
		{
			return "-";
		}
		if (accountSettingsVM.AccountSettings.Currency.NameEng == "RUB")
		{
			return accountSettingsVM.StateVM.State.Balance.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		}
		return accountSettingsVM.StateVM.State.Balance.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
