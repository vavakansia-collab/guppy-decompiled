using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertDisplayLogin : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = (string)value;
		if (!string.IsNullOrEmpty(text) && text.Length == 12 && text.Remove(2) == "+7")
		{
			text = $"{long.Parse(text):+# ### ### ####}";
		}
		return text;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
