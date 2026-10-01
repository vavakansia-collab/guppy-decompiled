using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertTypeBetStrategyItemView : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		_ = (value as ViewModelBetStrategyItem).BetStrategy.Bet.TypeBet;
		_ = 2;
		return new ucBetPoslegol();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
