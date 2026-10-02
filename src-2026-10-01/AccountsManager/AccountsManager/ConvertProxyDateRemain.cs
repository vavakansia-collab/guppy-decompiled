using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CommonLibrary;

namespace AccountsManager;

internal class ConvertProxyDateRemain : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value.ToString() == "")
		{
			return "";
		}
		if (((Proxy)value).IsEmpty)
		{
			return "";
		}
		string strRemain = "  ";
		TimeSpan ts = ((Proxy)value).DateTimeEnd - DateTime.Now;
		if (ts.TotalDays > 0.0)
		{
			double days = Math.Floor(ts.TotalDays);
			double hours = Math.Floor(ts.TotalHours) - days * 24.0;
			return strRemain + days + "дн. " + hours + "ч.";
		}
		return strRemain + ts.TotalHours + " ч.";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
