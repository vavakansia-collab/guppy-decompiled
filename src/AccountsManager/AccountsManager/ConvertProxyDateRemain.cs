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
		string text = "  ";
		TimeSpan timeSpan = ((Proxy)value).DateTimeEnd - DateTime.Now;
		if (timeSpan.TotalDays > 0.0)
		{
			double num = Math.Floor(timeSpan.TotalDays);
			double num2 = Math.Floor(timeSpan.TotalHours) - num * 24.0;
			return text + num + "дн. " + num2 + "ч.";
		}
		return text + timeSpan.TotalHours + " ч.";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
