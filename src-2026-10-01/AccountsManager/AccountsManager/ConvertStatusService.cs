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
			AccountSettingsVM acVM = values[1] as AccountSettingsVM;
			string statusService = values[0].ToString();
			if (acVM.AccountSettings.IsPaused)
			{
				return "На паузе";
			}
			if (acVM.StateVM.State.IsRunning)
			{
				return statusService;
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
