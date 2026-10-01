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
		if (value is IssueType financeOptions)
		{
			return GetString(financeOptions);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string text)
		{
			return Enum.Parse(typeof(IssueType), text.Substring(0, text.IndexOf(':')));
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
		foreach (IssueType value in Enum.GetValues(typeof(IssueType)))
		{
			list.Add(GetString(value));
		}
		return list.ToArray();
	}
}
