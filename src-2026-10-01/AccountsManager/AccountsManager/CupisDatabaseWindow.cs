using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AccountsLibrary;
using AccountsLibrary.CUPIS;
using BookmakerLibrary;

namespace AccountsManager;

public class CupisDatabaseWindow : Window, IComponentConnector, IStyleConnector
{
	public class Client
	{
		public string Username { get; set; }

		public string Number { get; set; }

		public DateTime Time { get; set; }
	}

	public class ClientInfoElement : INotifyPropertyChanged
	{
		public CupisClient client;

		public CupisResponse cupisResponse;

		public CupisBookmakerStatistic cupisBookmakerStatistic;

		public string Username { get; set; }

		public string Bookmaker { get; set; }

		public AccountSettings Account { get; set; }

		public string ButtonText
		{
			get
			{
				if (Account != null)
				{
					return "";
				}
				return "Добавить";
			}
			set
			{
			}
		}

		public int Saldo { get; set; }

		public int Input { get; set; }

		public int Output { get; set; }

		public string Comment
		{
			get
			{
				return Account?.Discription;
			}
			set
			{
				if (Account != null)
				{
					Account.Discription = value;
					RaisePropertyChanged("Comment");
				}
			}
		}

		public string RecommendedCost { get; set; }

		public DateTime Time { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class ResultElement
	{
		public int clientIndex;

		public int responseIndex;

		public int statisticIndex;

		public string Username { get; set; }

		public string Number { get; set; }

		public DateTime Time { get; set; }

		public int TotalCheck { get; set; }

		public string Comment
		{
			get
			{
				if (additionals.ContainsKey(Number))
				{
					return additionals[Number].comment;
				}
				return "";
			}
			set
			{
				if (!additionals.ContainsKey(Number))
				{
					additionals.Add(Number, new CupisClientAdditionals());
				}
				additionals[Number].comment = value;
			}
		}

		public SolidColorBrush Color
		{
			get
			{
				if (additionals.ContainsKey(Number))
				{
					return additionals[Number].color;
				}
				return Brushes.White;
			}
			set
			{
				if (!additionals.ContainsKey(Number))
				{
					additionals.Add(Number, new CupisClientAdditionals());
				}
				additionals[Number].color = value;
			}
		}
	}

	public class AccountStatElement
	{
		public string Name { get; set; }

		public int AddedCount { get; set; }

		public int TotalCount { get; set; }

		public Label Label { get; set; }

		public string DisplayedText
		{
			get
			{
				return $"{Name}: {AddedCount}/{TotalCount}";
			}
			set
			{
			}
		}

		public void Update()
		{
			Label.Content = DisplayedText;
		}

		public void Reset()
		{
			AddedCount = 0;
			TotalCount = 0;
		}
	}

	public class CupisClientAdditionals
	{
		public SolidColorBrush color;

		public string comment;
	}

	public static bool IsLoadedCupisDataBase = false;

	private static readonly Regex DIGITSONLYREGEX = new Regex("[^0-9]+");

	public static CupisDatabaseWindow instance;

	private static List<CupisClient> cupisClients = new List<CupisClient>();

	public List<ResultElement> cupisClientsSearchResults = new List<ResultElement>();

	private List<Client> clientsView = new List<Client>();

	private List<ClientInfoElement> clientInfoView = new List<ClientInfoElement>();

	private List<CheckBox> bookmakersCheckboxes = new List<CheckBox>();

	private List<AccountStatElement> accountStatElements = new List<AccountStatElement>();

	public static Dictionary<string, CupisClientAdditionals> additionals = new Dictionary<string, CupisClientAdditionals>();

	public static Dictionary<long, List<AccountSettings>> relatedAccounts = new Dictionary<long, List<AccountSettings>>();

	internal TextBox saldo_tb;

	internal WrapPanel bookmakersWrapPanel;

	internal TextBox usernameNumber_tb;

	internal ListBox userView_lb;

	internal TextBox cupisAccountInfo_tb;

	internal DataGrid datagrid;

	internal Label usersCount_lb;

	internal Label resultsCount_lb;

	internal Label lastUpdate_lb;

	internal WrapPanel accountsStatsWrapPanel;

	private bool _contentLoaded;

	public CupisDatabaseWindow()
	{
		InitializeComponent();
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AccountsManager;component/Resources/am_logo.png"));
		instance = this;
		datagrid.ItemsSource = cupisClientsSearchResults;
		userView_lb.ItemsSource = clientInfoView;
		foreach (Bookmaker item in BookmakersBL.Bookmakers)
		{
			if (!(item.NameCupis == ""))
			{
				CheckBox checkBox = new CheckBox
				{
					Content = item.NameCupis
				};
				if (item.NameCupis.Contains("Балтбет"))
				{
					checkBox.Content = "БК «БАЛТБЕТ»";
				}
				checkBox.Checked += OnBookmakerCheckboxChanged;
				checkBox.Unchecked += OnBookmakerCheckboxChanged;
				checkBox.Width = 150.0;
				bookmakersCheckboxes.Add(checkBox);
				bookmakersWrapPanel.Children.Add(checkBox);
			}
		}
		foreach (Bookmaker item2 in BookmakersBL.Bookmakers)
		{
			if (!(item2.NameCupis == ""))
			{
				AccountStatElement element = new AccountStatElement
				{
					Name = item2.NameCupis
				};
				Label label = new Label
				{
					DataContext = element,
					Content = element.DisplayedText,
					ToolTip = "Добавлено/Всего с учётом фильтра",
					Width = 170.0
				};
				element.Label = label;
				accountStatElements.Add(element);
				accountsStatsWrapPanel.Children.Add(label);
			}
		}
	}

	public static void HideWindow()
	{
		instance.Dispatcher.Invoke(delegate
		{
			instance.Hide();
		});
	}

	public void SetCupisDatabase(List<CupisClient> clients)
	{
		clients = (from x in clients
			orderby x.LastTimeRequest descending
			orderby x.LastSuccessCheck descending
			select x).ToList();
		foreach (CupisClient client2 in clients)
		{
			foreach (CupisResponse cupisResponse in client2.CupisResponses)
			{
				cupisResponse.Timestamp = cupisResponse.Timestamp.ToUniversalTime().AddHours(3.0);
			}
		}
		foreach (CupisClient item in clients)
		{
			foreach (AccountSettings item2 in AccountsSettingsBL.AccountsSettings)
			{
				if (!(item.CupisLogin == item2.Cupis.Login))
				{
					continue;
				}
				GetRelatedAccounts(item).Add(item2);
				foreach (CupisResponse cupisResponse2 in item.CupisResponses)
				{
					foreach (CupisBookmakerStatistic item3 in cupisResponse2.Statistic)
					{
						if (item3.BookmakerCompany.Contains(item2.NameBCAccount))
						{
							item3.isHasAccount = true;
							break;
						}
					}
				}
			}
		}
		foreach (CupisClient item4 in clients)
		{
			Client client = new Client();
			client.Username = item4.UserName;
			client.Number = item4.CupisLogin;
			client.Time = ((DateTime.MinValue != item4.LastSuccessCheck) ? item4.LastSuccessCheck : item4.LastTimeRequest);
			clientsView.Add(client);
		}
		cupisClients = clients;
		base.Dispatcher.Invoke(() => lastUpdate_lb.Content = DateTime.Now.ToLongTimeString());
		base.Dispatcher.Invoke(delegate
		{
			datagrid.Items.Refresh();
		});
		Update();
		IsLoadedCupisDataBase = true;
	}

	public static List<AccountSettings> GetRelatedAccounts(CupisClient client)
	{
		if (!relatedAccounts.ContainsKey(client.IdChat))
		{
			relatedAccounts.Add(client.IdChat, new List<AccountSettings>());
		}
		return relatedAccounts[client.IdChat];
	}

	public void Update()
	{
		string _saldo = "";
		base.Dispatcher.Invoke(() => _saldo = saldo_tb.Text);
		int.TryParse(_saldo, out var saldo);
		List<string> bookmakers = new List<string>();
		base.Dispatcher.Invoke(delegate
		{
			foreach (CheckBox current in bookmakersCheckboxes)
			{
				if (current.IsChecked.Value)
				{
					bookmakers.Add((string)current.Content);
				}
			}
		});
		string usernameNumber = "";
		base.Dispatcher.Invoke(() => usernameNumber = usernameNumber_tb.Text.Trim());
		if (usernameNumber.Length == 11 && (usernameNumber.Remove(2) == "89" || usernameNumber.Remove(2) == "79"))
		{
			usernameNumber = "+7" + usernameNumber.Remove(0, 1);
		}
		SetFilter(bookmakers, saldo, usernameNumber);
	}

	public void SetFilter(List<string> bookmakers, int saldo, string usernameNumber)
	{
		if (saldo == 0)
		{
			saldo = int.MinValue;
		}
		cupisClientsSearchResults.Clear();
		List<ResultElement> results = new List<ResultElement>();
		usernameNumber = usernameNumber.ToLower();
		for (int i = 0; i < cupisClients.Count; i++)
		{
			CupisClient client = cupisClients[i];
			for (int ii = 0; ii < client.CupisResponses.Count; ii++)
			{
				CupisResponse cupisResponse = client.CupisResponses[ii];
				if (cupisResponse.Statistic == null)
				{
					continue;
				}
				bool isClientAdded = false;
				for (int iii = 0; iii < cupisResponse.Statistic.Count; iii++)
				{
					CupisBookmakerStatistic statistic = cupisResponse.Statistic[iii];
					bool isBookmakerPass = bookmakers.Count == 0;
					foreach (string item2 in bookmakers)
					{
						if (statistic.BookmakerCompany.Contains(item2) || (item2 == "БК «FON.BET»" && statistic.BookmakerCompany.ToUpper().Contains("FONBET")))
						{
							isBookmakerPass = true;
							break;
						}
					}
					bool isHasAccount = false;
					foreach (AccountSettings account in AccountsSettingsBL.AccountsSettings.Where((AccountSettings x) => x.Cupis.Login == cupisResponse.CupisLogin))
					{
						if (account != null && account.NameBCAccount == BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker x) => x.NameCupis == statistic.BookmakerCompany)?.Name)
						{
							isHasAccount = true;
							break;
						}
					}
					if (statistic.SaldoPeriod > (double)saldo && !isHasAccount)
					{
						string number = (string.IsNullOrEmpty(cupisResponse.CupisLogin) ? client.CupisLogin : cupisResponse.CupisLogin);
						if (isBookmakerPass && !isClientAdded && ((client.UserName != null && client.UserName.ToLower().Contains(usernameNumber)) || number.Contains(usernameNumber)))
						{
							ResultElement element = new ResultElement();
							element.Username = client.UserName;
							element.Number = number;
							element.Time = cupisResponse.Timestamp;
							element.TotalCheck = client.TotalCheck;
							element.clientIndex = i;
							element.responseIndex = ii;
							element.statisticIndex = iii;
							results.Add(element);
							isClientAdded = true;
						}
					}
				}
			}
		}
		cupisClientsSearchResults.AddRange(results.OrderByDescending((ResultElement x) => x.Time));
		base.Dispatcher.Invoke(delegate
		{
			datagrid.Items.Refresh();
		});
		base.Dispatcher.Invoke(() => usersCount_lb.Content = cupisClients.Count);
		base.Dispatcher.Invoke(() => resultsCount_lb.Content = cupisClientsSearchResults.Count);
		UpdateAccountStats();
	}

