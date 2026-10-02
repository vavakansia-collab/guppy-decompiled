using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace SnapshotsViewer;

internal class ConvertTypeCoupons : IValueConverter
{
	private string GetEnumDescription(Enum enumObj)
	{
		object[] attribArray = enumObj.GetType().GetField(enumObj.ToString()).GetCustomAttributes(inherit: false);
		if (attribArray.Length == 0)
		{
			return enumObj.ToString();
		}
		return (attribArray[0] as DescriptionAttribute).Description;
	}

	object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Enum myEnum = (Enum)value;
		return GetEnumDescription(myEnum);
	}

	object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return string.Empty;
	}
}
