using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertImageProvider : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string name = value.ToString();
		if (name.Contains("Scout"))
		{
			return null;
		}
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + name + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
