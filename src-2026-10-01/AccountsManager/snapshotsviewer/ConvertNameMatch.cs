using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CompareLibrary;

namespace SnapshotsViewer;

internal class ConvertNameMatch : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		CompareMatch compareMatch = (CompareMatch)value;
		return VMSnapshotsViewer.instance.GetNameCompareMatch(compareMatch);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
