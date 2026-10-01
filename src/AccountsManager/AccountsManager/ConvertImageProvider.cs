using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertImageProvider : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value.ToString();
		if (text.Contains("Scout"))
		{
			return null;
		}
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + text + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
