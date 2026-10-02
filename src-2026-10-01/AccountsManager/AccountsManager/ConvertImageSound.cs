using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

public class ConvertImageSound : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((bool)value)
		{
			return new Uri("pack://application:,,,/AccountsManager;component/Resources/soundOn.png");
		}
		return new Uri("pack://application:,,,/AccountsManager;component/Resources/soundOff.png");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
