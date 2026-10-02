using System.Windows;

namespace AccountsManager;

public sealed class WaterMarkExtentions
{
	public static readonly DependencyProperty WaterMarkProperty = DependencyProperty.RegisterAttached("WaterMark", typeof(string), typeof(FrameworkElement), new FrameworkPropertyMetadata(""));

	public static string GetWaterMark(DependencyObject obj)
	{
		return (string)obj.GetValue(WaterMarkProperty);
	}

	public static void SetWaterMark(DependencyObject obj, string value)
	{
		obj.SetValue(WaterMarkProperty, value);
	}
}
