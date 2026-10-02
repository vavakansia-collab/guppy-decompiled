using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertDateOnly : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return ((DateTime)value).ToShortDateString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (DateTime.TryParse(value as string, out var resultDateTime))
		{
			return resultDateTime;
		}
		return DependencyProperty.UnsetValue;
	}
}
