using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AccountsLibrary;
using CommonLibrary;

namespace AccountsManager;

internal class ConvertProxy : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value?.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return new Proxy(value.ToString());
		}
		catch
		{
			return AccountsLibrary.ProxyBL.ListProxy.SingleOrDefault((Proxy pr) => pr.IsEmpty);
		}
	}
}
