using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertBetStrategyP1 : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if ((value as BetStrategy).Bet.Success)
		{
			return "Ставка принята".ToUpper();
		}
		return "Ставка не принята".ToUpper();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
