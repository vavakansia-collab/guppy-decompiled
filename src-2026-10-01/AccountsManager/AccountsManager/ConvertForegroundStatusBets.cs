using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

internal class ConvertForegroundStatusBets : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		AccountSettingsVM obj = values[0] as AccountSettingsVM;
		string statusBets = values[1].ToString();
		if (obj.AccountSettings.IsPaused || statusBets == "Не запущен...")
		{
			return Brushes.Transparent;
		}
		if (statusBets == "Приём ставки")
		{
			return Brushes.Green;
		}
		return Brushes.Black;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
