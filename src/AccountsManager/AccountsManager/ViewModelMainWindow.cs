using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using AccountsLibrary;
using AccountsManager.DropCoef;
using AccountsManager.Properties;
using BookmakerLibrary;
using CommonLibrary;
using FluentFTP;
using SnapshotsViewer;

namespace AccountsManager;

public class ViewModelMainWindow : INotifyPropertyChanged
{
	private const string FTP_ADDRESS = "31.31.196.215";

	private const int FTP_PORT = 21;

	private const string FTP_USERNAME = "u0377646_guppy";

	private const string FTP_PASSWORD = "Sto9cm19by50!";

	private static readonly string TIMESTAMPPATH = Environment.CurrentDirectory + "/timestamp";

	public static ViewModelMainWindow instance = null;

	private OwnerAccount mainOwner;

	private OwnerAccount currentOwner = new OwnerAccount();

	private bool isConnectServer;

	private CollectionViewSource cvsAccounts;

	private CollectionViewSource cvsBets;

	private EnumStatusUI statusUI;

	private RelayCommand addBookmakerCommand;

	private RelayCommand putAccountStartCommand;

	private RelayCommand putAccountStopCommand;

	private RelayCommand putAccountGetBalanceCommand;

	private RelayCommand clearMessagesCommand;

	private RelayCommand soundSwitch;

	private RelayCommand clearSearch;

	private RelayCommand cupisCommand;

	private RelayCommand proxyCommand;

	private RelayCommand openWindowOwnersCommand;

	private RelayCommand putAccountSetPoslegolStatusDefaultCommand;

	private RelayCommand putAccountSetValueStatusDefaultCommand;

	private RelayCommand putAccountSetForkStatusDefaultCommand;

	private RelayCommand putAccountSetValuePlusStatusDefaultCommand;

	private RelayCommand putAccountSetValueScoreStatusDefaultCommand;

	private RelayCommand saveCommand;

	private RelayCommand snapshotsViewerCommand;

	private RelayCommand dropCoefCommand;

	private RelayCommand selectionChangedAccount;

	private bool accountsStartedChecked = true;

	private bool accountsStopedChecked = true;

	private bool accountsCreatedChecked = true;

	private bool accountsReadyChecked = true;

	private bool accountsDelayChecked = true;

	private bool accountsoWorkedOutChecked = true;

	private bool statusBetsSuccess = true;

	private bool statusBetsFault = true;

	private bool statusBetsWin = true;

	private bool statusBetsLose = true;

	private bool statusBetsNotDefined = true;

	private double indicatorOpacity;

	private string searchText;

	private WindowProxy WindowProxy;

	private WindowUsers WindowUsers;

	private WindowSnapshotsViewer WindowSnapshotsViewer;

	private WindowDropCoef WindowDropCoef;

	private static FtpClient FtpClient;

	private static Dictionary<string, WindowAccountSettings> OpenAccountsSettings = new Dictionary<string, WindowAccountSettings>();

	private static List<Browser> OpenedBrowsers = new List<Browser>();

	private static bool IsLoaded = false;

	private static DateTime TimeStartApp = DateTime.Now;

	private DateTime LastTimeFormOwnerUpdated = DateTime.Now;

	public static string Version { get; set; } = "5.25";

	public ObservableCollection<AccountSettingsVM> AccountsSettingsVM { get; } = new ObservableCollection<AccountSettingsVM>();

	public ObservableCollection<ViewModelBetStrategyItem> BetStrategyItems { get; set; } = new ObservableCollection<ViewModelBetStrategyItem>();

	public static AMSettings AMSettings { get; set; }

	public OwnerAccount MainOwner
	{
		get
		{
			return mainOwner;
		}
		private set
		{
			if (mainOwner != null)
			{
				throw new ArgumentException("Владелец приложения не должене меняться.");
			}
			mainOwner = value;
		}
	}

	public OwnerAccount CurrentOwner
	{
		get
		{
			return currentOwner;
		}
		set
		{
			currentOwner = value;
			RaisePropertyChanged("CurrentOwner");
		}
	}

	public bool IsConnectServer
	{
		get
		{
			return isConnectServer;
		}
		set
		{
			isConnectServer = value;
			RaisePropertyChanged("IsConnectServer");
		}
	}

	public CollectionViewSource CVSAccounts
	{
		get
		{
			return cvsAccounts;
		}
		set
		{
			cvsAccounts = value;
			RaisePropertyChanged("CVSAccounts");
		}
	}

	public CollectionViewSource CVSBets
	{
		get
		{
			return cvsBets;
		}
		set
		{
			cvsBets = value;
			RaisePropertyChanged("CVSBets");
		}
	}

	private bool uiActive { get; set; } = true;

	public bool UIActive
	{
		get
		{
			return uiActive;
		}
		set
		{
			uiActive = value;
			RaisePropertyChanged("UIActive");
		}
	}

	public EnumStatusUI StatusUI
	{
		get
		{
			return statusUI;
		}
		set
		{
			statusUI = value;
			RaisePropertyChanged("StatusUI");
		}
	}

	public RelayCommand AddBookmakerCommand => addBookmakerCommand ?? (addBookmakerCommand = new RelayCommand(delegate(object obj)
	{
		Bookmaker bookmaker = (Bookmaker)obj;
		AddAccount(bookmaker);
	}));

