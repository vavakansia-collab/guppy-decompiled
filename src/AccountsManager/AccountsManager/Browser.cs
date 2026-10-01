using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using AccountsLibrary;
using CommonLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V113;
using OpenQA.Selenium.DevTools.V113.Network;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Network;

namespace AccountsManager;

public class Browser
{
	public struct Response
	{
		public string RequestId { get; set; }

		public string ResponseUrl { get; set; }

		public long ResponseStatus { get; set; }

		public bool ResponseBodySuccess { get; set; }

		public string ResponseBody { get; set; }
	}

	private static string[] gpuList = new string[76]
	{
		"AMD Radeon R7 250 1GB", "AMD Radeon RX 460 2GB", "AMD Radeon RX 460 4GB", "AMD Radeon RX 470 4GB", "AMD Radeon RX 470 8GB", "AMD Radeon RX 480 4GB", "AMD Radeon RX 480 8GB", "AMD Radeon RX 550 2GB", "AMD Radeon RX 550 4GB", "AMD Radeon RX 5500 XT 4GB",
		"AMD Radeon RX 5500 XT 8GB", "AMD Radeon RX 560 2GB", "AMD Radeon RX 560 4GB", "AMD Radeon RX 5600 XT 6GB", "AMD Radeon RX 570 4GB", "AMD Radeon RX 570 8GB", "AMD Radeon RX 5700 8GB", "AMD Radeon RX 5700 XT 50th Anniversary Edition 8GB", "AMD Radeon RX 5700 XT 8GB", "AMD Radeon RX 580 4GB",
		"AMD Radeon RX 580 8GB", "AMD Radeon RX 590 8GB", "AMD Radeon RX 6400 4GB", "AMD Radeon RX 6500 XT 4GB", "AMD Radeon RX 6600 8GB", "AMD Radeon RX 6600 XT 8GB", "AMD Radeon RX 6650 XT 8GB", "AMD Radeon RX 6700 10GB", "AMD Radeon RX 6700 XT 12GB", "AMD Radeon RX 6750 XT 12GB",
		"AMD Radeon RX 6800 16GB", "AMD Radeon RX 6800 XT 16GB", "AMD Radeon RX 6900 XT 16GB", "AMD Radeon RX 6950 XT 16GB", "AMD Radeon RX Vega 56 8GB", "AMD Radeon RX Vega 64 8GB", "AMD Radeon VII 16GB", "Nvidia GeForce GT 1030 2GB", "Nvidia GeForce GTX 1050 2GB", "Nvidia GeForce GTX 1050 Ti 4GB",
		"Nvidia GeForce GTX 1060 3GB", "Nvidia GeForce GTX 1060 6GB", "Nvidia GeForce GTX 1070 8GB", "Nvidia GeForce GTX 1080 8GB", "Nvidia GeForce GTX 1080 Ti 11GB", "Nvidia GeForce GTX 1650 4GB GDDR5", "Nvidia GeForce GTX 1650 4GB GDDR6", "Nvidia GeForce GTX 1650 Super 4GB", "Nvidia GeForce GTX 1660 6GB", "Nvidia GeForce GTX 1660 Super 6GB",
		"Nvidia GeForce GTX 1660 Ti 6GB", "Nvidia GeForce GTX 950 2GB", "Nvidia GeForce GTX 950 4GB", "Nvidia GeForce GTX 960 2GB", "Nvidia GeForce GTX 960 4GB", "Nvidia GeForce GTX 970 4GB", "Nvidia GeForce GTX 980 4GB", "Nvidia GeForce GTX 980 Ti 6GB", "Nvidia GeForce RTX 2060 6GB", "Nvidia GeForce RTX 2060 Super 6GB",
		"Nvidia GeForce RTX 2070 8GB", "Nvidia GeForce RTX 2070 Super 8GB", "Nvidia GeForce RTX 2080 8GB", "Nvidia GeForce RTX 2080 Super 8GB", "Nvidia GeForce RTX 2080 Ti 11GB", "Nvidia GeForce RTX 3060 12GB", "Nvidia GeForce RTX 3060 Ti 8GB", "Nvidia GeForce RTX 3070 8GB", "Nvidia GeForce RTX 3070 Ti 8GB", "Nvidia GeForce RTX 3080 10GB",
		"Nvidia GeForce RTX 3080 Ti 12GB", "Nvidia GeForce RTX 3090 24GB", "Nvidia GeForce RTX 3090 Ti 24GB", "Nvidia GeForce RTX 4070 Ti 12GB", "Nvidia GeForce RTX 4080 16GB", "Nvidia GeForce RTX 4090 24GB"
	};

	public static Random random = new Random();

	private readonly IntPtr TITLEPOINTER;

	private readonly string ProfilePath;

	private readonly string CookieFile;

	private ProxyServer TitaniumProxyServer = new ProxyServer();

	private static int numPortTitaniumProxy = 8010;

	private static readonly string DEFAULT_WEBDRIVER_DIRECTORY = Environment.CurrentDirectory + "\\ChromeDriver";

	private static readonly string DEFAULT_CHROME_BINARY_PATH = Environment.CurrentDirectory + "\\ChromeDriver\\Chrome\\chrome.exe";

	private static int driverServicePort = 8556;

	public AccountSettings AccountSettings { get; set; }

	public IWebDriver WebDriver { get; set; }

	private IDevTools DevTools { get; set; }

	private DevToolsSession DevToolsSession { get; set; }

	private static OpenQA.Selenium.DevTools.V113.DevToolsSessionDomains Domains { get; set; } = null;

	private Queue<ResponseReceivedEventArgs> ResponseEvents { get; set; } = new Queue<ResponseReceivedEventArgs>();

	public CookieContainer CookieContainer { get; set; } = new CookieContainer();

	public Process Process { get; set; }

	private string CurrentTabName { get; set; }

	private string ActiveHandle { get; set; }

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

