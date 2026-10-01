using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

public class SortKoefPoslegolConverter : IValueConverter
{
	public string[] ListOption => GetStrings();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is SortKoefEnum option)
		{
			return GetString(option);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string text)
		{
			return Enum.Parse(typeof(SortKoefEnum), text.Substring(0, text.IndexOf(':')));
		}
		return null;
	}

	public static string GetString(SortKoefEnum option)
	{
		return option.ToString() + ": " + GetDescription(option);
	}

	public static string GetDescription(SortKoefEnum option)
	{
		return option.GetType().GetMember(option.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
	}

	public static string[] GetStrings()
	{
		List<string> list = new List<string>();
		foreach (SortKoefEnum value in Enum.GetValues(typeof(SortKoefEnum)))
		{
			list.Add(GetString(value));
		}
		return list.ToArray();
	}
}
