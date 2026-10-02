using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertBalance : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		AccountSettingsVM acVM = values[0] as AccountSettingsVM;
		if (acVM.StateVM.State == null || !acVM.StateVM.State.IsRunning)
		{
			return "-";
		}
		if (acVM.AccountSettings.Currency.NameEng == "RUB")
		{
			return acVM.StateVM.State.Balance.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		}
		return acVM.StateVM.State.Balance.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
