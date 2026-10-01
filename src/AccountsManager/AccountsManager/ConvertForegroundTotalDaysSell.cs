using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertForegroundTotalDaysSell : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		AccountSettingsVM accountSettingsVM = values[0] as AccountSettingsVM;
		Brush transparent = Brushes.Transparent;
		if (accountSettingsVM.AccountSettings.StatusRegistration != StatusesRegistration.Новый)
		{
			_ = accountSettingsVM.AccountSettings.StatusRegistration;
			_ = 3;
		}
		return transparent;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
