using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertStatusService : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			AccountSettingsVM accountSettingsVM = values[1] as AccountSettingsVM;
			string result = values[0].ToString();
			if (accountSettingsVM.AccountSettings.IsPaused)
			{
				return "На паузе";
			}
			if (accountSettingsVM.StateVM.State.IsRunning)
			{
				return result;
			}
			return "";
		}
		catch
		{
		}
		return "";
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
