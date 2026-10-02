using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTypeBetStrategyImageSportSource : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		BetStrategy betStrategy = value as BetStrategy;
		return "pack://application:,,,/BookmakerLibrary;component/Resources/SportImages/" + betStrategy.CustomStrategy.IdSport + ".png";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
