using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertToolTipLastBet : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string[] param = parameter.ToString().Split('|');
		TypeBet typeBet = BetConverter.GetTypeBet(param[0]);
		bool isSuccess = param[1].Contains("Success");
		BetStrategy lastBet = ((ObservableCollection<BetStrategy>)value).Where((BetStrategy b) => b.Bet.TypeBet == typeBet && b.Bet.Success == isSuccess).LastOrDefault();
		if (lastBet != null)
		{
			if (!ViewModelMainWindow.instance.MainOwner.IsShortBetsInfo)
			{
				return lastBet.InfoHeader;
			}
			return lastBet.UserInfoHeader;
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
