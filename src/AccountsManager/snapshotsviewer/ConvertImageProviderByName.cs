using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SnapshotsViewer;

internal class ConvertImageProviderByName : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string name = ((ProviderMatch)value).Name;
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + name + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
