using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager.DropCoef;

internal class ConvertImageSportById : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/SportImages/" + value?.ToString() + ".png");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
