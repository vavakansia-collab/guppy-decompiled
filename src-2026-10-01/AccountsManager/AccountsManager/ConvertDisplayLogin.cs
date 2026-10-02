using System;
using System.Globalization;
using System.Windows.Data;

namespace AccountsManager;

internal class ConvertDisplayLogin : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string login = (string)value;
		if (!string.IsNullOrEmpty(login) && login.Length == 12 && login.Remove(2) == "+7")
		{
			login = $"{long.Parse(login):+# ### ### ####}";
		}
		return login;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
