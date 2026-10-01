using System;
using System.Globalization;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertOwnerByBuid : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return OwnersAccountBL.GetOwnerAccountByGuid(value.ToString());
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return (value as OwnerAccount).GUID;
	}
}
