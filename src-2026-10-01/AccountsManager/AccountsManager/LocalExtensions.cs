using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace AccountsManager;

internal static class LocalExtensions
{
	public static void ForWindowFromChild(this object childDependencyObject, Action<Window> action)
	{
		DependencyObject element = childDependencyObject as DependencyObject;
		while (element != null)
		{
			element = VisualTreeHelper.GetParent(element);
			if (element is Window)
			{
				action(element as Window);
				break;
			}
		}
	}

	public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
	{
		if (((FrameworkElement)templateFrameworkElement).TemplatedParent is Window window)
		{
			action(window);
		}
	}

	public static IntPtr GetWindowHandle(this Window window)
	{
		return new WindowInteropHelper(window).Handle;
	}
}
