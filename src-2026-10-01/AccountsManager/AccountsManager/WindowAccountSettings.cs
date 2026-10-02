using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AccountsLibrary;
using AccountsLibrary.CUPIS;
using BookmakerLibrary;
using CommonLibrary;

namespace AccountsManager;

public class WindowAccountSettings : Window, INotifyPropertyChanged, IComponentConnector
{
	public static WindowAccountSettings instance;

	private AccountSettings AccountSettings;

	private string accountLogin;

	private string accountPassword;

	private string cupisLogin;

	private string cupisPassword;

	internal WindowAccountSettings windowAccountSettings;

	internal ScaleTransform transform0;

	internal ScaleTransform transform4;

	internal Grid UIBlock;

	private bool _contentLoaded;

	public OwnerAccount CurrentOwner { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public WindowAccountSettings(AccountSettings account)
	{
		InitializeComponent();
		instance = this;
		AccountSettings = account;
		base.Left = WindowAccountsManager.instance.Left + 150.0;
		base.Top = WindowAccountsManager.instance.Top + 200.0;
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + AccountSettings.NameBCAccount + ".jpg"));
		base.DataContext = new ViewModelAccountSettings(account);
		CurrentOwner = ViewModelMainWindow.instance.CurrentOwner;
		accountLogin = account.Login;
		accountPassword = account.Password;
		cupisLogin = account.Cupis.Login;
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
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	private void btnGeneratePassword_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings.Password = RandomPassword.Generate();
		if (accountPassword == "")
		{
			AccountSettings.Logs += $"{AccountSettings.OwnerAccount.Name} {DateTime.Now} установил пароль аккаунта на {AccountSettings.Password}\r\n";
		}
		else
		{
			AccountSettings.Logs += $"{AccountSettings.OwnerAccount.Name} {DateTime.Now} сменил пароль аккаунта с {accountPassword} на {AccountSettings.Password}\r\n";
		}
		accountPassword = AccountSettings.Password;
	}

	private void btn_RunBrowser(object sender, MouseButtonEventArgs e)
	{
		if (AccountSettings.Host == "tennisi.bet")
		{
			string username = HttpUtility.UrlEncode(AccountSettings.Login);
			string password = HttpUtility.UrlEncode(AccountSettings.Password);
			string url = "https://" + username + ":" + password + "@tennisi.bet/rt/cgi/!rt_member.Welcome?gameid=5&lang=rus&categoryid=&eventid=";
			ViewModelMainWindow.GetBrowserAccount(AccountSettings).Navigate(url);
		}
		else
		{
			ViewModelMainWindow.GetBrowserAccount(AccountSettings).Navigate("https://" + AccountSettings.Host);
		}
	}

	private void btn_RunBrowserCupis(object sender, MouseButtonEventArgs e)
	{
		ViewModelMainWindow.GetBrowserAccount(AccountSettings, isCupis: true).Navigate("https://wallet.1cupis.ru/main");
	}

	private void btnGetRandomUserAgent_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings.UserAgent = (AccountSettings.IsMobileUserAgent ? CommonStaticFunction.GetRandomUserAgentMobile() : CommonStaticFunction.GetRandomUserAgent());
	}

	private void OnAccountLoginTextboxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
	}

	private void OnAccountPasswordTextboxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
	}

	private void OnCupisLoginTextboxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
	}

	private void OnCupisPasswordTextboxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
	}

	private void OnAccountLoginTextboxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		TextBox textBox = (TextBox)sender;
		string text = textBox.Text;
		if (text.Replace(" ", "") == accountLogin.Replace(" ", "") || text.Trim().Length < 3 || (accountLogin.Length == 11 && accountLogin.Remove(2) == "89" && ("+79" + accountLogin.Remove(0, 2)).Replace(" ", "") == text.Replace(" ", "")))
		{
			return;
		}
		BlockUI();
		Task.Run(async delegate
		{
			if (text != accountLogin)
			{
				if (accountLogin == "")
				{
					AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} установил логин аккаунта на {AccountSettings.Login}\r\n";
				}
				else
				{
					AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} сменил логин аккаунта с {accountLogin} на {text}\r\n";
				}
				accountLogin = text;
				ChangesBL.AccountsSettingsBL_OnAccountSettingChanged(AccountSettings);
			}
		});
		UnblockUI();
	}

	private void OnAccountPasswordTextboxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		TextBox textBox = (TextBox)sender;
		if (textBox.Text != accountPassword)
		{
			if (accountPassword == "")
			{
				AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} установил пароль аккаунта на {AccountSettings.Password}\r\n";
			}
			else
			{
				AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} сменил пароль аккаунта с {accountPassword} на {AccountSettings.Password}\r\n";
			}
			accountPassword = textBox.Text;
			ChangesBL.AccountsSettingsBL_OnAccountSettingChanged(AccountSettings);
		}
	}

	private void OnCupisLoginTextboxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		TextBox textBox = (TextBox)sender;
		if (textBox.Text != cupisLogin)
		{
			if (cupisLogin == "")
			{
				AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} установил логин ЦУПИС на {textBox.Text}\r\n";
			}
			else
			{
				AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} сменил логин ЦУПИС с {cupisLogin} на {textBox.Text}\r\n";
			}
			cupisLogin = textBox.Text;
			ChangesBL.AccountsSettingsBL_OnAccountSettingChanged(AccountSettings);
		}
	}

	private async void btnSetNewProxy_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings accountSettings = AccountSettings;
		accountSettings.Proxy = await ViewModelMainWindow.instance.GetAvabilityProxy(AccountSettings.NameBCAccount);
	}

	private void OnCupisPasswordTextboxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		TextBox textBox = (TextBox)sender;
		if (textBox.Text != cupisPassword)
		{
			if (cupisPassword == "")
			{
				AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} установил пароль ЦУПИС на {textBox.Text}\r\n";
			}
			else
			{
				AccountSettings.Logs += $"{Settings.AMSettings.OwnerAccounts.Name} {DateTime.Now} сменил пароль ЦУПИС с {cupisPassword} на {textBox.Text}\r\n";
			}
			cupisPassword = textBox.Text;
			ChangesBL.AccountsSettingsBL_OnAccountSettingChanged(AccountSettings);
		}
	}

	private void OnTextDataChanged(object sender, TextCompositionEventArgs e)
	{
		ChangesBL.AccountsSettingsBL_OnAccountSettingChanged(AccountSettings);
	}

	public static void BlockUI()
	{
		instance.Dispatcher.Invoke(() => instance.UIBlock.Visibility = Visibility.Visible);
	}

	public static void UnblockUI()
	{
		instance.Dispatcher.Invoke(() => instance.UIBlock.Visibility = Visibility.Hidden);
	}

	private void OnLinkAccountButtonClick(object sender, RoutedEventArgs e)
	{
		CupisClient client = CupisDatabaseWindow.GetClients().FirstOrDefault((CupisClient x) => x.CupisLogin == AccountSettings.Cupis.Login);
		if (client == null)
		{
			return;
		}
		Bookmaker bookmaker = BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker bc) => bc.Name.ToUpper() == AccountSettings.NameBCAccount.ToUpper());
		if (bookmaker == null)
		{
			return;
		}
		CupisBookmakerStatistic statistic = null;
		foreach (CupisResponse cupisResponse in client.CupisResponses)
		{
			foreach (CupisBookmakerStatistic item2 in cupisResponse.Statistic)
			{
				if (item2.BookmakerCompany == bookmaker.NameCupis)
				{
					statistic = item2;
					break;
				}
			}
		}
		statistic.isHasAccount = true;
		CupisDatabaseWindow.GetRelatedAccounts(client).Add(AccountSettings);
	}

	private void btnRandomChooseAllSlowSports_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings.RandomSettings.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
	}

	private void btnRandomChooseCancelSlowSports_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings.RandomSettings.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
	}

	private void LoginTextBox_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && (e.Key == Key.C || e.Key == Key.Insert))
		{
			Clipboard.SetText(AccountSettings.Login.Replace(" ", "").Trim());
		}
	}

	private void btnAutoRandomBet(object sender, RoutedEventArgs e)
	{
		ViewModelMainWindow.instance.PutAccountCommand(AccountSettings, AccountCommandEnum.PutRandomBet);
	}

	private void btnRandomChooseAllFastSports_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings.RandomSettings.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
	}

	private void btnRandomChooseCancelFastSports_Click(object sender, RoutedEventArgs e)
	{
		AccountSettings.RandomSettings.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
	}

	private void btnRandomChoose_Click(object sender, RoutedEventArgs e)
	{
		bool isChecked = AccountSettings.RandomSettings.IssueTypeParameters.First().IsChecked;
		AccountSettings.RandomSettings.IssueTypeParameters.ToList().ForEach(delegate(IssueTypeChoose par)
		{
			par.IsChecked = !isChecked;
		});
	}

	private void btnEditUser(object sender, RoutedEventArgs e)
	{
		new WindowUser(AccountSettings.OwnerAccount).Show();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocater = new Uri("/AccountsManager;component/_accountsmanager/windowaccountsettings.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
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
			windowAccountSettings = (WindowAccountSettings)target;
			break;
		case 2:
			((Grid)target).MouseDown += gridTitle_MouseDown;
			break;
		case 3:
			((Button)target).Click += btnHide_Click;
			break;
		case 4:
			((Button)target).Click += btnClose_Click;
			break;
		case 5:
			((Image)target).MouseLeftButtonDown += btn_RunBrowser;
			break;
		case 6:
			transform0 = (ScaleTransform)target;
			break;
		case 7:
			((TextBox)target).LostKeyboardFocus += OnAccountLoginTextboxLostKeyboardFocus;
			((TextBox)target).KeyUp += LoginTextBox_KeyUp;
			break;
		case 8:
			((TextBox)target).LostKeyboardFocus += OnAccountPasswordTextboxLostKeyboardFocus;
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 9:
			((Button)target).Click += btnGeneratePassword_Click;
			break;
		case 10:
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 11:
			((TextBox)target).GotKeyboardFocus += OnCupisLoginTextboxGotKeyboardFocus;
			((TextBox)target).LostKeyboardFocus += OnCupisLoginTextboxLostKeyboardFocus;
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 12:
			((Image)target).MouseLeftButtonDown += btn_RunBrowserCupis;
			break;
		case 13:
			transform4 = (ScaleTransform)target;
			break;
		case 14:
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 15:
			((Button)target).Click += btnEditUser;
			break;
		case 16:
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 17:
			((Button)target).Click += btnSetNewProxy_Click;
			break;
		case 18:
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 19:
			((TextBox)target).PreviewTextInput += OnTextDataChanged;
			break;
		case 20:
			((Button)target).Click += btnGetRandomUserAgent_Click;
			break;
		case 21:
			((Button)target).Click += btnAutoRandomBet;
			break;
		case 22:
			((Button)target).Click += btnRandomChooseAllSlowSports_Click;
			break;
		case 23:
			((Button)target).Click += btnRandomChooseCancelSlowSports_Click;
			break;
		case 24:
			((Button)target).Click += btnRandomChooseAllFastSports_Click;
			break;
		case 25:
			((Button)target).Click += btnRandomChooseCancelFastSports_Click;
			break;
		case 26:
			((Button)target).Click += btnRandomChoose_Click;
			break;
		case 27:
			UIBlock = (Grid)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
