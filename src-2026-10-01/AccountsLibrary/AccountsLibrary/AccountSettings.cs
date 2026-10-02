using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using CommonLibrary;
using Newtonsoft.Json;

namespace AccountsLibrary;

public class AccountSettings : INotifyPropertyChanged
{
	private DateTime dateTimeChanged;

	private bool isCheckedPoslegol = false;

	private bool isCheckedValue = false;

	private bool isCheckedFork = false;

	private bool isCheckedValuePlus = false;

	private bool isCheckedValueScore = false;

	private OwnerAccount ownerAccount;

	private string nameBCAccount;

	[IgnoreDataMember]
	private string imgSource;

	private string host;

	private string login = "";

	private string password = "";

	private string userAgent;

	private Proxy proxy;

	private Currency currency;

	private bool isPaused = false;

	private int processIndex = 0;

	private string discription = "";

	private string comment = "";

	private string logs = "";

	private string note = "";

	private byte[] borderBrushMass = new byte[4];

	private bool enableAutoFillingBrowser = true;

	private bool isDetailedFinance = false;

	private bool isMobileUserAgent = false;

	private StatusesRegistration statusRegistration;

	private string scriptBrowser;

	private string notification = "";

	private bool isClearCookieBrowser = true;

	private int prioritet = 0;

	private bool uniqueBets = true;

	private bool takeSecondHand = false;

	private bool checkProxyOnStart = false;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public DateTime DateTimeCreate { get; set; } = DateTime.Now;

	public DateTime DateTimeChanged
	{
		get
		{
			return dateTimeChanged;
		}
		set
		{
			dateTimeChanged = value;
			RaisePropertyChanged("DateTimeChanged");
		}
	}

	public string NameProvider { get; set; }

	public Cupis Cupis { get; set; } = new Cupis();

	[IgnoreDataMember]
	public bool IsChanged { get; set; } = false;

	public bool IsCheckedPoslegol
	{
		get
		{
			return isCheckedPoslegol;
		}
		set
		{
			isCheckedPoslegol = value;
			RaisePropertyChanged("IsCheckedPoslegol");
		}
	}

	public bool IsCheckedValue
	{
		get
		{
			return isCheckedValue;
		}
		set
		{
			isCheckedValue = value;
			RaisePropertyChanged("IsCheckedValue");
		}
	}

	public bool IsCheckedFork
	{
		get
		{
			return isCheckedFork;
		}
		set
		{
			isCheckedFork = value;
			RaisePropertyChanged("IsCheckedFork");
		}
	}

	public bool IsCheckedValuePlus
	{
		get
		{
			return isCheckedValuePlus;
		}
		set
		{
			isCheckedValuePlus = value;
			RaisePropertyChanged("IsCheckedValuePlus");
		}
	}

	public bool IsCheckedValueScore
	{
		get
		{
			return isCheckedValueScore;
		}
		set
		{
			isCheckedValueScore = value;
			RaisePropertyChanged("IsCheckedValueScore");
		}
	}

	public RandomSettings RandomSettings { get; set; }

	public OwnerAccount OwnerAccount
	{
		get
		{
			return ownerAccount;
		}
		set
		{
			ownerAccount = value;
			RaisePropertyChanged("OwnerAccount");
		}
	}

	public string NameBCAccount
	{
		get
		{
			return nameBCAccount;
		}
		set
		{
			try
			{
				nameBCAccount = value;
				ImgSource = "pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/" + NameBCAccount + ".jpg";
				RaisePropertyChanged("NameBCAccount");
			}
			catch
			{
			}
		}
	}

	[IgnoreDataMember]
	public string ImgSource
	{
		get
		{
			return imgSource;
		}
		set
		{
			imgSource = value;
			RaisePropertyChanged("ImgSource");
		}
	}

	public ObservableCollection<ParameterAccount> Parameters { get; set; } = new ObservableCollection<ParameterAccount>();

	public ObservableCollection<ParameterAccount> ParametersSession { get; set; } = new ObservableCollection<ParameterAccount>();

	public string Host
	{
		get
		{
			return host;
		}
		set
		{
			host = value;
			RaisePropertyChanged("Host");
		}
	}

	public string Login
	{
		get
		{
			return login;
		}
		set
		{
			string oldValue = login;
			login = value.Replace(" ", "");
			if (!string.IsNullOrEmpty(value))
			{
				if (value.Length == 11 && (value.Remove(2) == "89" || value.Remove(2) == "79") && NameBCAccount != null && NameBCAccount != "OlimpCom" && !NameBCAccount.Contains("Betcity") && !NameBCAccount.Contains("Tennisi"))
				{
					login = "+7" + value.Remove(0, 1);
				}
				if (login.Length > 2 && login.Remove(2) == "+7")
				{
					login = login.Replace("(", "").Replace(")", "").Replace(" ", "")
						.Replace("-", "");
				}
			}
			this.OnAccountLoginChanged?.Invoke(oldValue, login);
			RaisePropertyChanged("Login");
		}
	}

