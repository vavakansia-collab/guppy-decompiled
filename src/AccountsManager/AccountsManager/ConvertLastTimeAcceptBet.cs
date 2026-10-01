using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertLastTimeAcceptBet : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((double)value == 0.0)
		{
			return "";
		}
		if ((double)value > 120.0)
		{
			return "> 2 мин.";
		}
		return value.ToString() + " с.";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
