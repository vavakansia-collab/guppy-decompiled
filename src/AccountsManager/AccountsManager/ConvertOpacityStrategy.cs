using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertOpacityStrategy : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((bool)value)
		{
			return 1.0;
		}
		return 0.0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
