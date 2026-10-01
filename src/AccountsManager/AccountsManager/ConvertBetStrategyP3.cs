using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertBetStrategyP3 : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (double)value + " сек.";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
