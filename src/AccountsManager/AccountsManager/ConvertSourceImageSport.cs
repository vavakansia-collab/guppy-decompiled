using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertSourceImageSport : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || (value as ObservableCollection<BetStrategy>).Count == 0)
		{
			return null;
		}
		string[] array = parameter.ToString().Split('|');
		TypeBet typeBet = BetConverter.GetTypeBet(array[0]);
		bool isSuccess = array[1].Contains("Success");
		BetStrategy betStrategy = ((ObservableCollection<BetStrategy>)value).Where((BetStrategy b) => b.Bet.TypeBet == typeBet && b.Bet.Success == isSuccess).LastOrDefault();
		if (betStrategy == null)
		{
			return null;
		}
		string text = betStrategy.CustomStrategy.IdSport.ToString();
		if (!Settings.AMSettings.ImageSportBlack)
		{
			text += "_c";
		}
		return new BitmapImage(new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/SportImages/" + text + ".png"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
