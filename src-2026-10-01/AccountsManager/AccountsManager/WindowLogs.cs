using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using AccountsLibrary;

namespace AccountsManager;

public class WindowLogs : Window, INotifyPropertyChanged, IComponentConnector
{
	internal ListBox listBoxMessages;

	private bool _contentLoaded;

	public AccountSettings Account { get; set; }

	public string TitleWindow { get; set; } = "";

	public CollectionViewSource cvsListMessages { get; set; }

	public List<string> Logs { get; set; } = new List<string>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public WindowLogs(AccountSettings account)
	{
		InitializeComponent();
		Account = account;
		TitleWindow = Account.NameBCAccount + " " + Account.Login;
		base.DataContext = this;
		base.Width = 1200.0;
		base.Height = SystemParameters.WorkArea.Height / 1.4;
		base.Left = 1350.0;
		base.Top = SystemParameters.WorkArea.Height - base.Height;
		cvsListMessages = FindResource("cvsListMessages") as CollectionViewSource;
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
		Hide();
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	public void OpenLogs(List<string> initLogs)
	{
		Logs = initLogs;
		RaisePropertyChanged("Logs");
		if (!base.IsVisible)
		{
			Show();
			base.WindowState = WindowState.Normal;
		}
		else
		{
			Hide();
		}
		cvsListMessages.View.Refresh();
		listBoxMessages.Items.MoveCurrentToLast();
		listBoxMessages.ScrollIntoView(listBoxMessages.Items.CurrentItem);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocater = new Uri("/AccountsManager;component/_accountsmanager/windowlogs.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
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
			listBoxMessages = (ListBox)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