	public RelayCommand PutAccountStartCommand => putAccountStartCommand ?? (putAccountStartCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		accountSettings.ProcessIndex = 1;
		PutAccountCommand(accountSettings, AccountCommandEnum.Start);
	}));

	public RelayCommand PutAccountStopCommand => putAccountStopCommand ?? (putAccountStopCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		accountSettings.ProcessIndex = 0;
		PutAccountCommand(accountSettings, AccountCommandEnum.Stop);
	}));

	public RelayCommand PutAccountGetBalanceCommand => putAccountGetBalanceCommand ?? (putAccountGetBalanceCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		PutAccountCommand(accountSettings, AccountCommandEnum.GetBalance);
	}));

	public RelayCommand ClearMessagesCommand => clearMessagesCommand ?? (clearMessagesCommand = new RelayCommand(delegate
	{
		PutAccountCommand(null, AccountCommandEnum.ClearMessages);
	}));

	public RelayCommand SoundSwitch => soundSwitch ?? (soundSwitch = new RelayCommand(delegate
	{
		Settings.AMSettings.SoundPlay = !Settings.AMSettings.SoundPlay;
	}));

	public RelayCommand ClearSearch => clearSearch ?? (clearSearch = new RelayCommand(delegate
	{
		if (!string.IsNullOrEmpty(SearchText))
		{
			SearchText = "";
			LastTimeFormOwnerUpdated = DateTime.Now;
			LoadAccountsAsync();
			UpdateStatesAsync();
			SetStatus(EnumStatusUI.IsReady);
		}
	}));

	public RelayCommand CupisCommand => cupisCommand ?? (cupisCommand = new RelayCommand(delegate
	{
		OnOpenCupisDatabase();
	}));

	public RelayCommand ProxyCommand => proxyCommand ?? (proxyCommand = new RelayCommand(delegate
	{
		OnOpenWindowProxy();
	}));

	public RelayCommand OpenWindowOwnersCommand => openWindowOwnersCommand ?? (openWindowOwnersCommand = new RelayCommand(delegate
	{
		OnOpenWindowOwners();
	}));

	public RelayCommand PutAccountSetPoslegolStatusDefaultCommand => putAccountSetPoslegolStatusDefaultCommand ?? (putAccountSetPoslegolStatusDefaultCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		PutAccountCommand(accountSettings, AccountCommandEnum.SetPoslegolStatusDefault);
	}));

	public RelayCommand PutAccountSetValueStatusDefaultCommand => putAccountSetValueStatusDefaultCommand ?? (putAccountSetValueStatusDefaultCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		PutAccountCommand(accountSettings, AccountCommandEnum.SetValueStatusDefault);
	}));

	public RelayCommand PutAccountSetForkStatusDefaultCommand => putAccountSetForkStatusDefaultCommand ?? (putAccountSetForkStatusDefaultCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		PutAccountCommand(accountSettings, AccountCommandEnum.SetForkStatusDefault);
	}));

	public RelayCommand PutAccountSetValuePlusStatusDefaultCommand => putAccountSetValuePlusStatusDefaultCommand ?? (putAccountSetValuePlusStatusDefaultCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		PutAccountCommand(accountSettings, AccountCommandEnum.SetValuePlusStatusDefault);
	}));

	public RelayCommand PutAccountSetValueScoreStatusDefaultCommand => putAccountSetValueScoreStatusDefaultCommand ?? (putAccountSetValueScoreStatusDefaultCommand = new RelayCommand(delegate(object obj)
	{
		AccountSettings accountSettings = (AccountSettings)obj;
		PutAccountCommand(accountSettings, AccountCommandEnum.SetValueScoreStatusDefault);
	}));

	public RelayCommand SaveCommand => saveCommand ?? (saveCommand = new RelayCommand(delegate
	{
		SaveApp();
	}));

	public RelayCommand SnapshotsViewerCommand => snapshotsViewerCommand ?? (snapshotsViewerCommand = new RelayCommand(delegate
	{
		OnOpenWindowSnapshotsViewer();
	}));

	public RelayCommand DropCoefCommand => dropCoefCommand ?? (dropCoefCommand = new RelayCommand(delegate
	{
		OnOpenWindowDropCoef();
	}));

	public RelayCommand SelectionChangedAccount => selectionChangedAccount ?? (selectionChangedAccount = new RelayCommand(delegate(object obj)
	{
		if ((obj as ListBox).SelectedValue is AccountSettingsVM accountSettingsVM)
		{
			if (AccountSettingsVM_Selected != null && AccountSettingsVM_Selected.AccountSettings.GUID.ToString() == accountSettingsVM.AccountSettings.GUID.ToString())
			{
				AccountSettingsVM_Selected = null;
			}
			else
			{
				AccountSettingsVM_Selected = accountSettingsVM;
			}
			RefreshBetsView();
		}
	}));

	public bool AccountsStartedChecked
	{
		get
		{
			return accountsStartedChecked;
		}
		set
		{
			accountsStartedChecked = value;
			RaisePropertyChanged("AccountsStartedChecked");
			RefreshView();
		}
	}

	public bool AccountsStopedChecked
	{
		get
		{
			return accountsStopedChecked;
		}
		set
		{
			accountsStopedChecked = value;
			RaisePropertyChanged("AccountsStopedChecked");
			RefreshView();
		}
	}

	public bool AccountsCreatedChecked
	{
		get
		{
			return accountsCreatedChecked;
		}
		set
		{
			accountsCreatedChecked = value;
			RaisePropertyChanged("AccountsCreatedChecked");
			RefreshView();
		}
	}

	public bool AccountsReadyChecked
	{
		get
		{
			return accountsReadyChecked;
		}
		set
		{
			accountsReadyChecked = value;
			RaisePropertyChanged("AccountsReadyChecked");
			RefreshView();
		}
	}

	public bool AccountsDelayChecked
	{
		get
		{
			return accountsDelayChecked;
		}
		set
		{
			accountsDelayChecked = value;
			RaisePropertyChanged("AccountsDelayChecked");
			RefreshView();
		}
	}

	public bool AccountsoWorkedOutChecked
	{
		get
		{
			return accountsoWorkedOutChecked;
		}
		set
		{
			accountsoWorkedOutChecked = value;
			RaisePropertyChanged("AccountsoWorkedOutChecked");
			RefreshView();
		}
	}

	public bool StatusBetsSuccess
	{
		get
		{
			return statusBetsSuccess;
		}
		set
		{
			statusBetsSuccess = value;
			RaisePropertyChanged("StatusBetsSuccess");
			RefreshBetsView();
		}
	}

	public bool StatusBetsFault
	{
		get
		{
			return statusBetsFault;
		}
		set
		{
			statusBetsFault = value;
			RaisePropertyChanged("StatusBetsFault");
			RefreshBetsView();
		}
	}

	public bool StatusBetsWin
	{
		get
		{
			return statusBetsWin;
		}
		set
		{
			statusBetsWin = value;
			RaisePropertyChanged("StatusBetsWin");
			RefreshBetsView();
		}
	}

	public bool StatusBetsLose
	{
		get
		{
			return statusBetsLose;
		}
		set
		{
			statusBetsLose = value;
			RaisePropertyChanged("StatusBetsLose");
			RefreshBetsView();
		}
	}

	public bool StatusBetsNotDefined
	{
		get
		{
			return statusBetsNotDefined;
		}
		set
		{
			statusBetsNotDefined = value;
			RaisePropertyChanged("StatusBetsNotDefined");
			RefreshBetsView();
		}
	}

	public double IndicatorOpacity
	{
		get
		{
			return indicatorOpacity;
		}
		set
		{
			indicatorOpacity = value;
			RaisePropertyChanged("IndicatorOpacity");
		}
	}

	public string SearchText
	{
		get
		{
			return searchText;
		}
		set
		{
			searchText = value;
			RaisePropertyChanged("SearchText");
		}
	}

	public static CupisDatabaseWindow CupisDatabaseWindow { get; set; }

	public List<Bookmaker> BookmakersCupis { get; set; } = new List<Bookmaker>();

	public List<Bookmaker> BookmakersOffshore { get; set; } = new List<Bookmaker>();

	public AccountSettingsVM AccountSettingsVM_Selected { get; set; }

	public static bool IsAppBlock { get; set; } = false;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public ViewModelMainWindow()
	{
		if (!CommonStaticFunction.IsAfalika() && !Environment.CurrentDirectory.Contains("Debug"))
		{
			FtpClient = new FtpClient("31.31.196.215", 21);
			FtpClient.Credentials = new NetworkCredential("u0377646_guppy", "Sto9cm19by50!");
			if (!IsUpdatedApp())
			{
				CheckUpdateApp();
				Process.GetCurrentProcess().Kill();
				Application.Current.Shutdown();
			}
			string fullPath = Path.GetFullPath(Environment.CurrentDirectory + "/Temp");
			if (Directory.Exists(fullPath))
			{
				Directory.Delete(fullPath, recursive: true);
			}
		}
		instance = this;
		CVSAccounts = new CollectionViewSource();
		CVSAccounts.Source = AccountsSettingsVM;
		CVSAccounts.GroupDescriptions.Add(new PropertyGroupDescription("AccountSettings.OwnerAccount"));
		CVSAccounts.GroupDescriptions.Add(new PropertyGroupDescription("AccountSettings.StatusRegistration"));
		CVSAccounts.SortDescriptions.Add(new SortDescription("AccountSettings.StatusRegistration", ListSortDirection.Ascending));
		CVSAccounts.SortDescriptions.Add(new SortDescription("AccountSettings.NameProvider", ListSortDirection.Ascending));
		CVSAccounts.SortDescriptions.Add(new SortDescription("AccountSettings.NameBCAccount", ListSortDirection.Ascending));
		CVSAccounts.SortDescriptions.Add(new SortDescription("AccountSettings.DateTimeCreate", ListSortDirection.Ascending));
		CVSAccounts.Filter += CollectionViewSource_Filter;
		CVSBets = new CollectionViewSource();
		CVSBets.Source = BetStrategyItems;
		CVSBets.SortDescriptions.Add(new SortDescription("BetStrategy.TimeCreate", ListSortDirection.Descending));
		CVSBets.Filter += CVSBets_Filter;
		BookmakersCupis.AddRange((from bc in BookmakersBL.Bookmakers
			where !string.IsNullOrEmpty(bc.NameCupis)
			orderby bc.Name
			select bc).ToList());
		BookmakersOffshore.AddRange((from bc in BookmakersBL.Bookmakers
			where string.IsNullOrEmpty(bc.NameCupis)
			orderby bc.Name
			select bc).ToList());
		Settings.ViewModelMainWindow = this;
		Settings.LoadApp();
		AMSettings = Settings.AMSettings;
		AccountsCreatedChecked = AMSettings.AccountsCreatedChecked;
		AccountsReadyChecked = AMSettings.AccountsReadyChecked;
		AccountsDelayChecked = AMSettings.AccountsDelayChecked;
		AccountsoWorkedOutChecked = AMSettings.AccountsoWorkedOutChecked;
		AccountsStartedChecked = AMSettings.AccountsStartedChecked;
		AccountsStopedChecked = AMSettings.AccountsStopedChecked;
		WindowAccountsManager.instance?.Hide();
		new AuthDialog().Show();
		WindowAccountsManager.instance?.Hide();
		AccountsCenterClient.Start();
		BettingCenterClient.Start();
		BettingCenterClient.OnServerBettingCenterConnected += BettingCenterClient_OnServerBettingCenterConnected;
	}

	private void BettingCenterClient_OnServerBettingCenterConnected()
	{
		UpdateStatesAsync();
	}

	public async void Run()
	{
		TimeStartApp = DateTime.Now;
		BlockUI(EnumStatusUI.IsStarting);
		double minWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width / 100.0 * 10.0;
		double minHeight = SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height / 100.0 * 10.0;
		WindowAccountsManager.instance.Left = Math.Min(Settings.AMSettings.WindowStartUpLeft, minWidth);
		WindowAccountsManager.instance.Top = Math.Min(Settings.AMSettings.WindowStartUpTop, minHeight);
		WindowAccountsManager.instance.Width = Settings.AMSettings.WindowStartUpWidth;
		WindowAccountsManager.instance.Height = Settings.AMSettings.WindowStartUpHeight;
		Browser.KillDeadProcesses();
		RSS.RunRSS(WindowAccountsManager.instance);
		MainOwner = AMSettings.OwnerAccounts;
		CurrentOwner = AMSettings.OwnerAccounts;
		SetStatus(EnumStatusUI.LoadingStaticData);
		await AccountsCenterClient.LoadStaticData();
		await AccountsCenterClient.OwnersRequestAsync();
		foreach (OwnerAccount item in OwnersAccountBL.OwnersAccount)
		{
			if (!AMSettings.VisibilityOwners.ContainsKey(item.GUID.ToString()))
			{
				AMSettings.VisibilityOwners.Add(item.GUID.ToString(), MainOwner.GUID.ToString() == item.GUID.ToString());
			}
		}
		await LoadAccountsAsync();
		await UpdateStatesAsync();
		ChangesBL.Start();
		AccountsSettingsBL.OnAccountSettingChanged += ChangesBL.AccountsSettingsBL_OnAccountSettingChanged;
		AccountsSettingsBL.OnUpdateStatusRegistration += AccountsSettingsBL_OnUpdateStatusRegistration;
		ChangesBL.OnSendChanges = (Action)Delegate.Combine(ChangesBL.OnSendChanges, (Action)delegate
		{
			AccountsCenterClient.SendChanges();
		});
		ChangesBL.OnChangesReceived += ChangesBL_OnChangesReceived;
		ChangesBL.OnRemoveChangesReceived += ChangesBL_OnRemoveChangesReceived;
		ChangesBL.OnSendChangedStarted += delegate
		{
			BlockUI();
		};
		ChangesBL.OnSendChangedComplite += delegate
		{
			UnblockUI();
		};
		Task.Run(delegate
		{
			if (FtpClient == null)
			{
				return;
			}
			while (true)
			{
				Thread.Sleep(60000);
				CheckUpdateApp();
			}
		});
		UnblockUI();
		StartBlinkingIndicator();
		if (Settings.AMSettings.SoundPlay)
		{
			Task.Run(delegate
			{
				SoundPlayer soundPlayer = new SoundPlayer(Resources.start0);
				soundPlayer.Play();
				soundPlayer.Dispose();
			});
		}
		if (AMSettings.OwnerAccounts.RuleCupis)
		{
			CupisDatabaseWindow = new CupisDatabaseWindow();
			CupisDatabaseWindow.Left = Math.Min(Settings.AMSettings.WindowCupisDatabaseStartUpLeft, minWidth);
			CupisDatabaseWindow.Top = Math.Min(Settings.AMSettings.WindowCupisDatabaseStartUpTop, minHeight);
			CupisDatabaseWindow.Width = Settings.AMSettings.WindowCupisDatabaseStartUpWidth;
			CupisDatabaseWindow.Height = Settings.AMSettings.WindowCupisDatabaseStartUpHeight;
		}
		RefreshView();
	}

	private void AccountsSettingsBL_OnUpdateStatusRegistration(AccountSettings accountSettings)
	{
		if (accountSettings.StatusRegistration == StatusesRegistration.Оформлен && (string.IsNullOrEmpty(accountSettings.Login) || string.IsNullOrEmpty(accountSettings.Password)))
		{
			ShowDialog("Внимание! Не заполнены поля \"Логин\" или \"Пароль\"");
		}
		RefreshView();
	}

	private void ChangesBL_OnChangesReceived(AccountSettings accountSettings)
	{
		WindowAccountsManager.instance.Dispatcher.Invoke(delegate
		{
			if (AccountsSettingsVM.SingleOrDefault((AccountSettingsVM ac) => ac.AccountSettings.GUID.ToString() == accountSettings.GUID.ToString()) == null)
			{
				AccountsSettingsVM.Add(new AccountSettingsVM(accountSettings));
				RefreshView();
			}
			bool flag = false;
			if (OpenAccountsSettings.TryGetValue(accountSettings.GUID.ToString(), out var value))
			{
				if (value.IsVisible)
				{
					flag = true;
				}
				OpenAccountsSettings.Remove(accountSettings.GUID.ToString());
				value.Close();
				value = null;
				value = new WindowAccountSettings(accountSettings);
				OpenAccountsSettings.Add(accountSettings.GUID.ToString(), value);
				if (flag)
				{
					value.Show();
				}
			}
		});
	}

	private void ChangesBL_OnRemoveChangesReceived(string guid)
	{
		if (OpenAccountsSettings.TryGetValue(guid, out var windowAccountSettings))
		{
			WindowAccountsManager.instance.Dispatcher.Invoke(delegate
			{
				OpenAccountsSettings.Remove(guid);
				windowAccountSettings.Close();
				windowAccountSettings = null;
			});
		}
	}

	public async Task<AccountSettings> AddAccount(Bookmaker bookmaker, string cupisLogin = "", string cupisPassword = "", string description = "", double moneyInput = 0.0, double moneyOutput = 0.0, string telegram = "")
	{
		Proxy initProxy = await GetAvabilityProxy(bookmaker.Name);
		AccountSettings accountSettings = new AccountSettings(bookmaker.Name, CurrentOwner, initProxy);
		accountSettings.Login = ((!string.IsNullOrEmpty(cupisLogin)) ? cupisLogin : "");
		accountSettings.Cupis.Login = ((!string.IsNullOrEmpty(cupisLogin)) ? cupisLogin : "");
		accountSettings.Comment = description;
		if (!string.IsNullOrEmpty(telegram))
		{
			AccountSettings accountSettings2 = accountSettings;
			accountSettings2.Discription = accountSettings2.Discription + "@" + telegram + Environment.NewLine;
		}
		AccountsSettingsBL.AddAccountSettings(accountSettings);
		if (AccountsSettingsVM.SingleOrDefault((AccountSettingsVM ac) => ac.AccountSettings.GUID.ToString() == accountSettings.GUID.ToString()) == null)
		{
			AccountsSettingsVM.Add(new AccountSettingsVM(accountSettings));
		}
		OpenAccountSettings(accountSettings);
		return accountSettings;
	}

	public void RemoveAccount(AccountSettings accountSettings)
	{
		AccountsSettingsBL.RemoveAccountSettings(accountSettings);
		ChangesBL.RemoveAccount(accountSettings);
		AccountsSettingsVM.Remove(AccountsSettingsVM.SingleOrDefault((AccountSettingsVM ac) => ac.AccountSettings.GUID.ToString() == accountSettings.GUID.ToString()));
		if (OpenAccountsSettings.ContainsKey(accountSettings.GUID.ToString()))
		{
			OpenAccountsSettings[accountSettings.GUID.ToString()].Close();
			OpenAccountsSettings.Remove(accountSettings.GUID.ToString());
		}
	}

	public async Task<Proxy> GetAvabilityProxy(string nameBC)
	{
		if (CurrentOwner.Name != "Guppy")
		{
			return AccountsLibrary.ProxyBL.ListProxy.FirstOrDefault((Proxy pr) => pr.IsEmpty);
		}
		Proxy proxy = await AccountsCenterClient.GetAvabilityProxyAccountSyncRequestAsync(nameBC);
		if (proxy == null)
		{
			proxy = AccountsLibrary.ProxyBL.ListProxy.FirstOrDefault((Proxy pr) => pr.IsEmpty);
		}
		if (proxy == null)
		{
			return proxy;
		}
		if (proxy.IsEmpty)
		{
			ShowDialog("Нет свободного прокси для БК " + nameBC);
		}
		return proxy;
	}

	public void OnServerConnected()
	{
		if (!ViewModelAuth.instance.IsAuthorized)
		{
			return;
		}
		WindowAccountsManager.instance.Dispatcher.InvokeAsync((Func<Task>)async delegate
		{
			foreach (KeyValuePair<string, WindowAccountSettings> openAccountsSetting in OpenAccountsSettings)
			{
				openAccountsSetting.Value.Close();
			}
			OpenAccountsSettings.Clear();
			await AccountsCenterClient.LoadStaticData();
			await LoadAccountsAsync();
		});
		UnblockUI(EnumStatusUI.Connected);
	}

	public void OnServerDisconnected()
	{
		BlockUI(EnumStatusUI.Connecting);
		WindowAccountsManager.instance.Dispatcher.Invoke(delegate
		{
			foreach (KeyValuePair<string, WindowAccountSettings> openAccountsSetting in OpenAccountsSettings)
			{
				openAccountsSetting.Value.Close();
			}
			OpenAccountsSettings.Clear();
		});
	}

	public void OnServerDisconnectedDoubleEnterClient()
	{
		OnServerDisconnected();
		ShowDialog("Выполен вход под данным пользователем с другого места.");
	}

	private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
	{
		AccountSettingsVM accountSettingsVM = e.Item as AccountSettingsVM;
		OwnerAccount ownerAccountByGuid = OwnersAccountBL.GetOwnerAccountByGuid(accountSettingsVM.AccountSettings.OwnerAccount.GUID.ToString());
		bool value = true;
		if (ownerAccountByGuid != null)
		{
			AMSettings.VisibilityOwners.TryGetValue(ownerAccountByGuid.GUID.ToString(), out value);
		}
		if ((accountSettingsVM.AccountSettings.StatusRegistration == StatusesRegistration.Новый && !AccountsCreatedChecked) || (accountSettingsVM.AccountSettings.StatusRegistration == StatusesRegistration.Оформлен && !AccountsReadyChecked) || (accountSettingsVM.AccountSettings.StatusRegistration == StatusesRegistration.Отложен && !AccountsDelayChecked) || (accountSettingsVM.AccountSettings.StatusRegistration == StatusesRegistration.Отработан && !AccountsoWorkedOutChecked) || (accountSettingsVM.AccountSettings.ProcessIndex == 1 && !AccountsStartedChecked) || (accountSettingsVM.AccountSettings.ProcessIndex == 0 && !AccountsStopedChecked) || (ownerAccountByGuid != null && !value))
		{
			e.Accepted = false;
		}
		else
		{
			e.Accepted = true;
		}
	}

	private void CVSBets_Filter(object sender, FilterEventArgs e)
	{
		BetStrategy betStrategy = (e.Item as ViewModelBetStrategyItem).BetStrategy;
		if ((betStrategy.Bet.Success && !StatusBetsSuccess) || (!betStrategy.Bet.Success && !StatusBetsFault) || (betStrategy.Bet.TypeCalculationBet == TypesCalculationBet.Win && !StatusBetsWin) || (betStrategy.Bet.TypeCalculationBet == TypesCalculationBet.Lose && !StatusBetsLose) || ((betStrategy.Bet.TypeCalculationBet == TypesCalculationBet.NotCalculation || betStrategy.Bet.TypeCalculationBet == TypesCalculationBet.Unclear) && !StatusBetsNotDefined) || (AccountSettingsVM_Selected != null && AccountSettingsVM_Selected.AccountSettings.GUID.ToString() != betStrategy.AccountSettings.GUID.ToString()))
		{
			e.Accepted = false;
		}
		else
		{
			e.Accepted = true;
		}
	}

	public void RefreshView()
	{
		if (IsLoaded)
		{
			WindowAccountsManager.instance.Dispatcher.Invoke(delegate
			{
				CVSAccounts.View.Refresh();
			});
		}
		LastTimeFormOwnerUpdated = DateTime.Now;
	}

	public void RefreshBetsView()
	{
		CVSBets.View.Refresh();
	}

	public async Task LoadAccountsAsync()
	{
		BlockUI(EnumStatusUI.LoadingAccounts);
		IsLoaded = false;
		List<string> list = new List<string>();
		foreach (OwnerAccount item in OwnersAccountBL.OwnersAccount)
		{
			bool value = false;
			AMSettings.VisibilityOwners.TryGetValue(item.GUID.ToString(), out value);
			if (value)
			{
				list.Add(item.GUID.ToString());
			}
		}
		List<AccountSettings> newAccounts = await AccountsCenterClient.GetAccounts(list);
		WindowAccountsManager.instance.Dispatcher.Invoke(delegate
		{
			if (newAccounts != null && newAccounts.Count > 0)
			{
				AccountsSettingsVM.ToList().ForEach(delegate(AccountSettingsVM acVM)
				{
					AccountsSettingsBL.UnsubscribeAccount(acVM.AccountSettings);
				});
				AccountsSettingsVM.Clear();
				newAccounts.ForEach(delegate(AccountSettings ac)
				{
					AccountsSettingsBL.AddAccountSettings(ac, isReceived: true);
					AccountsSettingsVM.Add(new AccountSettingsVM(ac));
					ChangesBL_OnChangesReceived(ac);
				});
			}
			IsLoaded = true;
			UnblockUI(EnumStatusUI.LoadedAccounts);
		});
	}

	public async Task UpdateStatesAsync()
	{
		IsLoaded = false;
		await WindowAccountsManager.instance.Dispatcher.InvokeAsync((Func<Task>)async delegate
		{
			List<string> list = AccountsSettingsBL.AccountsSettings.Select((AccountSettings ac) => ac.GUID.ToString()).ToList();
			BetStrategyItems.Clear();
			if (list.Count > 0)
			{
				List<StateAccount> list2 = await BettingCenterClient.GetStatesAccountsAsync(list);
				if (list2 != null)
				{
					foreach (AccountSettingsVM item in AccountsSettingsVM)
					{
						item.StateVM.State.Bets.Clear();
						item.StateVM.State.Logs.Clear();
					}
					UpdateStatesAccounts(list2);
				}
			}
		});
		IsLoaded = true;
	}

	public void BlockUI(EnumStatusUI enumStatusUI = EnumStatusUI.Updating)
	{
		SetStatus(enumStatusUI);
		UIActive = false;
	}

	public void UnblockUI(EnumStatusUI enumStatusUI = EnumStatusUI.IsReady)
	{
		SetStatus(enumStatusUI);
		UIActive = true;
	}

	public static WindowAccountSettings OpenAccountSettings(AccountSettings accountSettings, bool readOnly = false)
	{
		WindowAccountSettings value = null;
		if (instance.CurrentOwner.Name == accountSettings.OwnerAccount.Name || (!string.IsNullOrEmpty(accountSettings.OwnerAccount.OwnerGuid) && instance.CurrentOwner.Name == OwnersAccountBL.GetOwnerAccountByGuid(accountSettings.OwnerAccount.OwnerGuid)?.Name) || AMSettings.OwnerAccounts.Name == "Guppy")
		{
			if (OpenAccountsSettings.TryGetValue(accountSettings.GUID.ToString(), out value))
			{
				if (value.IsVisible)
				{
					value.Hide();
				}
				else
				{
					value.Show();
				}
			}
			else
			{
				value = new WindowAccountSettings(accountSettings);
				if (readOnly)
				{
					value.UIBlock.Visibility = Visibility.Visible;
				}
				else
				{
					value.UIBlock.Visibility = Visibility.Hidden;
				}
				value.Show();
				OpenAccountsSettings.Add(accountSettings.GUID.ToString(), value);
			}
		}
		else
		{
			ShowDialog("Нет доступа к просмотру аккаунта.");
		}
		return value;
	}

	public static WindowAccountSettings GetAccountSettingsWindow(AccountSettings accountSettings)
	{
		OpenAccountsSettings.TryGetValue(accountSettings.GUID.ToString(), out var value);
		return value;
	}

	public void SetStatus(EnumStatusUI status)
	{
		StatusUI = status;
	}

	public void UpdateStatesAccounts(List<StateAccount> states)
	{
		Application.Current.Dispatcher.Invoke(delegate
		{
			try
			{
				TypeBet lastTypeBet = TypeBet._empty;
				foreach (StateAccount stateAccount in states)
				{
					if (CVSAccounts.View.Cast<AccountSettingsVM>().SingleOrDefault((AccountSettingsVM av) => av.AccountSettings.GUID.ToString() == stateAccount.GUIDAccount.ToString()) != null)
					{
						AccountSettingsVM accountSettingsVM = AccountsSettingsVM.SingleOrDefault((AccountSettingsVM ac) => ac.AccountSettings.GUID.ToString() == stateAccount.GUIDAccount.ToString());
						if (accountSettingsVM != null)
						{
							accountSettingsVM.StateVM.State.Balance = stateAccount.Balance;
							accountSettingsVM.StateVM.State.IsRunning = stateAccount.IsRunning;
							accountSettingsVM.StateVM.State.StatusService = stateAccount.StatusService;
							foreach (BetStrategy betStrategy in stateAccount.Bets)
							{
								if (accountSettingsVM.StateVM.State.Bets.Where((BetStrategy b) => b.Bet.GUID.ToString() == betStrategy.Bet.GUID.ToString()).Count() <= 0)
								{
									accountSettingsVM.StateVM.State.Bets.Add(betStrategy);
									BetStrategyItems.Add(new ViewModelBetStrategyItem(betStrategy));
									accountSettingsVM.StateVM.State.RaisePropertyChanged("Bets");
									if (betStrategy.Bet.Success && Settings.AMSettings.SoundPlay && DateTime.Now.Subtract(LastTimeFormOwnerUpdated).TotalSeconds > 10.0)
									{
										lastTypeBet = betStrategy.Bet.TypeBet;
									}
								}
							}
							foreach (string log in stateAccount.Logs)
							{
								accountSettingsVM.StateVM.State.Logs.Add(log);
							}
						}
					}
				}
				if (IsLoaded && lastTypeBet != TypeBet._empty && DateTime.Now.Subtract(TimeStartApp).TotalSeconds > 10.0)
				{
					Task.Run(delegate
					{
						SoundPlayer soundPlayer = new SoundPlayer(Resources.start0);
						soundPlayer = ((lastTypeBet != TypeBet.Poslegol) ? new SoundPlayer(Resources.ksilofon) : new SoundPlayer(Resources.poslegol));
						soundPlayer.Play();
						soundPlayer.Dispose();
					});
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [UpdateStatesAccounts] ERROR UpdateStatesAccounts " + ex.Message);
			}
		});
		if (false)
		{
			WindowAccountsManager.instance.Dispatcher.Invoke(delegate
			{
				RefreshView();
			});
		}
	}

	public static Browser GetBrowserAccount(AccountSettings accountSettings, bool isCupis = false)
	{
		Browser browser = OpenedBrowsers.SingleOrDefault((Browser br) => br.AccountSettings.GUID == accountSettings.GUID);
		if (browser != null && browser.Process != null && !browser.Process.HasExited)
		{
			return browser;
		}
		if (browser != null)
		{
			OpenedBrowsers.Remove(browser);
		}
		browser = new Browser(accountSettings);
		OpenedBrowsers.Add(browser);
		return browser;
	}

	public async void PutAccountCommand(AccountSettings accountSettings, AccountCommandEnum command)
	{
		if (MainOwner.IsBlocked)
		{
			new WindowInfo("Пользователь заблокирован.").Show();
		}
		else
		{
			if (command == AccountCommandEnum.ClearMessages && !WindowAccountsManager.ShowWindowDialog("Подтвердите очистку сообщений и ставок."))
			{
				return;
			}
			if (!BettingCenterClient.WatsonTcpClient.Connected)
			{
				new WindowInfo("Выполнение операции недоступно. Отсутствует связь с сервером ставок.").Show();
				return;
			}
			if (command == AccountCommandEnum.Start)
			{
				if ((accountSettings.Proxy == null || accountSettings.Proxy.IsEmpty) && MainOwner.Name != "Guppy")
				{
					new WindowInfo("Не указан прокси.").Show();
					return;
				}
				if (accountSettings.NameBCAccount == "_1Win" || accountSettings.NameBCAccount == "MelbetRU")
				{
					new WindowInfo("Нет технической возможности запуска этой БК").Show();
					return;
				}
				if (string.IsNullOrEmpty(accountSettings.Login) || string.IsNullOrEmpty(accountSettings.Password))
				{
					ShowDialog("Не заполнены поля \"Логин\" и \"Пароль\"");
					return;
				}
				if (accountSettings.StatusRegistration != StatusesRegistration.Оформлен)
				{
					ShowDialog("Запускать аккаунты можно только в статусе \"Оформлен\"");
					return;
				}
			}
			if (accountSettings != null && (command == AccountCommandEnum.Start || command == AccountCommandEnum.Stop))
			{
				accountSettings.Logs += $"{MainOwner.Name} {DateTime.Now} выполнение команды {command.ToString()}\r\n";
			}
			ChangesBL.SendChanges();
			Guid guidAccount = accountSettings?.GUID ?? Guid.Empty;
			await Task.Delay(3000);
			BettingCenterClient.PutCommand(new AccountCommand
			{
				Command = command,
				GuidAccount = guidAccount,
				OwnerName = CurrentOwner.Name
			});
		}
	}

	private void StartBlinkingIndicator()
	{
		IndicatorOpacity = 1.0;
		DoubleAnimation blinkAnimation = new DoubleAnimation
		{
			From = 1.0,
			To = 0.0,
			Duration = new Duration(TimeSpan.FromSeconds(0.5)),
			AutoReverse = true,
			RepeatBehavior = RepeatBehavior.Forever
		};
		blinkAnimation.Completed += delegate
		{
			IndicatorOpacity = 1.0;
			IndicatorOpacity = 0.0;
		};
		Application.Current.Dispatcher.BeginInvoke((Action)delegate
		{
			BlinkingAnimation(blinkAnimation);
		});
	}

	private async void BlinkingAnimation(DoubleAnimation animation)
	{
		while (true)
		{
			IndicatorOpacity = 1.0;
			await Task.Delay(1000);
			IndicatorOpacity = 0.0;
			await Task.Delay(1000);
		}
	}

	public static void ShowDialog(string text)
	{
		WindowAccountsManager.instance.Dispatcher.Invoke(() => new WindowInfo(text).ShowDialog());
	}

	public async void OnSearchTextChanged()
	{
		if (SearchText.Trim().Length < 4)
		{
			ShowDialog("Для поиска должно быть не менее 4-х символов.");
			return;
		}
		BlockUI(EnumStatusUI.Searching);
		LastTimeFormOwnerUpdated = DateTime.Now;
		List<AccountSettings> list = await AccountsCenterClient.SearchAccountsRequestAsync(SearchText);
		if (list != null && list.Count > 0)
		{
			AccountsSettingsVM.ToList().ForEach(delegate(AccountSettingsVM acVM)
			{
				AccountsSettingsBL.UnsubscribeAccount(acVM.AccountSettings);
			});
			AccountsSettingsVM.Clear();
			AccountsSettingsBL.ClearAccountsSettings();
			list.ForEach(delegate(AccountSettings asSearch)
			{
				AccountsSettingsBL.AddAccountSettings(asSearch, isReceived: true);
				AccountsSettingsVM.Add(new AccountSettingsVM(asSearch));
				ChangesBL_OnChangesReceived(asSearch);
			});
		}
		UpdateStatesAsync();
		UnblockUI();
	}

	private void OnOpenCupisDatabase()
	{
		Task.Run(delegate
		{
			if (!CupisDatabaseWindow.IsLoadedCupisDataBase)
			{
				SetStatus(EnumStatusUI.LoadingCupis);
				DateTime now = DateTime.Now;
				AccountsCenterClient.DatabaseRequest();
				while (!CupisDatabaseWindow.IsLoadedCupisDataBase && DateTime.Now.Subtract(now).TotalSeconds < 60.0)
				{
					Thread.Sleep(50);
				}
				while (!CupisDatabaseWindow.IsLoadedCupisDataBase)
				{
					Thread.Sleep(100);
				}
				SetStatus(EnumStatusUI.IsReady);
			}
			WindowAccountsManager.instance.Dispatcher.Invoke(delegate
			{
				if (!CupisDatabaseWindow.IsVisible)
				{
					CupisDatabaseWindow.Show();
				}
				else
				{
					CupisDatabaseWindow.Hide();
				}
			});
		});
	}

	private void OnOpenWindowProxy()
	{
		if (WindowProxy == null)
		{
			WindowProxy = new WindowProxy();
			WindowProxy.Show();
		}
		else if (!WindowProxy.IsVisible)
		{
			WindowProxy.Show();
		}
		else
		{
			WindowProxy.Hide();
		}
	}

	private void OnOpenWindowOwners()
	{
		if (WindowUsers == null)
		{
			WindowUsers = new WindowUsers();
			WindowUsers.Show();
		}
		else if (!WindowUsers.IsVisible)
		{
			WindowUsers.Show();
		}
		else
		{
			WindowUsers.Hide();
		}
	}

	public async void OnSwitchOwner(OwnerAccount ownerAccount)
	{
		LastTimeFormOwnerUpdated = DateTime.Now;
		if (CurrentOwner.GUID != ownerAccount.GUID)
		{
			CurrentOwner = ownerAccount;
			await LoadAccountsAsync();
		}
	}

	private void OnOpenWindowSnapshotsViewer()
	{
		if (WindowSnapshotsViewer == null)
		{
			WindowSnapshotsViewer = new WindowSnapshotsViewer();
			WindowSnapshotsViewer.Show();
		}
		else if (!WindowSnapshotsViewer.IsVisible)
		{
			WindowSnapshotsViewer.Show();
		}
		else
		{
			WindowSnapshotsViewer.Hide();
		}
	}

	private void OnOpenWindowDropCoef()
	{
		if (WindowDropCoef == null)
		{
			WindowDropCoef = new WindowDropCoef();
			WindowDropCoef.Show();
		}
		else if (!WindowDropCoef.IsVisible)
		{
			WindowDropCoef.Show();
		}
		else
		{
			WindowDropCoef.Hide();
		}
	}

	public void SaveApp()
	{
		AccountsCenterClient.SaveDatabaseRequest();
	}

	private bool IsUpdatedApp()
	{
		try
		{
			if (!File.Exists(TIMESTAMPPATH))
			{
				File.WriteAllText(TIMESTAMPPATH, "");
			}
			if (!FtpClient.IsConnected)
			{
				FtpClient.Connect();
			}
			if (!FtpClient.IsConnected)
			{
				Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [CheckUpdateApp] Нет соединения с FTP-сервером.");
				return false;
			}
			long ticks = FtpClient.GetModifiedTime("update.zip").Ticks;
			if (File.ReadAllText(TIMESTAMPPATH) == ticks.ToString())
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [CheckUpdateApp] Произошла ошибка при проверке обновления " + ex.Message);
		}
		return false;
	}

	private void CheckUpdateApp()
	{
		if (!File.Exists(TIMESTAMPPATH))
		{
			File.WriteAllText(TIMESTAMPPATH, "");
		}
		try
		{
			if (!FtpClient.IsConnected)
			{
				FtpClient.Connect();
			}
			if (!FtpClient.IsConnected)
			{
				Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [CheckUpdateApp] Нет соединения с FTP-сервером.");
				return;
			}
			long ticks = FtpClient.GetModifiedTime("update.zip").Ticks;
			if (File.ReadAllText(TIMESTAMPPATH) != ticks.ToString())
			{
				string fullPath = Path.GetFullPath(Environment.CurrentDirectory + "/AMUpdater");
				string fullPath2 = Path.GetFullPath(Environment.CurrentDirectory + "/Temp");
				CopyDirectory(fullPath, fullPath2);
				Process.Start(Environment.CurrentDirectory + "\\Temp\\AccountsManagerUpdater.exe");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [CheckUpdateApp] Произошла ошибка при проверке обновления " + ex.Message);
		}
	}

	private static void CopyDirectory(string from, string to, List<string> errors = null)
	{
		if (!Directory.Exists(to))
		{
			Directory.CreateDirectory(to);
		}
		foreach (string item in Directory.EnumerateFiles(from))
		{
			try
			{
				File.Copy(item, to + "/" + Path.GetFileName(item), overwrite: true);
			}
			catch (Exception ex)
			{
				errors?.Add("Can`t copy file " + item + Environment.NewLine + ex.Message);
			}
		}
		foreach (string item2 in Directory.EnumerateDirectories(from))
		{
			CopyDirectory(item2, to + "/" + Path.GetFileName(item2), errors);
		}
	}

	public static string GetAccountCookie(Guid guidAccount)
	{
		string result = null;
		AccountSettings accountSettings = instance.AccountsSettingsVM.SingleOrDefault((AccountSettingsVM acVM) => acVM.AccountSettings.GUID.ToString() == guidAccount.ToString()).AccountSettings;
		if (accountSettings != null)
		{
			result = LoadCookies(accountSettings);
		}
		return result;
	}

	public static string LoadCookies(AccountSettings accountSettings)
	{
		string path = Environment.CurrentDirectory + "\\WebCache\\_Chrome\\" + accountSettings.NameBCAccount + "\\" + accountSettings.GUID.ToString() + "/cookie_backup.txt";
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllText(path);
	}

	public static void UpdateOwners()
	{
		AccountsCenterClient.UpdateOwners();
		BettingCenterClient.UpdateOwners();
	}
}
