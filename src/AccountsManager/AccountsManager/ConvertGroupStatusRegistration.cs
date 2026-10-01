using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertGroupStatusRegistration : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value.ToString() == "Новый")
		{
			return "Новые";
		}
		if (value.ToString() == "Оформлен")
		{
			return "Оформленные";
		}
		if (value.ToString() == "Отработан")
		{
			return "Отработанные";
		}
		if (value.ToString() == "Отложен")
		{
			return "Отложенные";
		}
		if (value.ToString() == "Архив")
		{
			return "В архиве";
		}
		return value.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
