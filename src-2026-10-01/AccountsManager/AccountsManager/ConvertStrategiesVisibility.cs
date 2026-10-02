using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertStrategiesVisibility : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		TypeBet typeBet = BetConverter.GetTypeBet(parameter.ToString());
		AccountSettingsVM accountSettingsVM = values[1] as AccountSettingsVM;
		if ((typeBet == TypeBet.Poslegol && accountSettingsVM.AccountSettings.IsCheckedPoslegol) || (typeBet == TypeBet.Value && accountSettingsVM.AccountSettings.IsCheckedValue) || (typeBet == TypeBet.ValuePlus && accountSettingsVM.AccountSettings.IsCheckedValuePlus) || (typeBet == TypeBet.ValueScore && accountSettingsVM.AccountSettings.IsCheckedValueScore) || (typeBet == TypeBet.Fork && accountSettingsVM.AccountSettings.IsCheckedFork))
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
