using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using AccountsLibrary;
using CommonLibrary;

namespace AccountsManager;

public class WindowUser : Window, IComponentConnector
{
	private bool _contentLoaded;

	public OwnerAccount OwnerAccount { get; set; }

	private bool IsNew { get; set; }

	public WindowUser()
	{
		InitializeComponent();
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		OwnerAccount = new OwnerAccount();
		base.DataContext = OwnerAccount;
		IsNew = true;
	}

	public WindowUser(OwnerAccount ownerAccount)
	{
		InitializeComponent();
		base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		OwnerAccount = ownerAccount;
		base.DataContext = OwnerAccount;
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

	private void btnSave_Click(object sender, RoutedEventArgs e)
	{
		if (OwnerAccount.Name == "")
		{
			new WindowInfo("Не заполненны данные пользователя!").Show();
			return;
		}
		if (IsNew)
		{
			OwnersAccountBL.OwnersAccount.Add(OwnerAccount);
		}
		ViewModelMainWindow.UpdateOwners();
		Close();
	}

	private void btnCancel_Click(object sender, RoutedEventArgs e)
	{
		btnClose_Click(sender, e);
	}

	private void btnSetBackGround_Click(object sender, RoutedEventArgs e)
	{
		ColorDialog colorDialog = new ColorDialog();
		if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{
			OwnerAccount.BorderBrushMass = new byte[4]
			{
				colorDialog.Color.A,
				colorDialog.Color.R,
				colorDialog.Color.G,
				colorDialog.Color.B
			};
		}
	}

	private void btnGeneratePassword_Click(object sender, RoutedEventArgs e)
	{
		if (new WindowDialog
		{
			textBlockMessage = 
			{
				Text = "Сгенерировать новый пароль?"
			},
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
			Topmost = true
		}.ShowDialog().Value)
		{
			OwnerAccount.Password = RandomPassword.Generate();
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/_accountsmanager/windowuser.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
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
			((Grid)target).MouseDown += gridTitle_MouseDown;
			break;
		case 2:
			((System.Windows.Controls.Button)target).Click += btnHide_Click;
			break;
		case 3:
			((System.Windows.Controls.Button)target).Click += btnClose_Click;
			break;
		case 4:
			((System.Windows.Controls.Button)target).Click += btnSetBackGround_Click;
			break;
		case 5:
			((System.Windows.Controls.Button)target).Click += btnGeneratePassword_Click;
			break;
		case 6:
			((System.Windows.Controls.Button)target).Click += btnSave_Click;
			break;
		case 7:
			((System.Windows.Controls.Button)target).Click += btnCancel_Click;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
