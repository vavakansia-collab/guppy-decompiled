using System;
using System.Globalization;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertCurrencyUcDisplay : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is Currency && (value as Currency).NameEng != "RUB")
		{
			return (value as Currency).Symbol;
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (value as OwnerAccount).GUID;
	}
}
