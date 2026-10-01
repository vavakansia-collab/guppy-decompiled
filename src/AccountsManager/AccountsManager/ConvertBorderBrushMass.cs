using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

internal class ConvertBorderBrushMass : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		byte[] array = value as byte[];
		return new SolidColorBrush(Color.FromArgb(array[0], array[1], array[2], array[3]));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