	public void UpdateAccountStats()
	{
		string _saldo = "";
		base.Dispatcher.Invoke(() => _saldo = saldo_tb.Text);
		int.TryParse(_saldo, out var saldo);
		foreach (AccountStatElement accountStatElement2 in accountStatElements)
		{
			accountStatElement2.Reset();
		}
		for (int i = 0; i < cupisClients.Count; i++)
		{
			CupisClient client = cupisClients[i];
			foreach (CupisResponse item in client.CupisResponses)
			{
				if (item.Statistic == null)
				{
					continue;
				}
				for (int ii = 0; ii < item.Statistic.Count; ii++)
				{
					CupisBookmakerStatistic statistic = item.Statistic[ii];
					AccountStatElement accountStatElement = accountStatElements.FirstOrDefault((AccountStatElement x) => x.Name == statistic.BookmakerCompany);
					if (statistic.SaldoTotal > (double)saldo && accountStatElement != null)
					{
						accountStatElement.TotalCount++;
						string accountBookmakerName = BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker x) => x.NameCupis == accountStatElement.Name)?.Name;
						if (GetRelatedAccounts(client).FirstOrDefault((AccountSettings x) => x.NameBCAccount == accountBookmakerName) != null)
						{
							accountStatElement.AddedCount++;
						}
					}
				}
			}
		}
		foreach (AccountStatElement item2 in accountStatElements)
		{
			base.Dispatcher.Invoke(delegate
			{
				item2.Update();
			});
		}
	}

	public static List<CupisClient> GetClients()
	{
		return cupisClients;
	}

	private void OnCupisClientsResultsSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
	}

	private void OnSaldoPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		e.Handled = DIGITSONLYREGEX.IsMatch(e.Text);
	}

	private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			Update();
		}
	}

	private void OnPalleteButtonClick(object sender, RoutedEventArgs e)
	{
		int selectedIndex = datagrid.SelectedIndex;
		if (selectedIndex != -1 && selectedIndex < cupisClientsSearchResults.Count)
		{
			ResultElement resultElement = cupisClientsSearchResults[selectedIndex];
			SolidColorBrush color = (SolidColorBrush)((Button)sender).Background;
			resultElement.Color = color;
			datagrid.Items.Refresh();
		}
	}

	private void OnBookmakerCheckboxChanged(object sender, RoutedEventArgs e)
	{
		Update();
	}

	private void OnSearchResultsSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		int selectedIndex = datagrid.SelectedIndex;
		if (selectedIndex == -1 || selectedIndex >= cupisClientsSearchResults.Count)
		{
			return;
		}
		ResultElement resultElement = cupisClientsSearchResults[selectedIndex];
		CupisClient client = cupisClients[resultElement.clientIndex];
		CupisResponse cupisResponse = client.CupisResponses[resultElement.responseIndex];
		cupisAccountInfo_tb.Text = client.CupisResponses[resultElement.responseIndex].ToString();
		clientInfoView.Clear();
		foreach (CupisBookmakerStatistic item in cupisResponse.Statistic)
		{
			ClientInfoElement clientInfoElement = new ClientInfoElement();
			clientInfoElement.Bookmaker = item.BookmakerCompany;
			clientInfoElement.Username = client.UserName;
			clientInfoElement.Saldo = (int)item.SaldoTotal;
			clientInfoElement.Input = (int)item.InputTotal;
			clientInfoElement.Output = (int)item.OutputTotal;
			clientInfoElement.client = client;
			clientInfoElement.cupisResponse = cupisResponse;
			clientInfoElement.cupisBookmakerStatistic = item;
			clientInfoElement.Time = item.lastTransaction;
			clientInfoElement.RecommendedCost = clientInfoElement.cupisBookmakerStatistic.CalcualteAccountCost();
			foreach (AccountSettings account in AccountsSettingsBL.AccountsSettings.Where((AccountSettings x) => x.Cupis.Login == client.cupisLogin))
			{
				if (account != null && account.NameBCAccount == BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker x) => x.NameCupis == item.BookmakerCompany)?.Name)
				{
					clientInfoElement.Account = account;
					break;
				}
			}
			clientInfoView.Add(clientInfoElement);
		}
		userView_lb.Items.Refresh();
		userView_lb.ApplyTemplate();
	}

	private async void OnAccountInfoElementCommentaryTextChanged(object sender, TextChangedEventArgs e)
	{
		ClientInfoElement element = (ClientInfoElement)((TextBox)sender).DataContext;
		element.cupisBookmakerStatistic.isHasAccount = true;
		if (element.Account != null)
		{
			return;
		}
		Bookmaker bookmaker = BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker bc) => bc.NameCupis.ToUpper() == element.Bookmaker.ToUpper());
		if (bookmaker == null && (element.Bookmaker.ToUpper() == "БК «FONBET»".ToUpper() || element.Bookmaker.ToUpper() == "БК «ФОНБЕТ»".ToUpper()))
		{
			bookmaker = BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker bc) => bc.NameCupis.ToUpper() == "БК «FON.BET»".ToUpper());
		}
		if (bookmaker == null)
		{
			return;
		}
		ClientInfoElement clientInfoElement = element;
		clientInfoElement.Account = await ViewModelMainWindow.instance.AddAccount(bookmaker, element.client.CupisLogin, element.client.CupisPassword, element.cupisResponse.ToString(), element.cupisBookmakerStatistic.InputTotal, element.cupisBookmakerStatistic.OutputTotal, element.Username);
		if (element.Account != null)
		{
			GetRelatedAccounts(element.client).Add(element.Account);
			AccountStatElement accountStatElement = accountStatElements.FirstOrDefault((AccountStatElement x) => x.Name == element.Bookmaker);
			if (accountStatElement != null)
			{
				accountStatElement.AddedCount++;
				accountStatElement.Update();
			}
			userView_lb.Items.Refresh();
		}
	}

	private async void OnAddAccountButtonClick(object sender, RoutedEventArgs e)
	{
		ClientInfoElement element = (ClientInfoElement)((Button)sender).DataContext;
		if (element.Account != null)
		{
			ViewModelMainWindow.OpenAccountSettings(element.Account);
			return;
		}
		Bookmaker bookmaker = BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker bc) => bc.NameCupis.ToUpper() == element.Bookmaker.ToUpper());
		if (bookmaker == null && (element.Bookmaker.ToUpper() == "БК «FONBET»".ToUpper() || element.Bookmaker.ToUpper() == "БК «ФОНБЕТ»".ToUpper()))
		{
			bookmaker = BookmakersBL.Bookmakers.FirstOrDefault((Bookmaker bc) => bc.NameCupis.ToUpper() == "БК «FON.BET»".ToUpper());
		}
		if (bookmaker == null)
		{
			return;
		}
		ClientInfoElement clientInfoElement = element;
		clientInfoElement.Account = await ViewModelMainWindow.instance.AddAccount(bookmaker, element.cupisResponse.CupisLogin, element.client.CupisPassword, element.cupisResponse.ToString(), element.cupisBookmakerStatistic.InputTotal, element.cupisBookmakerStatistic.OutputTotal, element.Username);
		if (element.Account != null)
		{
			element.cupisBookmakerStatistic.isHasAccount = true;
			GetRelatedAccounts(element.client).Add(element.Account);
			AccountStatElement accountStatElement = accountStatElements.FirstOrDefault((AccountStatElement x) => x.Name == element.Bookmaker);
			if (accountStatElement != null)
			{
				accountStatElement.AddedCount++;
				accountStatElement.Update();
			}
			((Button)sender).GetBindingExpression(ContentControl.ContentProperty).UpdateTarget();
		}
	}

	private void OnWindowClosing(object sender, CancelEventArgs e)
	{
		Hide();
		e.Cancel = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocater = new Uri("/AccountsManager;component/cupisdatabasewindow.xaml", UriKind.Relative);
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
			((CupisDatabaseWindow)target).Closing += OnWindowClosing;
			break;
		case 2:
			saldo_tb = (TextBox)target;
			saldo_tb.PreviewTextInput += OnSaldoPreviewTextInput;
			saldo_tb.KeyDown += OnSearchTextBoxKeyDown;
			break;
		case 3:
			bookmakersWrapPanel = (WrapPanel)target;
			break;
		case 4:
			usernameNumber_tb = (TextBox)target;
			usernameNumber_tb.KeyDown += OnSearchTextBoxKeyDown;
			break;
		case 5:
			userView_lb = (ListBox)target;
			break;
		case 8:
			cupisAccountInfo_tb = (TextBox)target;
			break;
		case 9:
			datagrid = (DataGrid)target;
			datagrid.SelectionChanged += OnSearchResultsSelectionChanged;
			break;
		case 10:
			((Button)target).Click += OnPalleteButtonClick;
			break;
		case 11:
			((Button)target).Click += OnPalleteButtonClick;
			break;
		case 12:
			((Button)target).Click += OnPalleteButtonClick;
			break;
		case 13:
			((Button)target).Click += OnPalleteButtonClick;
			break;
		case 14:
			((Button)target).Click += OnPalleteButtonClick;
			break;
		case 15:
			usersCount_lb = (Label)target;
			break;
		case 16:
			resultsCount_lb = (Label)target;
			break;
		case 17:
			lastUpdate_lb = (Label)target;
			break;
		case 18:
			accountsStatsWrapPanel = (WrapPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 6:
			((Button)target).Click += OnAddAccountButtonClick;
			break;
		case 7:
			((TextBox)target).TextChanged += OnAccountInfoElementCommentaryTextChanged;
			break;
		}
	}
}
