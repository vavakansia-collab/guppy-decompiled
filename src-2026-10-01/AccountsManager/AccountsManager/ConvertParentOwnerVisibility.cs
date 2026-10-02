using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

internal class ConvertParentOwnerVisibility : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string ownerGuid = ((OwnerAccount)value).OwnerGuid;
		if (!string.IsNullOrEmpty(ownerGuid) && ownerGuid != "00000000-0000-0000-0000-000000000000")
		{
			if (OwnersAccountBL.OwnersAccount.SingleOrDefault((OwnerAccount ow) => ow.GUID.ToString() == ownerGuid) != null)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
		return Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
