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
		string[] array = parameter.ToString().Split('|');
		TypeBet typeBet = BetConverter.GetTypeBet(array[0]);
		bool isSuccess = array[1].Contains("Success");
		BetStrategy betStrategy = ((ObservableCollection<BetStrategy>)value).Where((BetStrategy b) => b.Bet.TypeBet == typeBet && b.Bet.Success == isSuccess).LastOrDefault();
		if (betStrategy != null)
		{
			if (!ViewModelMainWindow.instance.MainOwner.IsShortBetsInfo)
			{
				return betStrategy.InfoHeader;
			}
			return betStrategy.UserInfoHeader;
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
