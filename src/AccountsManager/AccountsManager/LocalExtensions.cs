using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace AccountsManager;

internal static class LocalExtensions
{
	public static void ForWindowFromChild(this object childDependencyObject, Action<Window> action)
	{
		DependencyObject dependencyObject = childDependencyObject as DependencyObject;
		while (dependencyObject != null)
		{
			dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			if (dependencyObject is Window)
			{
				action(dependencyObject as Window);
				break;
			}
		}
	}

	public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
	{
		if (((FrameworkElement)templateFrameworkElement).TemplatedParent is Window obj)
		{
			action(obj);
		}
	}

	public static IntPtr GetWindowHandle(this Window window)
	{
		return new WindowInteropHelper(window).Handle;
	}
}