	public Browser(AccountSettings accountSettings)
	{
		AccountSettings = accountSettings;
		if (AccountSettings.NameBCAccount == "MelbetRU")
		{
			if (!AccountSettings.Proxy.IsEmpty)
			{
				StartTitaniumProxyServer(AccountSettings.Proxy.Host, AccountSettings.Proxy.Port, AccountSettings.Proxy.UserName, AccountSettings.Proxy.Password);
			}
			else
			{
				StartTitaniumProxyServer();
			}
			InjectProxy("localhost", numPortTitaniumProxy, "", "");
			numPortTitaniumProxy++;
		}
		else
		{
			InjectProxy(AccountSettings.Proxy.Host, AccountSettings.Proxy.Port, AccountSettings.Proxy.UserName, AccountSettings.Proxy.Password);
		}
		string text = AccountSettings.Login;
		if (!string.IsNullOrEmpty(text) && text.Length == 12 && text.Remove(2) == "+7")
		{
			text = $"{long.Parse(text):+# ### ### ####}";
		}
		TITLEPOINTER = Marshal.StringToHGlobalUni(text);
		ProfilePath = Environment.CurrentDirectory + "\\WebCache\\_Chrome\\" + AccountSettings.NameBCAccount + "\\" + AccountSettings.GUID.ToString();
		CookieFile = ProfilePath + "/cookie_backup.txt";
		SetSitesOnMainPanel(new Dictionary<string, string>
		{
			{
				AccountSettings.NameBCAccount,
				"https://" + AccountSettings.Host
			},
			{ "ЦУПИС", "https://wallet.1cupis.ru/main" },
			{ "2 IP", "https://2ip.ru" }
		});
		ChromeOptions chromeOptions = new ChromeOptions();
		chromeOptions.AddArguments("user-data-dir=" + ProfilePath);
		chromeOptions.AddArgument("ignore-certificate-errors");
		chromeOptions.AddArgument("--disable-blink-features");
		chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
		if (AccountSettings.NameBCAccount != "MelbetRU" && AccountSettings.NameBCAccount != "LigaStavokRU" && AccountSettings.NameBCAccount != "BetBoom" && !AccountSettings.NameBCAccount.Contains("Leon") && !AccountSettings.NameBCAccount.Contains("Betcity"))
		{
			chromeOptions.AddArguments("--user-agent=" + accountSettings.UserAgent);
		}
		else
		{
			chromeOptions.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
		}
		chromeOptions.AddArgument("--check-for-update-interval=999999999");
		chromeOptions.AddExtension(Environment.CurrentDirectory + "/extension.crx");
		chromeOptions.AddExcludedArguments(new List<string> { "enable-automation" });
		chromeOptions.AddArgument("--no-sandbox");
		chromeOptions.AddArgument("--disable-infobars");
		chromeOptions.AddArgument("--disable-dev-shm-usage");
		chromeOptions.AddArgument("--disable-browser-side-navigation");
		chromeOptions.AddArgument("--disable-gpu");
		chromeOptions.AddArgument("--log-level=3");
		chromeOptions.BinaryLocation = DEFAULT_CHROME_BINARY_PATH;
		chromeOptions.PageLoadStrategy = PageLoadStrategy.None;
		double num = Math.Min(Math.Floor(SystemParameters.PrimaryScreenWidth / 1.3), 2000.0);
		double num2 = Math.Floor(SystemParameters.PrimaryScreenHeight / 1.1);
		double num3 = Math.Floor(SystemParameters.WorkArea.Width - num);
		double num4 = Math.Floor(SystemParameters.WorkArea.Height) - num2;
		chromeOptions.AddArguments("--window-size=" + num + "," + num2);
		chromeOptions.AddArguments("--window-position=" + num3 + "," + num4);
		ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(DEFAULT_WEBDRIVER_DIRECTORY);
		chromeDriverService.HideCommandPromptWindow = true;
		chromeDriverService.Port = driverServicePort;
		driverServicePort++;
		WebDriver = new ChromeDriver(chromeDriverService, chromeOptions);
		Process = GetWebBrowserProcess(chromeDriverService.ProcessId);
		InjectIcon();
		ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
		try
		{
			Task.Run(delegate
			{
				InitWebDriverDevTools();
			}).Wait();
		}
		catch
		{
			return;
		}
		Task.Run((Action)LoopResponse);
		Task.Run((Action)LoopName);
		Task.Run((Action)LoopTabs);
		if (string.IsNullOrEmpty(AccountSettings.ScriptBrowser))
		{
			int num5 = 1600;
			int num6 = 900;
			int num7 = 8;
			string text2 = gpuList[random.Next(0, gpuList.Length)];
			string text3 = (text2.Contains("AMD") ? "Google (AMD)" : "Google (Nvidia)");
			string text4 = "WebKit";
			string text5 = "WebKit WebGL";
			string text6 = "Version 1.0";
			string text7 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAANwAAAAeCAAAAABiES/iAAACeElEQVRYw+2YzUtUURjGf47OmDPh5AyFomUiEeEmyghXtWsh4dcswlYV2KYWfZh/QRBUVLhTCCJXEgmKUCIkFhJREARBkbkyKBlTRmUC82lxZ7z3TjM4whwXwz2ry3vO87znx33Pey4XFfHAg/PgPDgPzoPz4Dy4rFIKscSkAfmnsUY+iTfXFhxue4Zm4QpfaKbg8k+EsZNsGG6iNVzRMrkZeRPmjp6eCgcae5f+3wJIgtWLldG+DUnfzoail1etaVsEa1f2lUqw2hPd3T7nCrkMtlkQ24YDwP8+FZkI+gY3uq2cTcu54GIA/dJCDUAnSE4RdAESdALUxZ0hl4E5OMs49iE528E5a+cj5YFhDVI3vLA2c4K+zLXpvR37tNRDs3STg1OJqXqQSwS14wlJUD+VeHWAW86Qy8BwQ5Ek/WK/JBgqC72UTvJakmY5lAvurTRPSDrMmKRRcIvgeUo2KmmEI86Qy8DwmVu/ezQIBCSBLzwjKZhujv5cZZmUNkAq57ekRXCLYDG12pre5Qy5DAzDXbPfIOB/JqmCzNafCZd+dMA5RfZxdsBlNTAMF+FJfD2eSvSI0iGpmXe5GnbG3qyyHAO3yCZxlGV2uBLWDcJVMZKc7UrnfIBvQI+pHpxbS34ZaNkK7gYN0yvTDSCXyCZxNJTscFFe/DUH1w3QvpnzPiUPdTXfsvxZDdBGmeQU2SQd9lWQHS5m9J6Ln4/suZCwc96D25qM1formq5/3ApOX1uDkZ7P7JXkENkkK5eqQm3flRtuvitSYgCucKOf0zv01bazcG3Tyz8GKukvSjjrlB3/U5Rw42dqAo29yypKOO8figeX1/gH+zX9JqfOeUwAAAAASUVORK5CYII=";
			bool flag = false;
			string text8 = "WebGLRenderingContext.prototype.getParameter = function(origFn) { const paramMap = {};  paramMap[0x9245] = \"" + text3 + "\";paramMap[0x9246] = \"" + text2 + "\";paramMap[0x1F00] = \"" + text4 + "\";paramMap[0x1F01] = \"" + text5 + "\";paramMap[0x1F02] = \"" + text6 + "\";return function(parameter) { return paramMap[parameter] || origFn.call(this, parameter);};} (WebGLRenderingContext.prototype.getParameter);Object.defineProperties(navigator, { hardwareConcurrency: { value: " + num7 + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { availHeight: { value: " + num6 + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { availWidth: { value: " + num5 + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { height: { value: " + num6 + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { width: { value: " + num5 + ", configurable: false, enumerable: true, writable: false }});window.cdc_adoQpoasnfa76pfcZLmcfl_Symbol = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Object = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Promise = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Array = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Proxy = null;";
			if (flag)
			{
				text8 = text8 + "const mainFunction = HTMLCanvasElement.prototype.toDataURL; HTMLCanvasElement.prototype.toDataURL = function (type) { if (true) {return '" + text7 + "';} return mainFunction.apply(this, arguments);};";
			}
			AccountSettings.ScriptBrowser = text8;
		}
		if (!(accountSettings.NameBCAccount != "LigaStavokRU"))
		{
			return;
		}
		Task.Run(delegate
		{
			while (!Process.HasExited)
			{
				try
				{
					ExecuteScript(AccountSettings.ScriptBrowser);
				}
				catch
				{
					Console.WriteLine("[FingerprintExecutor] Error");
					Thread.Sleep(500);
				}
				Thread.Sleep(1);
			}
		});
	}

	public void LoadCookies()
	{
		if (!File.Exists(CookieFile))
		{
			return;
		}
		string[] array = File.ReadAllLines(CookieFile);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('•');
			string domain = array2[0];
			string name = array2[1];
			string value = array2[2];
			try
			{
				WebDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(name, value, domain, "", null));
			}
			catch
			{
			}
		}
	}

	public void SaveCookies()
	{
		ReadOnlyCollection<OpenQA.Selenium.Cookie> allCookies = WebDriver.Manage().Cookies.AllCookies;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (OpenQA.Selenium.Cookie item in (IEnumerable<OpenQA.Selenium.Cookie>)allCookies)
		{
			stringBuilder.Append(item.Domain + "•" + item.Name + "•" + item.Value + "•" + item.Path + "•" + item.IsHttpOnly + "•" + item.Secure + "•" + item.Expiry.ToString() + "\r\n");
		}
		File.WriteAllText(CookieFile, stringBuilder.ToString().TrimEnd());
	}

	private async void InitWebDriverDevTools()
	{
		DevTools = WebDriver as IDevTools;
		DevToolsSession = DevTools.GetDevToolsSession();
		Domains = DevToolsSession.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V113.DevToolsSessionDomains>();
		await Domains.Network.Enable(new EnableCommandSettings());
		Domains.Network.ResponseReceived += ResponseReceived;
	}

	private void InjectProxy(string host, int port, string userName, string password)
	{
		string text = Path.Combine(Environment.CurrentDirectory, "ChromeDriver\\Ext\\");
		if (File.Exists(text + "manifest.json") && File.Exists(text + "background_template.js"))
		{
			string text2 = File.ReadAllText(text + "background_template.js");
			text2 = (AccountSettings.Proxy.IsEmpty ? text2.Replace("MODE", "direct") : text2.Replace("MODE", "fixed_servers"));
			text2 = text2.Replace("PROXY_HOST", host);
			text2 = text2.Replace("PROXY_PORT", port.ToString());
			text2 = text2.Replace("PROXY_USERNAME", userName);
			text2 = text2.Replace("PROXY_PASSWORD", password);
			StreamWriter streamWriter = File.CreateText(text + "background.js");
			streamWriter.Write(text2);
			streamWriter.Close();
			if (File.Exists(Environment.CurrentDirectory + "/extension.crx"))
			{
				File.Delete(Environment.CurrentDirectory + "/extension.crx");
			}
			ZipFile.CreateFromDirectory(text, Environment.CurrentDirectory + "/extension.crx");
		}
	}

	private void StartTitaniumProxyServer(string externalProxyHost = "", int externalProxyPort = -1, string externalProxyUsername = "", string externalProxyPassword = "")
	{
		TitaniumProxyServer.CertificateManager.CertificateEngine = CertificateEngine.DefaultWindows;
		TitaniumProxyServer.CertificateManager.EnsureRootCertificate();
		TitaniumProxyServer.CertificateManager.TrustRootCertificate(machineTrusted: true);
		ExplicitProxyEndPoint endPoint = new ExplicitProxyEndPoint(IPAddress.Any, numPortTitaniumProxy);
		TitaniumProxyServer.AddEndPoint(endPoint);
		if (externalProxyHost != "")
		{
			TitaniumProxyServer.UpStreamHttpProxy = new ExternalProxy
			{
				HostName = externalProxyHost,
				Port = externalProxyPort,
				UserName = externalProxyUsername,
				Password = externalProxyPassword
			};
			TitaniumProxyServer.UpStreamHttpsProxy = new ExternalProxy
			{
				HostName = externalProxyHost,
				Port = externalProxyPort,
				UserName = externalProxyUsername,
				Password = externalProxyPassword
			};
		}
		TitaniumProxyServer.Start();
	}

	public void InjectIcon()
	{
		try
		{
			Icon icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/BookmakerLibrary;component/Resources/BCImages/Icons/" + AccountSettings.NameBCAccount + ".ico")).Stream);
			SendMessage(Process.MainWindowHandle, 128, (IntPtr)0, icon.Handle);
			SendMessage(Process.MainWindowHandle, 128, (IntPtr)1, icon.Handle);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error on icon injecting. Reason: " + ex.Message);
		}
	}

	public Process GetWebBrowserProcess(int webdriverPID)
	{
		return (from ManagementObject p in new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessId = " + webdriverPID).Get()
			select Process.GetProcessById(Convert.ToInt32(p.GetPropertyValue("ProcessId")))).FirstOrDefault((Process p) => p.ProcessName == "chrome");
	}

	private void SetSitesOnMainPanel(Dictionary<string, string> sites)
	{
		string path = ProfilePath + "/Default/Preferences";
		if (!File.Exists(path))
		{
			return;
		}
		string text = File.ReadAllText(path);
		int startIndex = text.IndexOf("countryid_at_install\":");
		startIndex = text.IndexOf(",", startIndex) + 1;
		int num = text.IndexOf("\"data_reduction");
		if (num == -1)
		{
			return;
		}
		string text2 = text.Substring(0, startIndex);
		string text3 = text.Substring(num, text.Length - num);
		string text4 = "\"custom_links\":{\"initialized\":true,\"list\":[";
		foreach (KeyValuePair<string, string> site in sites)
		{
			string key = site.Key;
			string value = site.Value;
			text4 += "{";
			text4 += "\"isMostVisited\":false";
			text4 += ",";
			text4 = text4 + "\"title\":\"" + key + "\"";
			text4 += ",";
			text4 = text4 + "\"url\":\"" + value + "\"";
			text4 += "},";
		}
		text4 = text4.TrimEnd(',');
		text4 += "]},";
		string contents = text2 + text4 + text3;
		File.WriteAllText(path, contents);
	}

	private void LoopName()
	{
		while (!Process.HasExited)
		{
			try
			{
				string text = null;
				if (GetActiveWindowTitle() != null)
				{
					text = GetActiveWindowTitle().Replace(" - Google Chrome", "");
				}
				if (text != AccountSettings.Login)
				{
					CurrentTabName = text;
					SendMessage(Process.MainWindowHandle, 12, IntPtr.Zero, TITLEPOINTER);
				}
			}
			catch (Exception)
			{
			}
			Thread.Sleep(500);
		}
	}

	private void LoopTabs()
	{
		try
		{
			Dictionary<AutomationElement, string> dictionary = new Dictionary<AutomationElement, string>();
			AutomationElement automationElement = AutomationElement.FromHandle(Process.MainWindowHandle);
			AutomationElementCollection automationElementCollection = automationElement.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
			automationElement = TreeWalker.ControlViewWalker.GetParent(automationElementCollection[0]);
			while (!Process.HasExited)
			{
				try
				{
					bool flag = false;
					foreach (string windowHandle in WebDriver.WindowHandles)
					{
						if (!dictionary.ContainsValue(windowHandle))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						AutomationElementCollection automationElementCollection2 = automationElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
						dictionary.Add(automationElementCollection2[automationElementCollection2.Count - 1], WebDriver.WindowHandles[WebDriver.WindowHandles.Count - 1]);
					}
					string currentTabName = CurrentTabName;
					foreach (AutomationElement item in dictionary.Keys.Where((AutomationElement o) => o.Current.Name == "").ToList())
					{
						dictionary.Remove(item);
					}
					foreach (AutomationElement key in dictionary.Keys)
					{
						if (key.Current.Name == currentTabName)
						{
							ActiveHandle = dictionary[key];
							break;
						}
					}
				}
				catch (Exception)
				{
				}
				Thread.Sleep(100);
			}
		}
		catch
		{
		}
	}

	private void ResponseReceived(object sender, ResponseReceivedEventArgs e)
	{
		if (e.Type == ResourceType.XHR || e.Type == ResourceType.Fetch || e.Type == ResourceType.Document || e.Type == ResourceType.Other)
		{
			ResponseEvents.Enqueue(e);
		}
	}

	private void LoopResponse()
	{
		while (!Process.HasExited)
		{
			if (ResponseEvents.Count > 0)
			{
				GetResponseBodyAsync(ResponseEvents.Dequeue()).Wait();
			}
			else
			{
				Thread.Sleep(500);
			}
		}
	}

	private void LoopAutoPaste()
	{
		int millisecondsTimeout = 1000;
		string text = null;
		while (!Process.HasExited)
		{
			if (!string.IsNullOrEmpty(ActiveHandle) && ActiveHandle != WebDriver.CurrentWindowHandle)
			{
				WebDriver.SwitchTo().Window(ActiveHandle);
			}
			Thread.Sleep(millisecondsTimeout);
			if (!AccountSettings.EnableAutoFillingBrowser)
			{
				continue;
			}
			try
			{
				text = WebDriver.Url;
				if (text.Contains("1cupis.ru/auth") && !string.IsNullOrEmpty(AccountSettings.Cupis.Login))
				{
					IWebElement webElement = WebDriver.FindElement(By.XPath("/html/body/div/div/div[1]/div[1]/div/form/div[1]/div[1]/input"));
					string attribute = webElement.GetAttribute("value");
					if (string.IsNullOrEmpty(attribute) || attribute == "+7")
					{
						webElement.SendKeys(AccountSettings.Cupis.Login.Replace("+7", ""));
						Thread.Sleep(1000);
						webElement = WebDriver.FindElement(By.XPath("/html/body/div/div/div[1]/div[1]/div/form/div[1]/div[2]/input"));
						webElement.Click();
						attribute = webElement.GetAttribute("value");
						webElement = WebDriver.FindElement(By.XPath("/html/body/div/div/div[1]/div[1]/div/form/div[3]/label"));
						webElement.Click();
					}
					Thread.Sleep(millisecondsTimeout);
				}
				else if (AccountSettings.NameBCAccount == "_1Win" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement2 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[2]/div[1]/div/div[2]/div/div/form/div[3]/div[1]/div/div/input"));
					if (string.IsNullOrEmpty(webElement2.GetAttribute("value")))
					{
						webElement2.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						webElement2 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[2]/div[1]/div/div[2]/div/div/form/div[3]/div[2]/div/div[1]/input"));
						webElement2.GetAttribute("value");
						webElement2.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "_1xbet" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[1]/div/input"));
					string attribute2 = webElement3.GetAttribute("value");
					string attribute3 = webElement3.GetAttribute("name");
					if (string.IsNullOrEmpty(attribute2))
					{
						if (attribute3 == "auth_id_email" && !AccountSettings.Login.Contains("+7"))
						{
							webElement3.SendKeys(AccountSettings.Login);
							Thread.Sleep(500);
							webElement3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[2]/div/input"));
							attribute2 = webElement3.GetAttribute("value");
							webElement3.SendKeys(AccountSettings.Password);
						}
						else if (attribute3 == "auth_id_email" && AccountSettings.Login.Contains("+7"))
						{
							WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[1]/div/button")).Click();
							Thread.Sleep(500);
							webElement3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[1]/div/div/div[2]/input"));
							webElement3.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(500);
							webElement3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[2]/div/input"));
							attribute2 = webElement3.GetAttribute("value");
							webElement3.SendKeys(AccountSettings.Password);
						}
						WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[3]/div/label")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount == "_1xbit" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement4 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div/div/form/div[1]/div/div[1]/input"));
					webElement4.Click();
					if (string.IsNullOrEmpty(webElement4.GetAttribute("value")))
					{
						webElement4.SendKeys(AccountSettings.Login);
						Thread.Sleep(1000);
						webElement4 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div/div/form/div[2]/div/div[1]/input"));
						webElement4.GetAttribute("value");
						webElement4.SendKeys(AccountSettings.Password);
						WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div/div/form/div[3]/label/span[2]")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount == "Melbet" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement5 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[1]/div/div/div[1]/div/input"));
					webElement5.Click();
					Thread.Sleep(2000);
					webElement5 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[1]/div/div/div[1]/div/input"));
					webElement5.Click();
					if (string.IsNullOrEmpty(webElement5.GetAttribute("value")))
					{
						webElement5.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						webElement5 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[2]/div/div/div[1]/div/input"));
						webElement5.GetAttribute("value");
						webElement5.SendKeys(AccountSettings.Password);
					}
					WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[3]/div[1]/label/span[2]")).Click();
				}
				else if (AccountSettings.NameBCAccount == "Betwinner" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[2]/div/input"));
					string attribute4 = webElement6.GetAttribute("value");
					string attribute5 = webElement6.GetAttribute("placeholder");
					if (string.IsNullOrEmpty(attribute4))
					{
						if (attribute5 == "E-mail или ID" && !AccountSettings.Login.Contains("+7"))
						{
							webElement6.SendKeys(AccountSettings.Login);
							Thread.Sleep(500);
							webElement6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[3]/div/input"));
							attribute4 = webElement6.GetAttribute("value");
							webElement6.SendKeys(AccountSettings.Password);
						}
						else if (attribute5 == "E-mail или ID" && AccountSettings.Login.Contains("+7"))
						{
							WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[2]/div/button")).Click();
							Thread.Sleep(500);
							webElement6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[4]/div/div/input"));
							webElement6.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(500);
							webElement6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[5]/div/input"));
							attribute4 = webElement6.GetAttribute("value");
							webElement6.SendKeys(AccountSettings.Password);
						}
						WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[6]/div[1]/label")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount == "_1xstavka" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[1]/div/input"));
					string attribute6 = webElement7.GetAttribute("value");
					string attribute7 = webElement7.GetAttribute("name");
					if (string.IsNullOrEmpty(attribute6))
					{
						if (attribute7 == "auth_id_email" && !AccountSettings.Login.Contains("+7"))
						{
							webElement7.SendKeys(AccountSettings.Login);
							Thread.Sleep(500);
							webElement7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[2]/div/input"));
							attribute6 = webElement7.GetAttribute("value");
							webElement7.SendKeys(AccountSettings.Password);
						}
						else if (attribute7 == "auth_id_email" && AccountSettings.Login.Contains("+7"))
						{
							WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[1]/div/button")).Click();
							Thread.Sleep(500);
							webElement7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[1]/div/div/div[2]/input"));
							webElement7.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(500);
							webElement7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[2]/div/input"));
							attribute6 = webElement7.GetAttribute("value");
							webElement7.SendKeys(AccountSettings.Password);
						}
						WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[3]/div/label")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount.Contains("Parimatch") && text.Contains("/login"))
				{
					Thread.Sleep(2000);
					if (!string.IsNullOrEmpty(AccountSettings.Login))
					{
						IWebElement webElement8 = WebDriver.FindElements(By.TagName("input"))[0];
						string attribute8 = webElement8.GetAttribute("value");
						if (string.IsNullOrEmpty(attribute8) || attribute8 == "+7")
						{
							IWebElement webElement9 = WebDriver.FindElements(By.TagName("select"))[WebDriver.FindElements(By.TagName("select")).Count - 1];
							webElement9.Click();
							SelectElement selectElement = new SelectElement(webElement9);
							if (AccountSettings.Login.Contains("+7"))
							{
								selectElement.SelectByValue("phone");
							}
							else if (AccountSettings.Login.Contains("@"))
							{
								selectElement.SelectByValue("email");
							}
							else
							{
								selectElement.SelectByValue("id");
							}
							Thread.Sleep(200);
							webElement8.Click();
							webElement8.SendKeys(AccountSettings.Login);
							webElement8 = WebDriver.FindElements(By.TagName("input"))[1];
							webElement8.Click();
							webElement8.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "BaltbetRU" && text.Contains(AccountSettings.Host))
				{
					if (text == "https://old.baltbet.ru/")
					{
						if (!string.IsNullOrEmpty(AccountSettings.Login))
						{
							WebDriver.ExecuteJavaScript("document.querySelector('input#Login').value='" + AccountSettings.Login + "';");
						}
						if (!string.IsNullOrEmpty(AccountSettings.Password))
						{
							WebDriver.ExecuteJavaScript("document.querySelector('input#Password').value='" + AccountSettings.Password + "';");
						}
					}
					else if (text == "https://baltbet.ru/")
					{
						IWebElement webElement10 = WebDriver.FindElement(By.XPath("/html/body/div[2]/header/div/div[2]/div[3]/div[2]/div/div[1]/form/input[1]"));
						string attribute9 = webElement10.GetAttribute("value");
						if (string.IsNullOrEmpty(attribute9) || attribute9 == "+7")
						{
							webElement10.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							webElement10 = WebDriver.FindElement(By.XPath("/html/body/div[2]/header/div/div[2]/div[3]/div[2]/div/div[1]/form/input[2]"));
							attribute9 = webElement10.GetAttribute("value");
							webElement10.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "Betcity" && text.Contains("login"))
				{
					if (text.Contains("https://m."))
					{
						IWebElement webElement11 = WebDriver.FindElements(By.TagName("form"))[0];
						IWebElement webElement12;
						if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[0].Click();
							Thread.Sleep(500);
							webElement12 = webElement11.FindElements(By.TagName("input"))[0];
							webElement12.SendKeys(AccountSettings.Login);
						}
						else if (AccountSettings.Login.Length > 2 && AccountSettings.Login.Remove(2) == "+7")
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[2].Click();
							webElement12 = webElement11.FindElements(By.TagName("input"))[0];
							if (string.IsNullOrEmpty(webElement12.GetAttribute("value")))
							{
								webElement12.SendKeys(AccountSettings.Login);
							}
						}
						webElement12 = webElement11.FindElements(By.Name("pass"))[0].FindElement(By.TagName("input"));
						webElement12.Click();
						Thread.Sleep(500);
						if (string.IsNullOrEmpty(webElement12.GetAttribute("value")))
						{
							webElement12.SendKeys(AccountSettings.Password);
						}
					}
					else
					{
						IWebElement webElement13 = WebDriver.FindElements(By.TagName("form"))[0];
						IWebElement webElement14;
						if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[0].Click();
							Thread.Sleep(500);
							webElement14 = webElement13.FindElements(By.TagName("input"))[0];
							webElement14.SendKeys(AccountSettings.Login);
						}
						else if (AccountSettings.Login.Length > 2 && AccountSettings.Login.Remove(2) == "+7")
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[2].Click();
							webElement14 = webElement13.FindElements(By.TagName("input"))[0];
							if (string.IsNullOrEmpty(webElement14.GetAttribute("value")))
							{
								webElement14.SendKeys(AccountSettings.Login);
							}
						}
						webElement14 = webElement13.FindElements(By.TagName("input"))[1];
						webElement14.Click();
						Thread.Sleep(500);
						if (string.IsNullOrEmpty(webElement14.GetAttribute("value")))
						{
							webElement14.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "BetcityRU" && text.Contains("login"))
				{
					if (!text.Contains("https://m."))
					{
						IWebElement webElement15 = WebDriver.FindElements(By.TagName("form"))[0];
						IWebElement webElement16;
						string attribute10;
						if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[1].Click();
							Thread.Sleep(500);
							webElement16 = webElement15.FindElements(By.TagName("input"))[0];
							webElement16.SendKeys(AccountSettings.Login);
						}
						else if (AccountSettings.Login.Length > 2 && AccountSettings.Login.Remove(2) == "+7")
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[0].Click();
							webElement16 = webElement15.FindElements(By.TagName("input"))[0];
							attribute10 = webElement16.GetAttribute("value");
							if (string.IsNullOrEmpty(attribute10) || attribute10 == "+7")
							{
								webElement16.SendKeys(AccountSettings.Login.Replace("+7", ""));
							}
						}
						else
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[2].Click();
							webElement16 = webElement15.FindElements(By.TagName("input"))[0];
							attribute10 = webElement16.GetAttribute("value");
							if (string.IsNullOrEmpty(attribute10) || attribute10 == "+7")
							{
								webElement16.SendKeys(AccountSettings.Login.Replace("+7", ""));
							}
						}
						webElement16 = webElement15.FindElements(By.TagName("input"))[1];
						webElement16.Click();
						Thread.Sleep(500);
						attribute10 = webElement16.GetAttribute("value");
						for (int i = 0; i <= attribute10.Length; i++)
						{
							webElement16.SendKeys(Keys.Backspace);
						}
						if (string.IsNullOrEmpty(attribute10))
						{
							webElement16.SendKeys(AccountSettings.Password);
						}
						break;
					}
					IWebElement webElement17 = WebDriver.FindElements(By.TagName("form"))[0];
					IWebElement webElement18;
					if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
					{
						WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[1].Click();
						Thread.Sleep(500);
						webElement18 = webElement17.FindElements(By.TagName("input"))[1];
						webElement18.SendKeys(AccountSettings.Login);
					}
					else
					{
						webElement18 = webElement17.FindElements(By.TagName("input"))[0];
						webElement18.SendKeys(AccountSettings.Login.Replace("+7", ""));
					}
					webElement18 = webElement17.FindElements(By.Name("password"))[0].FindElement(By.TagName("input"));
					webElement18.Click();
					Thread.Sleep(500);
					string attribute11 = webElement18.GetAttribute("value");
					for (int j = 0; j <= attribute11.Length; j++)
					{
						webElement18.SendKeys(Keys.Backspace);
					}
					webElement18.SendKeys(AccountSettings.Password);
				}
				else if (AccountSettings.NameBCAccount == "LigaStavokRU")
				{
					try
					{
						IWebElement webElement19 = WebDriver.FindElement(By.Id("mobilePhone"));
						if (string.IsNullOrEmpty(webElement19.GetAttribute("value")))
						{
							webElement19.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							webElement19 = WebDriver.FindElement(By.Id("password"));
							webElement19.GetAttribute("value");
							webElement19.SendKeys(AccountSettings.Password);
						}
						break;
					}
					catch (Exception ex)
					{
						Console.WriteLine("LigaStavokRU  LoopAutoPaste" + ex.Message);
					}
				}
				else if (AccountSettings.NameBCAccount == "FonbetRU" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement20 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[1]/div[1]/div/input"));
					if (string.IsNullOrEmpty(webElement20.GetAttribute("value")))
					{
						webElement20.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						webElement20 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[2]/div[2]/div[1]/div/input"));
						webElement20.GetAttribute("value");
						webElement20.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "ParibetRU" && text.Contains(AccountSettings.Host))
				{
					try
					{
						WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div/div/span")).Click();
					}
					catch
					{
					}
					IWebElement webElement21 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[1]/div[1]/div/input"));
					if (string.IsNullOrEmpty(webElement21.GetAttribute("value")))
					{
						webElement21.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						webElement21 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[2]/div[2]/div[1]/div/input"));
						webElement21.GetAttribute("value");
						webElement21.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "BetteryRU" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement22 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[1]/div[1]/div/input"));
					if (string.IsNullOrEmpty(webElement22.GetAttribute("value")))
					{
						webElement22.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						webElement22 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[2]/div[2]/div[1]/div/input"));
						webElement22.GetAttribute("value");
						webElement22.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "MelbetRU" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement23 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/form/div[1]/div[1]/div/div[1]/input"));
					if (string.IsNullOrEmpty(webElement23.GetAttribute("value")))
					{
						webElement23.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						webElement23 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/form/div[1]/div[2]/div/div/input"));
						webElement23.GetAttribute("value");
						webElement23.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "Marathon" && text.Contains(AccountSettings.Host))
				{
					if (AccountSettings.Login.Contains("+7"))
					{
						IWebElement webElement24 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/button[1]"));
						webElement24.Click();
						Thread.Sleep(500);
						webElement24 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[1]/div/div/div[2]/div/div/div[1]/input"));
						webElement24.Click();
						if (string.IsNullOrEmpty(webElement24.GetAttribute("value")))
						{
							webElement24.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							webElement24 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div/div/div[1]/div[1]/input"));
							webElement24.GetAttribute("value");
							webElement24.SendKeys(AccountSettings.Password);
						}
					}
					else
					{
						IWebElement webElement24 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/button[2]"));
						webElement24.Click();
						Thread.Sleep(500);
						webElement24 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[2]/div/div[1]/div[1]/div/div/div/div/input"));
						webElement24.Click();
						if (string.IsNullOrEmpty(webElement24.GetAttribute("value")))
						{
							webElement24.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							webElement24 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[2]/div/div[1]/div[2]/div/div/div[1]/div[1]/input"));
							webElement24.GetAttribute("value");
							webElement24.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "MarathonRU" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement25 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[1]/div/div/div[1]/div[2]/input"));
					webElement25.Click();
					if (string.IsNullOrEmpty(webElement25.GetAttribute("value")))
					{
						webElement25.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						webElement25 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div/div/div[1]/div[1]/input"));
						webElement25.GetAttribute("value");
						webElement25.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "BetBoom" && text == "https://betboom.ru/")
				{
					try
					{
						IWebElement webElement26 = null;
						try
						{
							webElement26 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/header/div/div[2]/div[1]/div/div"));
						}
						catch
						{
						}
						if (webElement26 == null)
						{
							ReadOnlyCollection<IWebElement> readOnlyCollection = WebDriver.FindElements(By.TagName("input"));
							IWebElement webElement27 = readOnlyCollection[0];
							string attribute12 = webElement27.GetAttribute("value");
							if (string.IsNullOrEmpty(attribute12) || attribute12 == "+7 ___-___-__-__")
							{
								webElement27.Click();
								webElement27.SendKeys(AccountSettings.Login.Replace("+7", ""));
								Thread.Sleep(1500);
								webElement27 = readOnlyCollection[1];
								attribute12 = webElement27.GetAttribute("value");
								webElement27.SendKeys(AccountSettings.Password);
							}
						}
					}
					catch
					{
					}
				}
				else if (AccountSettings.NameBCAccount == "Leon" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement28 = WebDriver.FindElement(By.XPath("/html/body/div[3]/div/div/div[2]/div/div[1]/div/div[2]/form/div/div[1]/div/div/div/div[1]/div[2]/div/div[2]/div/input"));
					if (string.IsNullOrEmpty(webElement28.GetAttribute("value")))
					{
						webElement28.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						webElement28 = WebDriver.FindElement(By.XPath("/html/body/div[3]/div/div/div[2]/div/div[1]/div/div[2]/form/div/div[2]/div/div[1]/div[2]/div/div/input"));
						webElement28.GetAttribute("value");
						webElement28.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "LeonRU" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement29 = WebDriver.FindElement(By.XPath("/html/body/div[3]/div/div/div/div[2]/div/div[1]/div/div[2]/form/div[1]"));
					IWebElement webElement30 = webElement29.FindElements(By.TagName("input"))[0];
					webElement30.Click();
					string attribute13 = webElement30.GetAttribute("value");
					if (string.IsNullOrEmpty(attribute13) || attribute13 == "on")
					{
						for (int k = 0; k <= attribute13.Length; k++)
						{
							webElement30.SendKeys(Keys.Backspace);
						}
						webElement30.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						webElement30 = webElement29.FindElements(By.TagName("input"))[1];
						webElement30.Click();
						attribute13 = webElement30.GetAttribute("value");
						for (int l = 0; l <= attribute13.Length; l++)
						{
							webElement30.SendKeys(Keys.Backspace);
						}
						webElement30.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "Winline" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement31 = WebDriver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/cdk-dialog-container/ww-feature-login/div/div/form/div/div/ww-shared-phone-input/div/input[2]"));
					if (string.IsNullOrEmpty(webElement31.GetAttribute("value")))
					{
						webElement31.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						webElement31 = WebDriver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/cdk-dialog-container/ww-feature-login/div/div/form/div/div/div[1]/ww-shared-password-input/div/input"));
						webElement31.GetAttribute("value");
						webElement31.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "OlimpCom" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement32 = WebDriver.FindElement(By.XPath("/html/body/header/div[1]/div[2]/form/div/input[1]"));
					if (string.IsNullOrEmpty(webElement32.GetAttribute("value")))
					{
						webElement32.SendKeys(AccountSettings.Login);
						Thread.Sleep(1000);
						webElement32 = WebDriver.FindElement(By.XPath("/html/body/header/div[1]/div[2]/form/div/input[2]"));
						webElement32.GetAttribute("value");
						webElement32.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "OlimpBet" && text.Contains(AccountSettings.Host))
				{
					try
					{
						IWebElement webElement33 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/form/div[1]/div/input"));
						if (string.IsNullOrEmpty(webElement33.GetAttribute("value")))
						{
							webElement33.SendKeys(AccountSettings.Login.Replace("+7", ""));
						}
					}
					catch
					{
					}
					try
					{
						IWebElement webElement34 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/form/div[2]/div/input"));
						if (string.IsNullOrEmpty(webElement34.GetAttribute("value")))
						{
							webElement34.SendKeys(AccountSettings.Password);
						}
					}
					catch
					{
					}
				}
				else if (AccountSettings.NameBCAccount == "ZenitWin" && text.Contains(AccountSettings.Host))
				{
					IWebElement webElement35 = WebDriver.FindElement(By.XPath("/html/body/div[7]/main/div[2]/div[2]/div[9]/div[2]/div[2]/input"));
					if (string.IsNullOrEmpty(webElement35.GetAttribute("value")))
					{
						webElement35.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						webElement35 = WebDriver.FindElement(By.XPath("/html/body/div[7]/main/div[2]/div[2]/div[9]/div[3]/input"));
						webElement35.GetAttribute("value");
						webElement35.SendKeys(AccountSettings.Password);
					}
				}
			}
			catch
			{
			}
			Thread.Sleep(3000);
		}
	}

	private async Task GetResponseBodyAsync(ResponseReceivedEventArgs e)
	{
		try
		{
			GetResponseBodyCommandSettings getResponseBodyCommandSettings = new GetResponseBodyCommandSettings();
			getResponseBodyCommandSettings.RequestId = e.RequestId;
			GetResponseBodyCommandResponse getResponseBodyCommandResponse = await Domains.Network.GetResponseBody(getResponseBodyCommandSettings);
			if (getResponseBodyCommandResponse == null)
			{
				return;
			}
			if (AccountSettings.NameBCAccount.Contains("Parimatch"))
			{
				if (e.Response.Url.Contains("/RoutingLogin"))
				{
					JProperty jProperty = (from p in ((JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body)).Descendants().OfType<JProperty>()
						where p.Name == "token"
						select p).FirstOrDefault();
					if (jProperty != null)
					{
						AccountSettings.SetParameterSession("token", jProperty.Value.ToString());
					}
				}
				else if (e.Response.Url.Contains("/user/getAccountInfo"))
				{
					string valueParameter = (from p in ((JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body)).Descendants().OfType<JProperty>()
						where p.Name == "status"
						select p).First().Value.ToString();
					AccountSettings.SetParameterSession("status", valueParameter);
				}
			}
			else if (AccountSettings.NameBCAccount == "LigaStavokRU" && e.Response.Url.Contains("ligastavok"))
			{
				if (e.Response.Url.Contains("/getUserInfo"))
				{
					string valueParameter2 = (from p in ((JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body)).Descendants().OfType<JProperty>()
						where p.Name == "vipClient"
						select p).First().Value.ToString();
					AccountSettings.SetParameter("vipClient", valueParameter2);
				}
				else if (e.Response.Url.Contains("/getBalance"))
				{
					JProperty jProperty2 = (from p in ((JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body)).Descendants().OfType<JProperty>()
						where p.Name == "number"
						select p).FirstOrDefault();
					if (jProperty2 != null)
					{
						AccountSettings.SetParameter("AccountNumber", jProperty2.Value.ToString());
					}
				}
			}
			else if (AccountSettings.NameBCAccount == "BetBoom")
			{
				if (e.Response.Url.Contains("/api/auth/login"))
				{
					dynamic val = JObject.Parse(getResponseBodyCommandResponse.Body);
					string text = (string)val.token;
					string valueParameter3 = (string)val.refresh_token;
					if (!string.IsNullOrEmpty(text))
					{
						AccountSettings.SetParameterSession("x_access_token", text);
						AccountSettings.SetParameterSession("RefreshToken", valueParameter3);
					}
				}
			}
			else
			{
				if (AccountSettings.NameBCAccount == "BetcityRU")
				{
					return;
				}
				if (AccountSettings.NameBCAccount == "Betcity")
				{
					if (e.Response.Url.Contains("/auth?"))
					{
						string url = e.Response.Url;
						url = url.Remove(0, url.IndexOf("&dev") + 5);
						url = url.Remove(url.IndexOf("&"));
						AccountSettings.SetParameterSession("dev", url);
						string valueParameter4 = (from p in ((JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body)).Descendants().OfType<JProperty>()
							where p.Name == "block_status"
							select p).First().Value.ToString();
						AccountSettings.SetParameter("block_status", valueParameter4);
					}
				}
				else if (AccountSettings.NameBCAccount == "FonbetRU" || AccountSettings.NameBCAccount == "BetteryRU")
				{
					if (e.Response.Url.Contains("/session/info"))
					{
						JContainer obj = (JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body);
						int num = (int)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "limitGroup"
							select p).First().Value;
						int num2 = (int)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "betToBetDelay"
							select p).First().Value;
						bool flag = (bool)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "betBlocked"
							select p).First().Value;
						string valueParameter5 = (string?)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "clientId"
							select p).First().Value;
						string text2 = "";
						if (num2 != 0)
						{
							text2 = text2 + " ! (тайм.=" + num2 + ")";
						}
						if (flag)
						{
							text2 += " ! БЛОК !";
						}
						if (!flag && num2 == 0 && num == 1)
						{
							text2 += " (проверен!)";
						}
						AccountSettings.SetParameter("commentDisplay", AccountSettings.Login + text2);
						AccountSettings.SetParameter("clientId", valueParameter5);
					}
					else if (e.Response.Url.Contains("/session/login"))
					{
						JContainer obj2 = (JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body);
						string valueParameter6 = obj2["session"]["client"].ToString();
						string valueParameter7 = obj2["session"]["fsid"].ToString();
						AccountSettings.SetParameter("clientId", valueParameter6);
						AccountSettings.SetParameterSession("fsid", valueParameter7);
					}
				}
				else
				{
					if (!(AccountSettings.NameBCAccount == "ParibetRU"))
					{
						return;
					}
					if (e.Response.Url.Contains("/session/info"))
					{
						JContainer obj3 = (JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body);
						int num3 = (int)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "limitGroup"
							select p).First().Value;
						int num4 = (int)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "betToBetDelay"
							select p).First().Value;
						bool flag2 = (bool)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "betBlocked"
							select p).First().Value;
						string valueParameter8 = (string?)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "clientId"
							select p).First().Value;
						string text3 = "";
						if (num4 != 0)
						{
							text3 = text3 + " ! (тайм.=" + num4 + ")";
						}
						if (flag2)
						{
							text3 += " ! БЛОК !";
						}
						if (!flag2 && num4 == 0 && num3 == 1)
						{
							text3 += " (проверен!)";
						}
						AccountSettings.SetParameter("commentDisplay", AccountSettings.Login + text3);
						AccountSettings.SetParameter("clientId", valueParameter8);
					}
					else if (e.Response.Url.Contains("/session/login"))
					{
						JContainer obj4 = (JContainer)JsonConvert.DeserializeObject(getResponseBodyCommandResponse.Body);
						string valueParameter9 = obj4["session"]["client"].ToString();
						string valueParameter10 = obj4["session"]["fsid"].ToString();
						AccountSettings.SetParameter("clientId", valueParameter9);
						AccountSettings.SetParameterSession("fsid", valueParameter10);
					}
				}
			}
		}
		catch
		{
		}
	}

	public void Navigate(string url)
	{
		Task.Run((Action)LoopAutoPaste);
		try
		{
			WebDriver.Navigate().GoToUrl(url);
			if (!(AccountSettings.NameBCAccount == "LigaStavokRU"))
			{
				return;
			}
			WebDriver.Navigate().GoToUrl("https://ligastavok.ru");
			Thread.Sleep(5000);
			string userAgent = (string)ExecuteScript("return navigator.userAgent;");
			CookieCollection cookies = QratorCookies.GetCookies("https://www.ligastavok.ru/", userAgent, AccountSettings.Proxy.webProxy);
			Thread.Sleep(2000);
			foreach (System.Net.Cookie item in cookies)
			{
				WebDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(item.Name, item.Value, ".ligastavok.ru", "/", DateTime.Now.AddDays(1.0)));
				Thread.Sleep(500);
			}
			Thread.Sleep(1000);
			WebDriver.Navigate().Refresh();
		}
		catch
		{
		}
	}

	private void BreakThroughQrator()
	{
		Thread.Sleep(5000);
		CookieContainer = new CookieContainer();
		GETResponse("http://www.ligastavok.ru/");
		GETResponse("https://ligastavok.ru/__qrator/qauth_utm_v2.js");
		GETResponse("https://ligastavok.ru/favicon.ico");
		string value = CookieContainer.GetCookies(new Uri("https://www.ligastavok.ru/"))[0].Value;
		string text = value.Split('-')[0];
		string text2 = value.Split('-')[1];
		int pow = GetPow();
		string url = "https://ligastavok.ru/__qrator/validate?pow=" + pow + "&nonce=" + text + "&qsessid=" + text2;
		UpdateJsid(url);
		CookieCollection cookies = CookieContainer.GetCookies(new Uri("https://www.ligastavok.ru/"));
		if (cookies == null)
		{
			return;
		}
		foreach (System.Net.Cookie item in cookies)
		{
			try
			{
				WebDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(item.Name, item.Value, item.Domain, "", null));
			}
			catch
			{
				Console.WriteLine("This cookie not set: " + item.Name);
			}
		}
	}

	private void OpenDevTools()
	{
		WebDriver.FindElement(By.CssSelector("body")).SendKeys(Keys.F12);
	}

	private void UpdateJsid(string url)
	{
		POSTResponse(url);
	}

	private string GETResponse(string url)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "GET";
		httpWebRequest.Headers.Add("Accept-Encoding: gzip, deflate, br");
		httpWebRequest.Headers.Add("Accept-Language: ru,en;q=0.9");
		httpWebRequest.Headers.Add("Origin: https://www.ligastavok.ru");
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = AccountSettings.UserAgent;
		httpWebRequest.Referer = "https://www.ligastavok.ru/";
		httpWebRequest.CookieContainer = CookieContainer;
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		httpWebRequest.KeepAlive = true;
		if (!AccountSettings.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = AccountSettings.Proxy.webProxy;
		}
		httpWebRequest.ContinueTimeout = 10000;
		ServicePoint servicePoint = httpWebRequest.ServicePoint;
		servicePoint.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(servicePoint, (byte)0, null);
		string text = "";
		try
		{
			WebResponse response = httpWebRequest.GetResponse();
			text = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return text;
	}

	private string POSTResponse(string url)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "POST";
		httpWebRequest.Headers.Add("Accept-Encoding: gzip, deflate, br");
		httpWebRequest.Headers.Add("Accept-Language: ru,en;q=0.9");
		httpWebRequest.Headers.Add("Origin: https://ligastavok.ru");
		httpWebRequest.Accept = "*/*";
		string userAgent = (httpWebRequest.UserAgent = AccountSettings.UserAgent);
		httpWebRequest.UserAgent = userAgent;
		httpWebRequest.CookieContainer = CookieContainer;
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		httpWebRequest.KeepAlive = true;
		if (!AccountSettings.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = AccountSettings.Proxy.webProxy;
		}
		httpWebRequest.ServicePoint.ConnectionLimit = 5;
		httpWebRequest.ContinueTimeout = 10000;
		httpWebRequest.ServicePoint.Expect100Continue = false;
		ServicePoint servicePoint = httpWebRequest.ServicePoint;
		servicePoint.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(servicePoint, (byte)0, null);
		string text = "";
		try
		{
			StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
			streamWriter.Write(0);
			streamWriter.Flush();
			WebResponse response = httpWebRequest.GetResponse();
			text = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return text;
	}

	private int GetPow()
	{
		int num = 2;
		int num2 = 0;
		string text = "";
		string text2 = CookieContainer.GetCookies(new Uri("https://ligastavok.ru"))[0].Value.Split('-')[0];
		for (int i = 0; i < 10000; i++)
		{
			switch (num)
			{
			case 2:
			{
				num2++;
				string input = text2 + num2;
				text = CreateMD5(input);
				if (num2 % 1000 != 0)
				{
					num = 8;
				}
				break;
			}
			case 8:
				if (text.Substring(0, 2) != "00")
				{
					num = 2;
					break;
				}
				return num2;
			}
		}
		return -1;
	}

	public string CreateMD5(string input)
	{
		using MD5 mD = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	private string GetActiveWindowTitle()
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		if (GetWindowText(Process.MainWindowHandle, stringBuilder, 256) > 0)
		{
			return stringBuilder.ToString();
		}
		return null;
	}

	public async Task<IWebElement> GetElementByXPathAsync(string xPath, int timeout = 10000)
	{
		return await SearchAsyncOperation(() => WebDriver.FindElement(By.XPath(xPath)), timeout);
	}

	private async Task<IWebElement> SearchAsyncOperation(Func<IWebElement> action, int timeout)
	{
		IWebElement result = null;
		await Task.Run(delegate
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (true)
			{
				try
				{
					result = action();
					break;
				}
				catch
				{
					if (stopwatch.ElapsedMilliseconds > timeout)
					{
						break;
					}
					Thread.Sleep(500);
				}
			}
		});
		return result;
	}

	public object ExecuteScript(string script)
	{
		return ((IJavaScriptExecutor)WebDriver).ExecuteScript(script);
	}

	public object ExecuteScriptAsync(string script)
	{
		return ((IJavaScriptExecutor)WebDriver).ExecuteAsyncScript(script);
	}

	public static void KillDeadProcesses()
	{
		Process[] processesByName = Process.GetProcessesByName("chrome");
		foreach (Process process in processesByName)
		{
			if (process.MainModule.FileName.Contains("ChromeDriver"))
			{
				try
				{
					process.Kill();
				}
				catch
				{
				}
			}
		}
		processesByName = Process.GetProcessesByName("chromedriver");
		foreach (Process process2 in processesByName)
		{
			if (process2.MainModule.FileName.Contains("ChromeDriver"))
			{
				try
				{
					process2.Kill();
				}
				catch
				{
				}
			}
		}
		Thread.Sleep(1000);
	}

	public CookieCollection GetCookieCollection(AccountSettings accountSettings)
	{
		CookieCollection cookieCollection = new CookieCollection();
		if (File.Exists(CookieFile))
		{
			string[] array = File.ReadAllLines(CookieFile);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('•');
				string domain = array2[0];
				string name = array2[1];
				string value = array2[2];
				string path = array2[3];
				string value2 = array2[4];
				string value3 = array2[5];
				_ = array2[6];
				try
				{
					System.Net.Cookie cookie = new System.Net.Cookie();
					cookie.Name = name;
					cookie.Value = value;
					cookie.Domain = domain;
					cookie.Path = path;
					cookie.HttpOnly = Convert.ToBoolean(value2);
					cookie.Secure = Convert.ToBoolean(value3);
					cookieCollection.Add(cookie);
				}
				catch (Exception ex)
				{
					Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [BrowserAB.GetCookieCollection] error. " + ex.Message);
				}
			}
		}
		return cookieCollection;
	}
}
