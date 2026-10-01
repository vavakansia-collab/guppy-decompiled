using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertForegroundStatusRegistration : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((StatusesRegistration)value == StatusesRegistration.Новый)
		{
			return Brushes.OrangeRed;
		}
		return Brushes.Black;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
