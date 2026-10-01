using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BookmakerLibrary;

namespace AccountsManager.DropCoef;

internal class ConvertNameSportById : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || value == DependencyProperty.UnsetValue)
		{
			return null;
		}
		return SportBL.GetSportByID((int)value).FullName;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
