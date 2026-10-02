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
		string[] param = parameter.ToString().Split('|');
		TypeBet typeBet = BetConverter.GetTypeBet(param[0]);
		bool isSuccess = param[1].Contains("Success");
		BetStrategy lastBet = ((ObservableCollection<BetStrategy>)value).Where((BetStrategy b) => b.Bet.TypeBet == typeBet && b.Bet.Success == isSuccess).LastOrDefault();
		if (lastBet == null)
		{
			return null;
		}
		string strIdSport = lastBet.CustomStrategy.IdSport.ToString();
		if (!Settings.AMSettings.ImageSportBlack)
		{
			strIdSport += "_c";
		}
		return new BitmapImage(new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/SportImages/" + strIdSport + ".png"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
