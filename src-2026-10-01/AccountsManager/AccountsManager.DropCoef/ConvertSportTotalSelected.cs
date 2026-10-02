using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager.DropCoef;

internal class ConvertSportTotalSelected : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return null;
		}
		int totalSelected = (int)value;
		if (totalSelected == 0)
		{
			return "-";
		}
		return totalSelected.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
