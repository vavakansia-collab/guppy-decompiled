using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using AccountsLibrary;

namespace AccountsManager;

public class ucAccount : System.Windows.Controls.UserControl, IComponentConnector
{
	private AccountSettingsVM AccountSettingsVM;

	private bool _contentLoaded;

	private WindowLogs windowLogs { get; set; }

	public ucAccount()
	{
		InitializeComponent();
	}

	private void ucAccountSettings_Loaded(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM = base.DataContext as AccountSettingsVM;
		windowLogs = new WindowLogs(AccountSettingsVM.AccountSettings);
	}

	private void btn_RunAccount(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		ViewModelMainWindow.instance.PutAccountStartCommand.Execute(accountSettingsVM.AccountSettings);
	}

	private void btn_StopAccount(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		ViewModelMainWindow.instance.PutAccountStopCommand.Execute(accountSettingsVM.AccountSettings);
	}

	private void btn_PauseAccount(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		accountSettingsVM.AccountSettings.IsPaused = !accountSettingsVM.AccountSettings.IsPaused;
		accountSettingsVM.AccountSettings.Logs += string.Format("{0} {1} изменение статуса \"на паузе\":{2}\r\n", ViewModelMainWindow.instance.MainOwner.Name, DateTime.Now, accountSettingsVM.AccountSettings.IsPaused ? "да" : "нет");
	}

	private void btnSetBackGround_Click(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		ColorDialog colorDialog = new ColorDialog();
		if (colorDialog.ShowDialog() == DialogResult.OK)
		{
			accountSettingsVM.AccountSettings.BorderBrushMass = new byte[4]
			{
				colorDialog.Color.A,
				colorDialog.Color.R,
				colorDialog.Color.G,
				colorDialog.Color.B
			};
		}
	}

	private void OnDeleteAccountClick(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		if (WindowAccountsManager.ShowWindowDialog("Подтвердите удаление аккаунта " + accountSettingsVM.AccountSettings.NameBCAccount + " " + accountSettingsVM.AccountSettings.Login))
		{
			ViewModelMainWindow.instance.RemoveAccount(accountSettingsVM.AccountSettings);
		}
	}

	private void OnRunBrowserAccountClick(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		ViewModelMainWindow.GetBrowserAccount(accountSettingsVM.AccountSettings).Navigate("https://" + accountSettingsVM.AccountSettings.Host);
	}

	private void OnRunBrowserCupis(object sender, RoutedEventArgs e)
	{
		ViewModelMainWindow.GetBrowserAccount(((AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext).AccountSettings).Navigate("https://wallet.1cupis.ru/main");
	}

	private void OnOpenAccountSettingsClick(object sender, RoutedEventArgs e)
	{
		ViewModelMainWindow.OpenAccountSettings(((AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext).AccountSettings);
	}

	private void OnStatusRegistrationSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if ((sender as System.Windows.Controls.ComboBox).IsLoaded && (StatusesRegistration)((System.Windows.Controls.ComboBox)sender).SelectedItem == StatusesRegistration.Оформлен)
		{
			AccountSettingsVM accountSettingsVM = (sender as System.Windows.Controls.ComboBox).DataContext as AccountSettingsVM;
			if (accountSettingsVM.AccountSettings.Proxy != null && accountSettingsVM.AccountSettings.Proxy.IsEmpty)
			{
				new WindowInfo("Внимание! Не указан прокси.").ShowDialog();
			}
		}
	}

	private void btnGetBalance_Click(object sender, RoutedEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = (AccountSettingsVM)((System.Windows.Controls.Button)sender).DataContext;
		ViewModelMainWindow.instance.PutAccountGetBalanceCommand.Execute(accountSettingsVM.AccountSettings);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/_accountsmanager/ucaccount.xaml", UriKind.Relative);
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
			((ucAccount)target).Loaded += ucAccountSettings_Loaded;
			break;
		case 2:
			((System.Windows.Controls.Button)target).Click += OnOpenAccountSettingsClick;
			break;
		case 3:
			((System.Windows.Controls.Button)target).Click += OnRunBrowserAccountClick;
			break;
		case 4:
			((System.Windows.Controls.Button)target).Click += btn_RunAccount;
			break;
		case 5:
			((System.Windows.Controls.Button)target).Click += btn_StopAccount;
			break;
		case 6:
			((System.Windows.Controls.Button)target).Click += btn_PauseAccount;
			break;
		case 7:
			((System.Windows.Controls.Button)target).Click += btnGetBalance_Click;
			break;
		case 8:
			((System.Windows.Controls.Button)target).Click += btnSetBackGround_Click;
			break;
		case 9:
			((System.Windows.Controls.Button)target).Click += OnDeleteAccountClick;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
