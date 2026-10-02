using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using AccountsLibrary;
using BookmakerLibrary;
using CommonLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AccountsManager;

public class WindowProxy : Window, IComponentConnector, IStyleConnector
{
	private NetPersistent Net = new NetPersistent();

	internal TextBox textBox_Proxy;

	internal TextBox textBox_ProxyTotalDays;

	private bool _contentLoaded;

	public Country Country { get; set; }

	public CollectionViewSource cvs { get; set; }

	private bool ShowDisplayOld { get; set; }

	public ObservableCollection<AccountSettings> ListAccounts { get; set; } = new ObservableCollection<AccountSettings>();

	public WindowProxy()
	{
		InitializeComponent();
		base.DataContext = this;
		cvs = FindResource("cvsListProxy") as CollectionViewSource;
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

	private void btnAddProxy(object sender, RoutedEventArgs e)
	{
		AccountsLibrary.ProxyBL.ListProxy.Add(new Proxy());
		cvs.View.Refresh();
	}

	private void checkBoxDisplayOld_Checked(object sender, RoutedEventArgs e)
	{
		ShowDisplayOld = !ShowDisplayOld;
		cvs.View.Refresh();
	}

	private void checkBoxDisplayOld_Unchecked(object sender, RoutedEventArgs e)
	{
		ShowDisplayOld = !ShowDisplayOld;
		cvs.View.Refresh();
	}

	private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
	{
		bool isAccepted = true;
		Proxy proxy = e.Item as Proxy;
		isAccepted = ((ShowDisplayOld || !(DateTime.Now > proxy.DateTimeEnd)) ? true : false);
		e.Accepted = isAccepted;
	}

	private void btnCopyProxy_Click(object sender, RoutedEventArgs e)
	{
		Proxy proxy = (sender as Button).DataContext as Proxy;
		Clipboard.SetText(proxy.Host + ":" + proxy.Port + ":" + proxy.UserName + ":" + proxy.Password);
	}

	private async void btnAddProxies(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(textBox_Proxy.Text))
		{
			return;
		}
		if (string.IsNullOrEmpty(textBox_ProxyTotalDays.Text))
		{
			new WindowInfo("Не указано количество дней прокси!").ShowDialog();
			return;
		}
		string strProxy = textBox_Proxy.Text;
		int totalDays = Convert.ToInt32(textBox_ProxyTotalDays.Text);
		Regex regex = new Regex("(?<ip>\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(?<port>\\d{1,5}):(?<username>\\w*):(?<password>\\w*)");
		if (regex.IsMatch(strProxy))
		{
			foreach (System.Text.RegularExpressions.Match findRegex in regex.Matches(strProxy))
			{
				string ip = findRegex.Groups["ip"].Value;
				string port = findRegex.Groups["port"].Value;
				string userName = findRegex.Groups["username"].Value;
				string password = findRegex.Groups["password"].Value;
				if (AccountsLibrary.ProxyBL.ListProxy.Where((Proxy pr) => pr.Host == ip && pr.Port == Convert.ToInt32(port) && pr.UserName == userName && pr.Password == password).FirstOrDefault() == null)
				{
					int numberProxy = ((AccountsLibrary.ProxyBL.ListProxy.Where((Proxy pr) => pr.NameProxy.Contains(userName) && pr.Country != null && pr.Country.Name == Country.Name).Count() == 0) ? 1 : ((from pr in AccountsLibrary.ProxyBL.ListProxy
						where pr.NameProxy.Contains(userName) && pr.Country != null && pr.Country.Name == Country.Name
						select Convert.ToInt32(pr.NameProxy.Replace(userName + "_", ""))).Max() + 1));
					string nameProxy = userName + "_" + numberProxy;
					Proxy newProxy = new Proxy
					{
						IsEmpty = false,
						Host = ip,
						Port = Convert.ToInt32(port),
						NameProxy = nameProxy,
						UserName = userName,
						Password = password,
						DateTimeEnd = DateTime.Now.AddDays(totalDays)
					};
					if (Country != null)
					{
						newProxy.Country = Country;
					}
					AccountsLibrary.ProxyBL.ListProxy.Add(newProxy);
				}
			}
			AccountsCenterClient.UpdateProxies();
		}
		else
		{
			regex = new Regex("Login: (?<login>.*)\\b");
			string userName2 = regex.Match(strProxy).Groups["login"].Value;
			regex = new Regex("Password: (?<password>.*)\\b");
			string password2 = regex.Match(strProxy).Groups["password"].Value;
			regex = new Regex("(?<ip>\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(?<port>\\d{1,5})");
			int numberProxy2 = 0;
			foreach (System.Text.RegularExpressions.Match findRegex2 in regex.Matches(strProxy))
			{
				numberProxy2++;
				string ip2 = findRegex2.Groups["ip"].Value;
				string port2 = findRegex2.Groups["port"].Value;
				if (AccountsLibrary.ProxyBL.ListProxy.Where((Proxy pr) => pr.Host == ip2 && pr.Port == Convert.ToInt32(port2) && pr.UserName == userName2 && pr.Password == password2).FirstOrDefault() == null)
				{
					string nameProxy2 = userName2 + "_" + numberProxy2;
					Proxy newProxy2 = new Proxy
					{
						IsEmpty = false,
						Host = ip2,
						Port = Convert.ToInt32(port2),
						NameProxy = nameProxy2,
						UserName = userName2,
						Password = password2,
						DateTimeEnd = DateTime.Now.AddDays(totalDays)
					};
					if (Country != null)
					{
						newProxy2.Country = Country;
					}
					AccountsLibrary.ProxyBL.ListProxy.Add(newProxy2);
				}
			}
		}
		foreach (Proxy proxy in AccountsLibrary.ProxyBL.ListProxy.Where((Proxy pr) => !pr.IsEmpty))
		{
			if (!string.IsNullOrEmpty(proxy.City))
			{
				continue;
			}
			string url = "http://ipwho.is/" + proxy.Host.ToString();
			string result = await Net.GETResponseAsync(url);
			if (!string.IsNullOrEmpty(result))
			{
				JContainer jsonData = (JContainer)JsonConvert.DeserializeObject(result);
				proxy.Country = CountriesBL.Countries.SingleOrDefault((Country c) => c.NameEng == jsonData["country_code"].ToString());
				proxy.City = ((proxy.Country != null) ? proxy.Country.Name : "") + " " + jsonData["city"].ToString();
			}
			await Task.Delay(50);
		}
	}

