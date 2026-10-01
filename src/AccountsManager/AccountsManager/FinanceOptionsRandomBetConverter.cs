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
		if (value is EnumFinanceOptionsRandomBet financeOptions)
		{
			return GetString(financeOptions);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string text)
		{
			return Enum.Parse(typeof(EnumFinanceOptionsRandomBet), text.Substring(0, text.IndexOf(':')));
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
		foreach (EnumFinanceOptionsRandomBet value in Enum.GetValues(typeof(EnumFinanceOptionsRandomBet)))
		{
			list.Add(GetString(value));
		}
		return list.ToArray();
	}
}
