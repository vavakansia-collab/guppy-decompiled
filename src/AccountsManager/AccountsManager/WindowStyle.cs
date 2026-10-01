using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace AccountsManager;

public class WindowStyle : ResourceDictionary, IComponentConnector, IStyleConnector
{
	private bool _contentLoaded;

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		sender.ForWindowFromTemplate(delegate(Window w)
		{
			w.Close();
		});
	}

	private void btnHide_Click(object sender, RoutedEventArgs e)
	{
		sender.ForWindowFromTemplate(delegate(Window w)
		{
			w.WindowState = WindowState.Minimized;
		});
	}

	private void btnWindow_Click(object sender, RoutedEventArgs e)
	{
		sender.ForWindowFromTemplate(delegate(Window w)
		{
			w.WindowState = ((w.WindowState != WindowState.Maximized) ? WindowState.Maximized : WindowState.Normal);
		});
	}

	public void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ClickCount > 1)
		{
			btnWindow_Click(sender, e);
		}
		else if (e.LeftButton == MouseButtonState.Pressed)
		{
			sender.ForWindowFromTemplate(delegate(Window w)
			{
				w.DragMove();
			});
		}
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		Application.Current.Shutdown();
		Process.GetCurrentProcess().Kill();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/styles/customizedwindow/windowstyle.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		_contentLoaded = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Grid)target).MouseLeftButtonDown += gridTitle_MouseDown;
			break;
		case 2:
			((Button)target).Click += btnHide_Click;
			break;
		case 3:
			((Button)target).Click += btnWindow_Click;
			break;
		case 4:
			((Button)target).Click += btnClose_Click;
			break;
		}
	}
}
