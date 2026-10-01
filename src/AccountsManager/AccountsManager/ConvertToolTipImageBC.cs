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
			string text = (string)values[0];
			Proxy proxy = (Proxy)values[1];
			DateTime value = (DateTime)values[2];
			string text2 = ((Guid)values[3]/*cast due to .constrained prefix*/).ToString();
			string text3 = (string)values[4];
			string text4 = text + ". Cоздан " + value.ToString() + ", " + Math.Floor(DateTime.Now.Subtract(value).TotalDays) + " дней назад.";
			text4 = text4 + Environment.NewLine + "ЦУПИС: " + text3;
			if (proxy != null)
			{
				text4 = text4 + Environment.NewLine + "Прокси: " + ((proxy.Country != null) ? proxy.Country.ShortName : "") + " " + proxy.NameProxy + ", от " + proxy.DateTimeCreate.ToShortDateString();
			}
			return text4 + Environment.NewLine + "GUID: " + text2;
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
