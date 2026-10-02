using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager.DropCoef;

internal class ConvertIntervalsVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((TimeType)value == TimeType.Current)
		{
			return Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
