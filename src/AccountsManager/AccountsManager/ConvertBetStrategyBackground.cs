using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertBetStrategyBackground : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if ((value as BetStrategy).Bet.Success)
		{
			return Brushes.LightGreen;
		}
		return Brushes.LightGray;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
