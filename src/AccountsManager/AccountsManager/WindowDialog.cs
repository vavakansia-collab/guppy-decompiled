using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace AccountsManager;

public class WindowDialog : Window, IComponentConnector
{
	private bool IsTimerWondow;

	internal TextBlock textBlockMessage;

	internal Button btnYes;

	private bool _contentLoaded;

	public bool IsClickYes { get; set; }

	public bool IsClickNO { get; set; }

	public WindowDialog(bool isTimerWindow = false)
	{
		InitializeComponent();
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		base.Topmost = true;
		IsTimerWondow = isTimerWindow;
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
		IsClickNO = true;
		Hide();
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	private void btnYes_Click(object sender, RoutedEventArgs e)
	{
		IsClickYes = true;
		if (!IsTimerWondow)
		{
			base.DialogResult = true;
		}
		Hide();
	}

	private void btnNo_Click(object sender, RoutedEventArgs e)
	{
		IsClickNO = true;
		if (!IsTimerWondow)
		{
			base.DialogResult = false;
		}
		Hide();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/windowdialog.xaml", UriKind.Relative);
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
			textBlockMessage = (TextBlock)target;
			break;
		case 6:
			btnYes = (Button)target;
			btnYes.Click += btnYes_Click;
			break;
		case 7:
			((Button)target).Click += btnNo_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
