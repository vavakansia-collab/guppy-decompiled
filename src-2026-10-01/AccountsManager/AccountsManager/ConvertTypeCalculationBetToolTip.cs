using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTypeCalculationBetToolTip : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return (TypesCalculationBet)value switch
		{
			TypesCalculationBet.NotCalculation => "не рассчитана", 
			TypesCalculationBet.Win => "выигрыш", 
			TypesCalculationBet.Lose => "проигрыш", 
			_ => "не определено", 
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
