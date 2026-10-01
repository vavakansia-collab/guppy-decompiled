using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertBetStrategyBackgroundTimeResponce : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		BetStrategy betStrategy = value as BetStrategy;
		if ((betStrategy.AccountSettings.NameProvider == "OlimpBet" && betStrategy.Bet.TimeResponce > 15.0) || (betStrategy.AccountSettings.NameProvider == "FonbetRU" && betStrategy.Bet.TimeResponce > 7.0) || (betStrategy.AccountSettings.NameProvider == "LigaStavokRU" && betStrategy.Bet.TimeResponce > 11.0) || (betStrategy.AccountSettings.NameProvider.Contains("Marathon") && betStrategy.Bet.TimeResponce > 10.0) || (betStrategy.AccountSettings.NameProvider == "Winline" && betStrategy.Bet.TimeResponce > 10.0))
		{
			return Brushes.Red;
		}
		return Brushes.Transparent;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