	public string Password
	{
		get
		{
			return password;
		}
		set
		{
			string oldValue = password;
			password = value;
			this.OnAccountPasswordChanged?.Invoke(oldValue, password);
			RaisePropertyChanged("Password");
		}
	}

	public bool AuthorizationOnStart { get; set; } = true;

	public string UserAgent
	{
		get
		{
			return userAgent;
		}
		set
		{
			userAgent = value;
			RaisePropertyChanged("UserAgent");
		}
	}

	public Proxy Proxy
	{
		get
		{
			return proxy;
		}
		set
		{
			proxy = value;
			RaisePropertyChanged("Proxy");
		}
	}

	public Currency Currency
	{
		get
		{
			return currency;
		}
		set
		{
			currency = value;
			RaisePropertyChanged("Currency");
		}
	}

	public bool IsPaused
	{
		get
		{
			return isPaused;
		}
		set
		{
			isPaused = value;
			RaisePropertyChanged("IsPaused");
		}
	}

	public int ProcessIndex
	{
		get
		{
			return processIndex;
		}
		set
		{
			processIndex = value;
			RaisePropertyChanged("ProcessIndex");
		}
	}

	public string Discription
	{
		get
		{
			return discription;
		}
		set
		{
			discription = value;
			RaisePropertyChanged("Discription");
		}
	}