	private void btn_SettingsClick(object sender, RoutedEventArgs e)
	{
		ViewModelMainWindow.OpenAccountSettings((AccountSettings)((Button)sender).DataContext, readOnly: true);
	}

	private async void listBoxProxy_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ListAccounts.Clear();
		if (!((sender as ListBox).SelectedItem is Proxy proxy))
		{
			return;
		}
		List<AccountSettings> accountsSettings = await AccountsCenterClient.SearchRequestAsync(proxy.ToString());
		if (accountsSettings == null || accountsSettings.Count == 0)
		{
			return;
		}
		foreach (AccountSettings accountSettings in accountsSettings)
		{
			ListAccounts.Add(accountSettings);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocater = new Uri("/AccountsManager;component/_accountsmanager/windowproxy.xaml", UriKind.Relative);
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
			((Button)target).Click += btnClose_Click;
			break;
		case 4:
			((CheckBox)target).Checked += checkBoxDisplayOld_Checked;
			((CheckBox)target).Unchecked += checkBoxDisplayOld_Unchecked;
			break;
		case 5:
			((ListBox)target).SelectionChanged += listBoxProxy_SelectionChanged;
			break;
		case 8:
			((Button)target).Click += btnAddProxy;
			break;
		case 9:
			textBox_Proxy = (TextBox)target;
			break;
		case 10:
			textBox_ProxyTotalDays = (TextBox)target;
			break;
		case 11:
			((Button)target).Click += btnAddProxies;
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
			((Button)target).Click += btnCopyProxy_Click;
			break;
		case 7:
			((Button)target).Click += btn_SettingsClick;
			break;
		}
	}
}
