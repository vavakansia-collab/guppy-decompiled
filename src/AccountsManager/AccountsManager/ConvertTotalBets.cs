using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertTotalBets : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((value as ObservableCollection<BetStrategy>).Count != 0)
		{
			string[] array = parameter.ToString().Split('|');
			TypeBet typeBet = BetConverter.GetTypeBet(array[0]);
			bool isSuccess = array[1].Contains("Success");
			int num = (value as ObservableCollection<BetStrategy>).Where((BetStrategy b) => b.Bet.TypeBet == typeBet && b.Bet.Success == isSuccess).Count();
			if (num > 0)
			{
				return num;
			}
			return "";
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
