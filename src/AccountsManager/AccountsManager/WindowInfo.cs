using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace AccountsManager;

public class WindowInfo : Window, IComponentConnector
{
	internal TextBlock textBlockAdminMessage;

	private bool _contentLoaded;

	public WindowInfo(string text)
	{
		InitializeComponent();
		base.Left = SystemParameters.WorkArea.Width / 2.0 - base.Width / 2.0;
		base.Top = SystemParameters.WorkArea.Height / 2.0;
		base.Topmost = true;
		textBlockAdminMessage.Text = text;
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void btnHide_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void btnWindow_Click(object sender, RoutedEventArgs e)
	{
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		Hide();
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/windowinfo.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Grid)target).MouseDown += gridTitle_MouseDown;
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
		case 5:
			textBlockAdminMessage = (TextBlock)target;
			break;
		case 6:
			((Button)target).Click += Button_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
