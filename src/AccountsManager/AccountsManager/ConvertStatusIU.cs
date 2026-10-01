using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace AccountsManager;

public class ConvertStatusIU : IValueConverter
{
	public string[] ListFinanceOptionsRandomBet => GetStrings();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is EnumStatusUI statusIU)
		{
			return GetString(statusIU);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string text)
		{
			return Enum.Parse(typeof(EnumStatusUI), text.Substring(0, text.IndexOf(':')));
		}
		return null;
	}

	public static string GetString(EnumStatusUI statusIU)
	{
		return GetDescription(statusIU);
	}

	public static string GetDescription(EnumStatusUI statusIU)
	{
		return statusIU.GetType().GetMember(statusIU.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
	}

	public static string[] GetStrings()
	{
		List<string> list = new List<string>();
		foreach (EnumStatusUI value in Enum.GetValues(typeof(EnumStatusUI)))
		{
			list.Add(GetString(value));
		}
		return list.ToArray();
	}
}
