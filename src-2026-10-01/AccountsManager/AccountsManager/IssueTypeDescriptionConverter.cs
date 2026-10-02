using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using BookmakerLibrary;

namespace AccountsManager;

public class IssueTypeDescriptionConverter : IValueConverter
{
	public string[] ListFinanceOptionsRandomBet => GetStrings();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is IssueType format)
		{
			return GetString(format);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string s)
		{
			return Enum.Parse(typeof(IssueType), s.Substring(0, s.IndexOf(':')));
		}
		return null;
	}

	public static string GetString(IssueType financeOptions)
	{
		return GetDescription(financeOptions);
	}

	public static string GetDescription(IssueType financeOptions)
	{
		return financeOptions.GetType().GetMember(financeOptions.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
	}

	public static string[] GetStrings()
	{
		List<string> list = new List<string>();
		foreach (IssueType financeOptions in Enum.GetValues(typeof(IssueType)))
		{
			list.Add(GetString(financeOptions));
		}
		return list.ToArray();
	}
}
