using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertVisivilitySendTelegram : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (((OwnerAccount)value).Name == "Guppy")
		{
			return Visibility.Visible;
		}
		return Visibility.Hidden;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return value.ToString().Replace(" ", "");
		}
		catch
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
