using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertButtonsRuleVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((StatusesRegistration)value == StatusesRegistration.Оформлен && (ViewModelMainWindow.instance.CurrentOwner.AvailabilityPoslegol || ViewModelMainWindow.instance.CurrentOwner.AvailabilityValue || ViewModelMainWindow.instance.CurrentOwner.AvailabilityValuePlus || ViewModelMainWindow.instance.CurrentOwner.AvailabilityValueScore || ViewModelMainWindow.instance.CurrentOwner.AvailabilityFork))
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
