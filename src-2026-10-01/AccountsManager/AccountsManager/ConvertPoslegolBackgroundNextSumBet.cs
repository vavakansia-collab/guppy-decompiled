using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertPoslegolBackgroundNextSumBet : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
		{
			return Brushes.Transparent;
		}
		double nextSubBet = (double)values[0];
		AccountSettingsVM acVM = (AccountSettingsVM)values[1];
		if (acVM.AccountSettings.PoslegolsSettings.Count == 0)
		{
			return Brushes.Transparent;
		}
		double minSumBet = acVM.AccountSettings.PoslegolsSettings.Select((PoslegolSettings s) => s.MinSumBet).Min();
		if (nextSubBet < minSumBet && acVM.StateVM.State.Balance >= 0.0 && acVM.StateVM.State.IsRunning)
		{
			return Brushes.Red;
		}
		return Brushes.Transparent;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
