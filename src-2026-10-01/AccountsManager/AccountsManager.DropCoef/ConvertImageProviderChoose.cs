using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BookmakerLibrary;

namespace AccountsManager.DropCoef;

internal class ConvertImageProviderChoose : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string nameProvider = ((Provider)value).Name;
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + nameProvider + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
