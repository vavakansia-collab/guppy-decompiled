using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager.DropCoef;

public class EnumTimeTypeDescriptionConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return string.Empty;
		}
		FieldInfo field = value.GetType().GetField(value.ToString());
		if (field == null)
		{
			return value.ToString();
		}
		DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
		if (array.Length == 0)
		{
			return value.ToString();
		}
		return array[0].Description;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
