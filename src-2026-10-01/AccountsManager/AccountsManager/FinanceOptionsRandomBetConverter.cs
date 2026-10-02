using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using AccountsLibrary;

namespace AccountsManager;

public class FinanceOptionsRandomBetConverter : IValueConverter
{
	public string[] ListFinanceOptionsRandomBet => GetStrings();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is EnumFinanceOptionsRandomBet format)
		{
			return GetString(format);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string s)
		{
			return Enum.Parse(typeof(EnumFinanceOptionsRandomBet), s.Substring(0, s.IndexOf(':')));
		}
		return null;
	}

	public static string GetString(EnumFinanceOptionsRandomBet financeOptions)
	{
		return financeOptions.ToString() + ": " + GetDescription(financeOptions);
	}

	public static string GetDescription(EnumFinanceOptionsRandomBet financeOptions)
	{
		return financeOptions.GetType().GetMember(financeOptions.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
	}

	public static string[] GetStrings()
	{
		List<string> list = new List<string>();
		foreach (EnumFinanceOptionsRandomBet financeOptions in Enum.GetValues(typeof(EnumFinanceOptionsRandomBet)))
		{
			list.Add(GetString(financeOptions));
		}
		return list.ToArray();
	}
}
