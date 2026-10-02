using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

public class ConvertImageBC : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		AccountSettings accountSettings = (AccountSettings)value;
		if (accountSettings == null)
		{
			return null;
		}
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + accountSettings.NameBCAccount + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
