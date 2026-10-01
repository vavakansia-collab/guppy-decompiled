using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CompareLibrary;

namespace SnapshotsViewer;

internal class ConvertImageProviderByCMP : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string nameProvider = ((CompareMatchParameter)value).NameProvider;
		return new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + nameProvider + ".jpg");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DependencyProperty.UnsetValue;
	}
}
