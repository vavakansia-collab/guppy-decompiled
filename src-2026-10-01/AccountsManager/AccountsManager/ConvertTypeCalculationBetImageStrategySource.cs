using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTypeCalculationBetImageStrategySource : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return null;
		}
		Bet bet = (Bet)value;
		if (!bet.Success)
		{
			return null;
		}
		if (bet.TypeCalculationBet == TypesCalculationBet.NotCalculation)
		{
			return new Uri("pack://application:,,,/AccountsManager;component/Resources/delay.png");
		}
		if (bet.TypeCalculationBet == TypesCalculationBet.Win)
		{
			return new Uri("pack://application:,,,/AccountsManager;component/Resources/win.png");
		}
		if (bet.TypeCalculationBet == TypesCalculationBet.Lose)
		{
			return new Uri("pack://application:,,,/AccountsManager;component/Resources/lose.png");
		}
		return new Uri("pack://application:,,,/AccountsManager;component/Resources/linking.png");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
