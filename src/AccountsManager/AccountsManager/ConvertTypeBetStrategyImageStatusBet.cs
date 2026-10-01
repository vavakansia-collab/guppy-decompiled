using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTypeBetStrategyImageStatusBet : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		if ((value as BetStrategy).Bet.Success)
		{
			return new Uri("pack://application:,,,/AccountsManager;component/Resources/success2.png");
		}
		return new Uri("pack://application:,,,/AccountsManager;component/Resources/btnDelete.png");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
