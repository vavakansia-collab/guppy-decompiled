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
		AccountSettingsVM acVM = values[0] as AccountSettingsVM;
		Brush foreground = Brushes.Transparent;
		if (acVM.AccountSettings.StatusRegistration != StatusesRegistration.Новый)
		{
			_ = acVM.AccountSettings.StatusRegistration;
			_ = 3;
		}
		return foreground;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
