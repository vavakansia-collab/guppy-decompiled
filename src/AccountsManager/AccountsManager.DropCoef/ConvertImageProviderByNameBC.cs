using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager.DropCoef;

internal class ConvertImageProviderByNameBC : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + value?.ToString() + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
