using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

internal class ConvertColorProcessIndex : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			if ((values[0] as AccountSettingsVM).AccountSettings.ProcessIndex == 0)
			{
				return Brushes.LightGray;
			}
			return Brushes.Green;
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
