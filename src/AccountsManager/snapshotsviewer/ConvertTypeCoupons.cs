using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace SnapshotsViewer;

internal class ConvertTypeCoupons : IValueConverter
{
	private string GetEnumDescription(Enum enumObj)
	{
		object[] customAttributes = enumObj.GetType().GetField(enumObj.ToString()).GetCustomAttributes(inherit: false);
		if (customAttributes.Length == 0)
		{
			return enumObj.ToString();
		}
		return (customAttributes[0] as DescriptionAttribute).Description;
	}

	object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Enum enumObj = (Enum)value;
		return GetEnumDescription(enumObj);
	}

	object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return string.Empty;
	}
}
