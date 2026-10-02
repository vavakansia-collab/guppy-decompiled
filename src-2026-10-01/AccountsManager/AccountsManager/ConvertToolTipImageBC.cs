using System;
using System.Globalization;
using System.Windows.Data;
using CommonLibrary;

namespace AccountsManager;

internal class ConvertToolTipImageBC : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			string nameBC = (string)values[0];
			Proxy proxy = (Proxy)values[1];
			DateTime dateTimeCreate = (DateTime)values[2];
			string accountGUID = ((Guid)values[3]/*cast due to .constrained prefix*/).ToString();
			string cupisLogin = (string)values[4];
			string toolTip = nameBC + ". Cоздан " + dateTimeCreate.ToString() + ", " + Math.Floor(DateTime.Now.Subtract(dateTimeCreate).TotalDays) + " дней назад.";
			toolTip = toolTip + Environment.NewLine + "ЦУПИС: " + cupisLogin;
			if (proxy != null)
			{
				toolTip = toolTip + Environment.NewLine + "Прокси: " + ((proxy.Country != null) ? proxy.Country.ShortName : "") + " " + proxy.NameProxy + ", от " + proxy.DateTimeCreate.ToShortDateString();
			}
			return toolTip + Environment.NewLine + "GUID: " + accountGUID;
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
