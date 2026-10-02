using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

internal class ConvertBackgroundPaused : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			AccountSettingsVM acVM = values[0] as AccountSettingsVM;
			if (acVM.AccountSettings.IsPaused && acVM.AccountSettings.ProcessIndex == 1)
			{
				return Brushes.LightYellow;
			}
		}
		catch
		{
		}
		return Brushes.LightGray;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
