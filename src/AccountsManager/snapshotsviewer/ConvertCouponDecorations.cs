using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BookmakerLibrary;

namespace SnapshotsViewer;

internal class ConvertCouponDecorations : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (((Coupon)value).PayoutRatio == -1.0)
		{
			return TextDecorations.Strikethrough;
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
