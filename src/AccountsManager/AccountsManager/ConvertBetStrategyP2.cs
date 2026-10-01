using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertBetStrategyP2 : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (value as BetStrategy).Bet.TimeBegin.ToLocalTime().ToString("dd.MM.yyyy HH.mm.ss");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
