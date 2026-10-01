using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using AccountsManager;

namespace SnapshotsViewer;

public class WindowSnapshotsViewer : Window, IComponentConnector
{
	internal Grid grid;

	internal RowDefinition rowBoard;

	internal Grid gridTitle;

	internal ListBox listBoxCompareMatches;

	private bool _contentLoaded;

	public WindowSnapshotsViewer()
	{
		InitializeComponent();
		base.Left = 10.0;
		base.Top = 10.0;
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AccountsManager;component/SnapshotsViewer/Resources/SnapshotsViewer.ico"));
	}

	private void btnHide_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void btnWindow_Click(object sender, RoutedEventArgs e)
	{
		if (base.WindowState != WindowState.Maximized)
		{
			base.WindowState = WindowState.Maximized;
		}
		else
		{
			base.WindowState = WindowState.Normal;
		}
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		WatsonClientExchange.Disconnect();
		Hide();
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	public static bool ShowWindowDialog(string text)
	{
		return new WindowDialog
		{
			textBlockMessage = 
			{
				Text = text
			},
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
			Topmost = true
		}.ShowDialog().Value;
	}

	private void SearchAccountsByText_KeyDown(object sender, KeyEventArgs e)
	{
		VMSnapshotsViewer.instance.OnSearchTextChanged();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/snapshotsviewer/windowsnapshotsviewer.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			grid = (Grid)target;
			break;
		case 2:
			rowBoard = (RowDefinition)target;
			break;
		case 3:
			gridTitle = (Grid)target;
			gridTitle.MouseDown += gridTitle_MouseDown;
			break;
		case 4:
			((Button)target).Click += btnHide_Click;
			break;
		case 5:
			((Button)target).Click += btnWindow_Click;
			break;
		case 6:
			((Button)target).Click += btnClose_Click;
			break;
		case 7:
			((TextBox)target).KeyDown += SearchAccountsByText_KeyDown;
			break;
		case 8:
			listBoxCompareMatches = (ListBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
