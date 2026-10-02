using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountsManager;

internal class ConvertAccountBorderBrushMass : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		_ = values[1];
		if (values[0] == DependencyProperty.UnsetValue || values[0] == null)
		{
			return Brushes.LightGray;
		}
		byte[] borderBrushMass = values[0] as byte[];
		if (borderBrushMass[0] == 0 && borderBrushMass[1] == 0 && borderBrushMass[2] == 0 && borderBrushMass[3] == 0)
		{
			return Brushes.LightGray;
		}
		return new SolidColorBrush(Color.FromArgb(borderBrushMass[0], borderBrushMass[1], borderBrushMass[2], borderBrushMass[3]));
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
