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
		byte[] borderBrushMass = value as byte[];
		return new SolidColorBrush(Color.FromArgb(borderBrushMass[0], borderBrushMass[1], borderBrushMass[2], borderBrushMass[3]));
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
