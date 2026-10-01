using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTypeBetStrategyToolTip : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (TypeBet)value switch
		{
			TypeBet.Poslegol => "ПОСЛЕГОЛ", 
			TypeBet.Fork => "ВИЛКА", 
			_ => "ВАЛУИ", 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
