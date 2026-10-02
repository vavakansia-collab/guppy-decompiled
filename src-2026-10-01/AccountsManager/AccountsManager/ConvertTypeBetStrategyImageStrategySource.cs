using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTypeBetStrategyImageStrategySource : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (TypeBet)value switch
		{
			TypeBet.Poslegol => new Uri("pack://application:,,,/AccountsManager;component/Resources/poslegol.png"), 
			TypeBet.Fork => new Uri("pack://application:,,,/AccountsManager;component/Resources/fork.png"), 
			_ => new Uri("pack://application:,,,/AccountsManager;component/Resources/value.png"), 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
