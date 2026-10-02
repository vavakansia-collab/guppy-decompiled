using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BookmakerLibrary;

namespace SnapshotsViewer;

internal class ConvertImageSport : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/SportImages/" + (value as Sport).IdSport + ".png");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