	public string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			comment = value;
			RaisePropertyChanged("Comment");
		}
	}

	public string Logs
	{
		get
		{
			return logs;
		}
		set
		{
			logs = value;
			RaisePropertyChanged("Logs");
		}
	}

	public string Note
	{
		get
		{
			return note;
		}
		set
		{
			note = value;
			RaisePropertyChanged("Note");
		}
	}

	public byte[] BorderBrushMass
	{
		get
		{
			return borderBrushMass;
		}
		set
		{
			borderBrushMass = value;
			RaisePropertyChanged("BorderBrushMass");
		}
	}

	public bool EnableAutoFillingBrowser
	{
		get
		{
			return enableAutoFillingBrowser;
		}
		set
		{
			enableAutoFillingBrowser = value;
			RaisePropertyChanged("EnableAutoFillingBrowser");
		}
	}

	public bool IsDetailedFinance
	{
		get
		{
			return isDetailedFinance;
		}
		set
		{
			isDetailedFinance = value;
			RaisePropertyChanged("IsDetailedFinance");
		}
	}

	public bool IsMobileUserAgent
	{
		get
		{
			return isMobileUserAgent;
		}
		set
		{
			isMobileUserAgent = value;
			RaisePropertyChanged("IsMobileUserAgent");
		}
	}

	public StatusesRegistration StatusRegistration
	{
		get
		{
			return statusRegistration;
		}
		set
		{
			StatusesRegistration oldValue = statusRegistration;
			statusRegistration = value;
			RaisePropertyChanged("StatusRegistration");
			this.OnUpdateStatusRegistration?.Invoke(this);
		}
	}

	public string ScriptBrowser
	{
		get
		{
			return scriptBrowser;
		}
		set
		{
			scriptBrowser = value;
			RaisePropertyChanged("ScriptBrowser");
		}
	}

	public string Notification
	{
		get
		{
			return notification;
		}
		set
		{
			foreach (char c in value)
			{
				if (!char.IsDigit(c) && c != ',')
				{
					return;
				}
			}
			notification = value;
			RaisePropertyChanged("Notification");
		}
	}

	public bool IsClearCookieBrowser
	{
		get
		{
			return isClearCookieBrowser;
		}
		set
		{
			isClearCookieBrowser = value;
			RaisePropertyChanged("IsClearCookieBrowser");
		}
	}

	public int Prioritet
	{
		get
		{
			return prioritet;
		}
		set
		{
			prioritet = value;
			RaisePropertyChanged("Prioritet");
		}
	}

	public bool UniqueBets
	{
		get
		{
			return uniqueBets;
		}
		set
		{
			uniqueBets = value;
			RaisePropertyChanged("UniqueBets");
		}
	}

	public bool TakeSecondHand
	{
		get
		{
			return takeSecondHand;
		}
		set
		{
			takeSecondHand = value;
			RaisePropertyChanged("TakeSecondHand");
		}
	}

	public bool CheckProxyOnStart
	{
		get
		{
			return checkProxyOnStart;
		}
		set
		{
			checkProxyOnStart = value;
			RaisePropertyChanged("CheckProxyOnStart");
		}
	}

	public ObservableCollection<PoslegolSettings> PoslegolsSettings { get; set; } = new ObservableCollection<PoslegolSettings>();

	public ObservableCollection<ValueSettings> ValuesSettings { get; set; } = new ObservableCollection<ValueSettings>();

	public ObservableCollection<ValuesPlusSettings> ValuesPlusSettings { get; set; } = new ObservableCollection<ValuesPlusSettings>();

	public ObservableCollection<ValueScoreSettings> ValuesScoreSettings { get; set; } = new ObservableCollection<ValueScoreSettings>();

	public ObservableCollection<ForkSettings> ForksSettings { get; set; } = new ObservableCollection<ForkSettings>();

	public event OnOpenSettingsHandler OnOpenSettings;

	public event OnUpdateAccountSettingsHandler OnUpdateAccount;

	public event OnUpdateAccountSettingsHandler OnUpdateStatusRegistration;

	public event PropertyChangedEventHandler PropertyChanged;

	public event OnTextFieldChangedHandler OnAccountLoginChanged;

	public event OnTextFieldChangedHandler OnAccountPasswordChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		UpdateAccount(propertyName);
	}

	public AccountSettings()
	{
	}

	public AccountSettings(string initNameBCOriginal, OwnerAccount initOwnerAccount, Proxy initProxy)
	{
		NameBCAccount = initNameBCOriginal;
		UserAgent = (IsMobileUserAgent ? CommonStaticFunction.GetRandomUserAgentMobile() : CommonStaticFunction.GetRandomUserAgent());
		if (NameBCAccount == "_1Win")
		{
			NameProvider = "_1Win";
			Host = "1wayrw.xyz";
		}
		else if (NameBCAccount == "_1xbet")
		{
			NameProvider = "_1xbet";
			Host = "1xbet.com";
		}
		else if (NameBCAccount == "_1xbit")
		{
			NameProvider = "_1xbet";
			Host = "1xbit.com";
		}
		else if (NameBCAccount == "_1xstavka")
		{
			NameProvider = "_1xbet";
			Host = "1xstavka.ru";
		}
		else if (NameBCAccount == "Betwinner")
		{
			NameProvider = "_1xbet";
			Host = "betwinner-4021277.top";
		}
		else if (NameBCAccount == "Melbet")
		{
			NameProvider = "_1xbet";
			Host = "melbet-751196.info";
		}
		else if (NameBCAccount == "BetBoom")
		{
			NameProvider = "BetBoom";
			Host = "betboom.ru";
		}
		else if (NameBCAccount == "MelbetRU")
		{
			NameProvider = "BetBoom";
			Host = "melbet.ru";
		}
		else if (NameBCAccount == "Marathon")
		{
			NameProvider = "Marathon";
			Host = "marathonbet.com";
		}
		else if (NameBCAccount == "MarathonRU")
		{
			NameProvider = "MarathonRU";
			Host = "marathonbet.ru";
		}
		else if (NameBCAccount == "Baltbet")
		{
			NameProvider = "BaltbetRU";
			Host = "baltbet.com";
		}
		else if (NameBCAccount == "BaltbetRU")
		{
			NameProvider = "BaltbetRU";
			Host = "baltbet.ru";
		}
		else if (NameBCAccount == "Betcity")
		{
			NameProvider = "Betcity";
			Host = "betcityru.com";
		}
		else if (NameBCAccount == "BetcityRU")
		{
			NameProvider = "Betcity";
			Host = "betcity.ru";
		}
		else if (NameBCAccount == "Parimatch")
		{
			NameProvider = "Parimatch";
			Host = "parimatch.com";
		}
		else if (NameBCAccount == "ParimatchRU")
		{
			NameProvider = "Parimatch";
			Host = "parimatch.ru";
		}
		else if (NameBCAccount == "Tennisi")
		{
			NameProvider = "Tennisi";
			Host = "tennisi.com";
		}
		else if (NameBCAccount == "TennisiBet")
		{
			NameProvider = "Tennisi";
			Host = "tennisi.bet";
		}
		else if (NameBCAccount == "BetteryRU")
		{
			NameProvider = "FonbetRU";
			Host = "bettery.ru";
		}
		else if (NameBCAccount == "FonbetCOM")
		{
			NameProvider = "FonbetRU";
			Host = "fonbet.com";
		}
		else if (NameBCAccount == "FonbetRU")
		{
			NameProvider = "FonbetRU";
			Host = "fon.bet";
		}
		else if (NameBCAccount == "ParibetRU")
		{
			NameProvider = "FonbetRU";
			Host = "pari.ru";
		}
		else if (NameBCAccount == "LigaStavokCom")
		{
			NameProvider = "LigaStavokRU";
			Host = "liga-st.com";
		}
		else if (NameBCAccount == "LigaStavokRU")
		{
			NameProvider = "LigaStavokRU";
			Host = "ligastavok.ru";
		}
		else if (NameBCAccount == "Leon")
		{
			NameProvider = "LeonRU";
			Host = "7054.info";
		}
		else if (NameBCAccount == "LeonRU")
		{
			NameProvider = "LeonRU";
			Host = "leon.ru";
		}
		else if (NameBCAccount == "OlimpCom")
		{
			NameProvider = "OlimpBet";
			Host = "olimp.com";
		}
		else if (NameBCAccount == "OlimpBet")
		{
			NameProvider = "OlimpBet";
			Host = "olimp.bet";
		}
		else if (NameBCAccount == "Winline")
		{
			NameProvider = "Winline";
			Host = "winline.ru";
		}
		else if (NameBCAccount == "Zenit")
		{
			NameProvider = "ZenitWin";
			Host = "zenitnow377.top";
		}
		else
		{
			if (!(NameBCAccount == "ZenitWin"))
			{
				return;
			}
			NameProvider = "ZenitWin";
			Host = "zenit.win";
		}
		OwnerAccount = initOwnerAccount;
		RandomSettings = new RandomSettings(this);
		Currency = CurrencyBL.Currencies.Single((Currency c) => c.NameEng == "RUB");
		if (!NameBCAccount.Contains("_1x") && !(NameBCAccount == "Melbet") && !(NameBCAccount == "Betwinner"))
		{
			if (NameBCAccount.Contains("BetBoom"))
			{
				IsClearCookieBrowser = false;
				PoslegolsSettings.LastOrDefault()?.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
				{
					sp.IsChecked = false;
				});
			}
			else if (NameBCAccount.Contains("MelbetRU"))
			{
				IsClearCookieBrowser = false;
				PoslegolsSettings.LastOrDefault()?.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
				{
					sp.IsChecked = false;
				});
			}
			else if (NameBCAccount.Contains("Baltbet"))
			{
				PoslegolsSettings.LastOrDefault()?.IssueTypeParameters.Where((IssueTypeParameter itp) => itp.IssueType.ToString().Contains("GetTennisGameTeam")).ToList().ForEach(delegate(IssueTypeParameter itp)
				{
					itp.Points1 = true;
				});
			}
			else if (NameBCAccount.Contains("Betcity"))
			{
				AddParameterAccount(new ParameterAccount
				{
					NameParameter = "dev",
					ValueParameter = "",
					IsNecessary = false
				});
			}
			else if (!(NameProvider == "FonbetRU") && !(NameBCAccount == "Parimatch") && !(NameBCAccount == "ParimatchRU"))
			{
				if (NameBCAccount.Contains("Tennisi"))
				{
					AddParameterAccount(new ParameterAccount
					{
						NameParameter = "btAKU",
						ValueParameter = "",
						IsNecessary = true
					});
				}
				else if (NameBCAccount.Contains("LigaStavok"))
				{
					UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
					AddParameterAccount(new ParameterAccount
					{
						NameParameter = "AccountNumber",
						ValueParameter = "",
						IsNecessary = false
					});
				}
				else if (!(NameBCAccount == "OlimpBet") && !(NameBCAccount == "Winline"))
				{
				}
			}
		}
		PoslegolSettings ps = CreatePoslegolSettings();
		ps.Name = "Послегол_Футбол";
		ps.BalancePercent = 100.0;
		ps.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
		ps.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
		ps.ParametersCheckBC.ToList().ForEach(delegate(ParameterAccount par)
		{
			par.IsChecked = false;
		});
		if (NameProvider != "Winline")
		{
			ps.ParametersCheckBC.Single((ParameterAccount par) => par.NameParameter == "Winline").IsChecked = true;
		}
		AddStrategySettingsBase(ps);
		PoslegolSettings ps2 = CreatePoslegolSettings();
		ps2.Name = "Послегол_Настольный";
		ps2.BalancePercent = 100.0;
		ps2.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
		ps2.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
		ps2.ParametersCheckBC.ToList().ForEach(delegate(ParameterAccount par)
		{
			par.IsChecked = false;
		});
		if (NameProvider != "Winline")
		{
			ps2.ParametersCheckBC.Single((ParameterAccount par) => par.NameParameter == "Winline").IsChecked = true;
		}
		ps2.ParametersCheckBC.Single((ParameterAccount par) => par.NameParameter == "SetkaCupProvider").IsChecked = true;
		AddStrategySettingsBase(ps2);
		ValueSettings val1 = CreateValueSettings();
		val1.Name = "Валуи_1Очко";
		val1.MaxMaxSum = 10000000.0;
		val1.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
		val1.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
		val1.SportsParameters.Single((ParametersSport sp) => sp.Sport.IdSport == 3).IsChecked = false;
		val1.SportsParameters.Single((ParametersSport sp) => sp.Sport.IdSport == 10).IsChecked = false;
		if (NameProvider != "_1xbet")
		{
			val1.ParametersCheckBC.SingleOrDefault((ParameterAccount par) => par.NameParameter == "_1xbet").IsChecked = false;
		}
		foreach (IssueTypeParameter issueTypeParameter in val1.IssueTypeParameters)
		{
			issueTypeParameter.Points0 = false;
			issueTypeParameter.Points1 = true;
			issueTypeParameter.Points2 = true;
			issueTypeParameter.Points2Over = true;
		}
		AddStrategySettingsBase(val1);
		ValueSettings val2 = CreateValueSettings();
		val2.Name = "Валуи_2Очка";
		val2.MaxMaxSum = 10000000.0;
		val2.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
		val2.SportsParameters.Where((ParametersSport sp) => !sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
		foreach (IssueTypeParameter issueTypeParameter2 in val2.IssueTypeParameters)
		{
			issueTypeParameter2.Points0 = false;
			issueTypeParameter2.Points1 = false;
			issueTypeParameter2.Points2 = true;
			issueTypeParameter2.Points2Over = true;
		}
		AddStrategySettingsBase(val2);
		ValuesPlusSettings valPlus1 = CreateValuePlusSettings();
		valPlus1.Name = "Валуи+";
		valPlus1.MaxMaxSum = 10000000.0;
		valPlus1.SportsParameters.Where((ParametersSport sp) => sp.Sport.IdSport != 3).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
		valPlus1.SportsParameters.Where((ParametersSport sp) => sp.Sport.IdSport == 3).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
		foreach (IssueTypeParameter issueTypeParameter3 in valPlus1.IssueTypeParameters)
		{
			issueTypeParameter3.Points0 = false;
			issueTypeParameter3.Points1 = false;
			issueTypeParameter3.Points2 = true;
			issueTypeParameter3.Points2Over = true;
		}
		AddStrategySettingsBase(valPlus1);
		ValuesPlusSettings valPlus2 = CreateValuePlusSettings();
		valPlus2.Name = "Валуи+_Баскетбол";
		valPlus2.MaxMaxSum = 10000000.0;
		valPlus2.SportsParameters.Where((ParametersSport sp) => sp.Sport.IdSport != 3).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = false;
		});
		valPlus2.SportsParameters.Where((ParametersSport sp) => sp.Sport.IdSport == 3).ToList().ForEach(delegate(ParametersSport sp)
		{
			sp.IsChecked = true;
		});
		foreach (IssueTypeParameter issueTypeParameter4 in valPlus2.IssueTypeParameters)
		{
			issueTypeParameter4.Points0 = false;
			issueTypeParameter4.Points1 = false;
			issueTypeParameter4.Points2 = true;
			issueTypeParameter4.Points2Over = true;
		}
		AddStrategySettingsBase(valPlus2);
		if (OwnerAccount.Name != "Guppy")
		{
			Prioritet = 1;
			UniqueBets = false;
			ValueSettings val3 = CreateValueSettings();
			val3.Name = "Валуи";
			val3.MaxMaxSum = 10000000.0;
			val3.SportsParameters.Where((ParametersSport sp) => sp.IdSport != 3 && sp.IdSport != 4).ToList().ForEach(delegate(ParametersSport sp)
			{
				sp.IsChecked = false;
			});
			val3.SportsParameters.Where((ParametersSport sp) => sp.IdSport == 3 || sp.IdSport == 4).ToList().ForEach(delegate(ParametersSport sp)
			{
				sp.IsChecked = true;
			});
			foreach (IssueTypeParameter issueTypeParameter5 in val3.IssueTypeParameters)
			{
				issueTypeParameter5.Points0 = false;
				issueTypeParameter5.Points1 = false;
				issueTypeParameter5.Points2 = true;
				issueTypeParameter5.Points2Over = true;
			}
			AddStrategySettingsBase(val3);
		}
		Color colorAccount = Color.LightGray;
		BorderBrushMass = new byte[4] { colorAccount.A, colorAccount.R, colorAccount.G, colorAccount.B };
		StatusRegistration = StatusesRegistration.Новый;
		Proxy = initProxy;
		if (NameBCAccount == "MelbetRU" || NameBCAccount == "BetBoom" || NameBCAccount == "LigaStavokRU" || NameBCAccount.Contains("Leon") || NameBCAccount.Contains("Betcity"))
		{
			UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
		}
	}

	public void UpdateAccount(string propertyName)
	{
		if (propertyName != "OwnerAccount")
		{
			this.OnUpdateAccount?.Invoke(this);
		}
	}

	public void AddStrategySettingsBase(StrategySettingsBase ss)
	{
		if (ss.TypeStrategy == TypesStrategy.Poslegoal)
		{
			PoslegolsSettings.Add(ss as PoslegolSettings);
		}
		else if (ss.TypeStrategy == TypesStrategy.Value)
		{
			ValuesSettings.Add(ss as ValueSettings);
		}
		else if (ss.TypeStrategy == TypesStrategy.Fork)
		{
			ForksSettings.Add(ss as ForkSettings);
		}
		else if (ss.TypeStrategy == TypesStrategy.ValuePlus)
		{
			ValuesPlusSettings.Add(ss as ValuesPlusSettings);
		}
		else if (ss.TypeStrategy == TypesStrategy.ValueScore)
		{
			ValuesScoreSettings.Add(ss as ValueScoreSettings);
		}
		SubscribeSettings(ss);
	}

	public void OnSubscribeSettings()
	{
		PoslegolsSettings.ToList().ForEach(delegate(PoslegolSettings ss)
		{
			SubscribeSettings(ss);
		});
		ValuesSettings.ToList().ForEach(delegate(ValueSettings ss)
		{
			SubscribeSettings(ss);
		});
		ForksSettings.ToList().ForEach(delegate(ForkSettings ss)
		{
			SubscribeSettings(ss);
		});
		ValuesPlusSettings.ToList().ForEach(delegate(ValuesPlusSettings ss)
		{
			SubscribeSettings(ss);
		});
		ValuesScoreSettings.ToList().ForEach(delegate(ValueScoreSettings ss)
		{
			SubscribeSettings(ss);
		});
	}

	public void OnUnsubscribeSettings()
	{
		PoslegolsSettings.ToList().ForEach(delegate(PoslegolSettings ss)
		{
			UnsubscribeSettings(ss);
		});
		ValuesSettings.ToList().ForEach(delegate(ValueSettings ss)
		{
			UnsubscribeSettings(ss);
		});
		ForksSettings.ToList().ForEach(delegate(ForkSettings ss)
		{
			UnsubscribeSettings(ss);
		});
		ValuesPlusSettings.ToList().ForEach(delegate(ValuesPlusSettings ss)
		{
			UnsubscribeSettings(ss);
		});
		ValuesScoreSettings.ToList().ForEach(delegate(ValueScoreSettings ss)
		{
			UnsubscribeSettings(ss);
		});
	}

	private void SubscribeSettings(StrategySettingsBase ss)
	{
		ss.OnUpdateStrategySettingsBase += delegate
		{
			this.OnUpdateAccount?.Invoke(this);
		};
		foreach (IssueTypeParameter par in ss.IssueTypeParameters)
		{
			par.OnUpdate += delegate
			{
				this.OnUpdateAccount?.Invoke(this);
			};
		}
		foreach (ParametersSport par2 in ss.SportsParameters)
		{
			par2.OnUpdate += delegate
			{
				this.OnUpdateAccount?.Invoke(this);
			};
		}
		foreach (ParameterAccount par3 in ss.ParametersCheckBC)
		{
			par3.OnUpdate += delegate
			{
				this.OnUpdateAccount?.Invoke(this);
			};
		}
	}

	private void UnsubscribeSettings(StrategySettingsBase ss)
	{
		ss.OnUpdateStrategySettingsBase -= delegate
		{
			this.OnUpdateAccount?.Invoke(this);
		};
		foreach (IssueTypeParameter par in ss.IssueTypeParameters)
		{
			par.OnUpdate -= delegate
			{
				this.OnUpdateAccount?.Invoke(this);
			};
		}
		foreach (ParametersSport par2 in ss.SportsParameters)
		{
			par2.OnUpdate -= delegate
			{
				this.OnUpdateAccount?.Invoke(this);
			};
		}
		foreach (ParameterAccount par3 in ss.ParametersCheckBC)
		{
			par3.OnUpdate -= delegate
			{
				this.OnUpdateAccount?.Invoke(this);
			};
		}
	}

	public void AddParameterAccount(ParameterAccount p)
	{
		Parameters.Add(p);
		OnAddParameterAccount(p);
	}

	public void OnAddParameterAccount(ParameterAccount p)
	{
		p.OnUpdate += delegate
		{
			this.OnUpdateAccount?.Invoke(this);
		};
	}

	public void AddStrategy(TypesStrategy typesStrategy)
	{
		switch (typesStrategy)
		{
		case TypesStrategy.Poslegoal:
		{
			PoslegolSettings ps = CreatePoslegolSettings();
			AddStrategySettingsBase(ps);
			break;
		}
		case TypesStrategy.Value:
		{
			ValueSettings vs4 = CreateValueSettings();
			AddStrategySettingsBase(vs4);
			break;
		}
		case TypesStrategy.Fork:
		{
			ForkSettings vs3 = CreateForkSettings();
			AddStrategySettingsBase(vs3);
			break;
		}
		case TypesStrategy.ValuePlus:
		{
			ValuesPlusSettings vs2 = CreateValuePlusSettings();
			AddStrategySettingsBase(vs2);
			break;
		}
		case TypesStrategy.ValueScore:
		{
			ValueScoreSettings vs = CreateValueScoreSettings();
			AddStrategySettingsBase(vs);
			break;
		}
		}
	}

	public PoslegolSettings CreatePoslegolSettings()
	{
		PoslegolSettings settings = new PoslegolSettings(this)
		{
			Name = "Послегол_" + (PoslegolsSettings.Count + 1)
		};
		settings.SportsParameters.First((ParametersSport sp) => sp.IdSport == 3).IsChecked = false;
		if (OwnerAccount.Name == "Guppy")
		{
			if (NameBCAccount.Contains("LigaStavok"))
			{
				settings.IssueTypeParameters.Where((IssueTypeParameter itp) => itp.Points0 = false);
				settings.MinMaxSum = 3000.0;
				settings.MaxBalance = 300000.0;
			}
			else if (!(NameProvider == "FonbetRU"))
			{
				if (NameBCAccount.Contains("Olimp"))
				{
					settings.ParametersCheckBC.ToList().ForEach(delegate(ParameterAccount p)
					{
						p.IsChecked = p.NameParameter == "Winline";
					});
					settings.IssueTypeParameters.ToList().ForEach(delegate(IssueTypeParameter sp)
					{
						sp.Points0 = true;
					});
					settings.MinimalPayoutRatio = 1.4;
				}
				else if (NameBCAccount == "Winline")
				{
					settings.IssueTypeParameters.Where((IssueTypeParameter itp) => itp.Points0 = false);
					settings.SportsParameters.First((ParametersSport sp) => sp.IdSport == 6).IsChecked = false;
				}
			}
		}
		if (!(OwnerAccount.Name == "Guppy"))
		{
			settings.MaxBalance = 500000.0;
			settings.MinimalPayoutRatio = 1.6;
			settings.IssueTypeParameters.Where((IssueTypeParameter pr) => pr.IssueType.ToString().Contains("PointP")).ToList().ForEach(delegate(IssueTypeParameter pr)
			{
				pr.Points0 = false;
				pr.Points1 = false;
				pr.Points2 = false;
				pr.Points2Over = false;
			});
			if (NameProvider == "FonbetRU")
			{
				settings.ParametersCheckBC.Single((ParameterAccount pr) => pr.NameParameter == "SetkaCupProvider").IsChecked = false;
				settings.TypeIncludeFilterNamesTags = TypeInclude.Except;
				settings.FilterNamesTags = "SetkaCup";
			}
		}
		settings.SportsParameters.Single((ParametersSport sp) => sp.IdSport == 6).IsChecked = false;
		return settings;
	}

	public ValueSettings CreateValueSettings()
	{
		ValueSettings settings = new ValueSettings(this, TypesStrategy.Value)
		{
			Name = "Валуи_" + (ValuesSettings.Count + 1)
		};
		settings.ParametersCheckBC.Single((ParameterAccount par) => par.NameParameter == "SetkaCupProvider").IsChecked = false;
		settings.SportsParameters.First((ParametersSport sp) => sp.IdSport == 3).IsChecked = false;
		if (OwnerAccount.Name == "Guppy")
		{
			settings.MinMaxSum = 5000.0;
			settings.HalfPeriodFirst = false;
			if (NameBCAccount.Contains("LigaStavok"))
			{
				settings.MaxBalance = 300000.0;
			}
			else if (!(NameProvider == "FonbetRU") && NameBCAccount.Contains("Olimp"))
			{
				settings.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
				{
					sp.IsChecked = false;
				});
			}
		}
		else
		{
			settings.BalancePercent = 20.0;
			settings.MinSumBet = 50.0;
			settings.MaxBalance = 500000.0;
			settings.BalancePercent = 20.0;
			settings.HalfPeriodFirst = false;
			settings.CyberMatches = false;
		}
		settings.MinimalPayoutRatio = 1.8;
		return settings;
	}

	public ForkSettings CreateForkSettings()
	{
		ForkSettings settings = new ForkSettings(this, TypesStrategy.Fork)
		{
			Name = "Вилка(валуйная)_" + (ForksSettings.Count + 1)
		};
		settings.ParametersCheckBC.Single((ParameterAccount par) => par.NameParameter == "SetkaCupProvider").IsChecked = false;
		settings.SportsParameters.First((ParametersSport sp) => sp.IdSport == 3).IsChecked = false;
		if (OwnerAccount.Name == "Guppy")
		{
			settings.MinMaxSum = 5000.0;
			settings.HalfPeriodFirst = false;
			if (NameBCAccount.Contains("LigaStavok"))
			{
				settings.MaxBalance = 300000.0;
			}
			else if (!(NameProvider == "FonbetRU") && NameBCAccount.Contains("Olimp"))
			{
				settings.SportsParameters.Where((ParametersSport sp) => sp.Sport.IsSlowTimeGoal).ToList().ForEach(delegate(ParametersSport sp)
				{
					sp.IsChecked = false;
				});
			}
		}
		else
		{
			settings.BalancePercent = 20.0;
			settings.MinSumBet = 50.0;
			settings.MaxBalance = 500000.0;
			settings.BalancePercent = 20.0;
			settings.HalfPeriodFirst = false;
			settings.CyberMatches = false;
		}
		settings.MinimalPayoutRatio = 1.8;
		return settings;
	}

	public ValuesPlusSettings CreateValuePlusSettings()
	{
		ValuesPlusSettings settings = new ValuesPlusSettings(this, TypesStrategy.ValuePlus)
		{
			Name = "Валуи+_" + (ValuesPlusSettings.Count + 1)
		};
		settings.ParametersCheckBC.Single((ParameterAccount par) => par.NameParameter == "SetkaCupProvider").IsChecked = false;
		settings.SportsParameters.First((ParametersSport sp) => sp.IdSport == 3).IsChecked = false;
		if (OwnerAccount.Name == "Guppy")
		{
			settings.HalfPeriodFirst = false;
			settings.MinMaxSum = 5000.0;
			if (NameBCAccount.Contains("LigaStavok"))
			{
				settings.MaxBalance = 300000.0;
			}
			else if (!(NameProvider == "FonbetRU"))
			{
			}
		}
		else
		{
			settings.BalancePercent = 20.0;
			settings.CyberMatches = false;
		}
		return settings;
	}

	public ValueScoreSettings CreateValueScoreSettings()
	{
		ValueScoreSettings settings = new ValueScoreSettings(this, TypesStrategy.ValueScore)
		{
			Name = "ВалуиПоСчету_" + (ValuesScoreSettings.Count + 1)
		};
		settings.MaximalPayoutRatio = 2.0;
		if (OwnerAccount.Name == "Guppy")
		{
			settings.HalfPeriodFirst = false;
			settings.MinMaxSum = 5000.0;
			if (NameBCAccount.Contains("LigaStavok"))
			{
				settings.MaxBalance = 300000.0;
			}
			else if (!(NameProvider == "FonbetRU"))
			{
			}
		}
		else
		{
			settings.BalancePercent = 20.0;
			settings.HalfPeriodFirst = false;
		}
		return settings;
	}

	public string CovertParametersToStringJSON()
	{
		return JsonConvert.SerializeObject(Parameters);
	}

	public string GetParameterValue(string nameParameter)
	{
		return Parameters.SingleOrDefault((ParameterAccount p) => p.NameParameter == nameParameter)?.ValueParameter;
	}

	public ObservableCollection<ParameterAccount> GetParameters()
	{
		return Parameters;
	}

	public void SetParameter(string nameParameter, string valueParameter)
	{
		ParameterAccount parameter = Parameters.SingleOrDefault((ParameterAccount p) => p.NameParameter == nameParameter);
		if (parameter == null)
		{
			AddParameterAccount(new ParameterAccount
			{
				NameParameter = nameParameter,
				ValueParameter = valueParameter
			});
		}
		else if (parameter.ValueParameter != valueParameter)
		{
			parameter.ValueParameter = valueParameter;
		}
		RaisePropertyChanged("SetParameter");
	}

	public string GetParameterSessionValue(string nameParameter)
	{
		return ParametersSession.SingleOrDefault((ParameterAccount p) => p.NameParameter == nameParameter)?.ValueParameter;
	}

	public void SetParameterSession(string nameParameter, string valueParameter)
	{
		ParameterAccount parameter = ParametersSession.SingleOrDefault((ParameterAccount p) => p.NameParameter == nameParameter);
		if (parameter == null)
		{
			ParametersSession.Add(new ParameterAccount
			{
				NameParameter = nameParameter,
				ValueParameter = valueParameter
			});
		}
		else if (parameter.ValueParameter != valueParameter)
		{
			parameter.ValueParameter = valueParameter;
		}
		RaisePropertyChanged("SetParameterSession");
	}

	public double ConvertToCurrencyAccount(double sum)
	{
		return Math.Floor(sum / Currency.Exchange);
	}

	public void OpenSettings()
	{
		this.OnOpenSettings?.Invoke(this);
	}

	public override string ToString()
	{
		return OwnerAccount?.Name + " " + NameProvider + " " + Login;
	}
}
