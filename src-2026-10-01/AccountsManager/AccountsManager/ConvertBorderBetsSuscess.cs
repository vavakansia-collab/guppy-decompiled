using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertBorderBetsSuscess : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string[] param = parameter.ToString().Split('|');
		TypeBet typeBet = BetConverter.GetTypeBet(param[0]);
		param[1].Contains("Success");
		parameter.ToString();
		if ((value as StateVM).State.Bets.Where((BetStrategy b) => b.Bet.TypeBet == typeBet && b.Bet.Success).Count() > 0)
		{
			return Brushes.LightGreen;
		}
		return Brushes.Transparent;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
