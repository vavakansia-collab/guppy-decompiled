using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

public class ConvertColorIndicatorStatusIU : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((EnumStatusUI)value == EnumStatusUI.Connecting)
		{
			return Brushes.Yellow;
		}
		if ((EnumStatusUI)value == EnumStatusUI.Updating)
		{
			return Brushes.GreenYellow;
		}
		return Brushes.Green;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
