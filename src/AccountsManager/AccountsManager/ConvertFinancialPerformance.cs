using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertFinancialPerformance : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((double)value == 0.0)
		{
			return "";
		}
		return Math.Floor((double)value).ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return value.ToString().Replace("\u00a0", "");
		}
		catch
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
