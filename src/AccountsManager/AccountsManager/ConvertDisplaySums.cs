using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertDisplaySums : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return ((double)value).ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		double result = 0.0;
		double.TryParse(value.ToString().Replace(" ", ""), out result);
		return result;
	}
}
