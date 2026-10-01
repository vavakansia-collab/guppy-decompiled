using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

internal class ConvertForegroundStatusService : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		string text = values[0].ToString();
		_ = values[1];
		return text switch
		{
			"Авторизация" => Brushes.OrangeRed, 
			"Ошибка автор-ции" => Brushes.Violet, 
			"Запрос баланса" => Brushes.Gray, 
			"Приём ставки" => Brushes.Green, 
			_ => Brushes.Black, 
		};
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
