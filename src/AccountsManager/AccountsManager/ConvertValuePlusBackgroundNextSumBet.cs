using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertValuePlusBackgroundNextSumBet : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
		{
			return Brushes.Transparent;
		}
		double num = (double)values[0];
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)values[1];
		if (accountSettingsVM.AccountSettings.ValuesPlusSettings.Count == 0)
		{
			return Brushes.Transparent;
		}
		double num2 = accountSettingsVM.AccountSettings.ValuesPlusSettings.Select((ValuesPlusSettings s) => s.MinSumBet).Min();
		if (num < num2 && accountSettingsVM.StateVM.State.Balance >= 0.0 && accountSettingsVM.StateVM.State.IsRunning)
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
