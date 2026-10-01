using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertNumberDefault : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (!(value.ToString() == "0"))
		{
			return value.ToString();
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
