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
		string login = AccountSettings.Login;
		if (!string.IsNullOrEmpty(login) && login.Length == 12 && login.Remove(2) == "+7")
		{
			login = $"{long.Parse(login):+# ### ### ####}";
		}
		TITLEPOINTER = Marshal.StringToHGlobalUni(login);
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
		ChromeOptions options = new ChromeOptions();
		options.AddArguments("user-data-dir=" + ProfilePath);
		options.AddArgument("ignore-certificate-errors");
		options.AddArgument("--disable-blink-features");
		options.AddArgument("--disable-blink-features=AutomationControlled");
		if (AccountSettings.NameBCAccount != "MelbetRU" && AccountSettings.NameBCAccount != "LigaStavokRU" && AccountSettings.NameBCAccount != "BetBoom" && !AccountSettings.NameBCAccount.Contains("Leon") && !AccountSettings.NameBCAccount.Contains("Betcity"))
		{
			options.AddArguments("--user-agent=" + accountSettings.UserAgent);
		}
		else
		{
			options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
		}
		options.AddArgument("--check-for-update-interval=999999999");
		options.AddExtension(Environment.CurrentDirectory + "/extension.crx");
		options.AddExcludedArguments(new List<string> { "enable-automation" });
		options.AddArgument("--no-sandbox");
		options.AddArgument("--disable-infobars");
		options.AddArgument("--disable-dev-shm-usage");
		options.AddArgument("--disable-browser-side-navigation");
		options.AddArgument("--disable-gpu");
		options.AddArgument("--log-level=3");
		options.BinaryLocation = DEFAULT_CHROME_BINARY_PATH;
		options.PageLoadStrategy = PageLoadStrategy.None;
		double width = Math.Min(Math.Floor(SystemParameters.PrimaryScreenWidth / 1.3), 2000.0);
		double height = Math.Floor(SystemParameters.PrimaryScreenHeight / 1.1);
		double left = Math.Floor(SystemParameters.WorkArea.Width - width);
		double top = Math.Floor(SystemParameters.WorkArea.Height) - height;
		options.AddArguments("--window-size=" + width + "," + height);
		options.AddArguments("--window-position=" + left + "," + top);
		ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(DEFAULT_WEBDRIVER_DIRECTORY);
		driverService.HideCommandPromptWindow = true;
		driverService.Port = driverServicePort;
		driverServicePort++;
		WebDriver = new ChromeDriver(driverService, options);
		Process = GetWebBrowserProcess(driverService.ProcessId);
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
			int widthBr = 1600;
			int heightBr = 900;
			int CPUCORES = 8;
			string GPU = gpuList[random.Next(0, gpuList.Length)];
			string GPUVENDOR = (GPU.Contains("AMD") ? "Google (AMD)" : "Google (Nvidia)");
			string VENDOR = "WebKit";
			string RENDERER = "WebKit WebGL";
			string GLVERSION = "Version 1.0";
			string PNGDATA = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAANwAAAAeCAAAAABiES/iAAACeElEQVRYw+2YzUtUURjGf47OmDPh5AyFomUiEeEmyghXtWsh4dcswlYV2KYWfZh/QRBUVLhTCCJXEgmKUCIkFhJREARBkbkyKBlTRmUC82lxZ7z3TjM4whwXwz2ry3vO87znx33Pey4XFfHAg/PgPDgPzoPz4Dy4rFIKscSkAfmnsUY+iTfXFhxue4Zm4QpfaKbg8k+EsZNsGG6iNVzRMrkZeRPmjp6eCgcae5f+3wJIgtWLldG+DUnfzoail1etaVsEa1f2lUqw2hPd3T7nCrkMtlkQ24YDwP8+FZkI+gY3uq2cTcu54GIA/dJCDUAnSE4RdAESdALUxZ0hl4E5OMs49iE528E5a+cj5YFhDVI3vLA2c4K+zLXpvR37tNRDs3STg1OJqXqQSwS14wlJUD+VeHWAW86Qy8BwQ5Ek/WK/JBgqC72UTvJakmY5lAvurTRPSDrMmKRRcIvgeUo2KmmEI86Qy8DwmVu/ezQIBCSBLzwjKZhujv5cZZmUNkAq57ekRXCLYDG12pre5Qy5DAzDXbPfIOB/JqmCzNafCZd+dMA5RfZxdsBlNTAMF+FJfD2eSvSI0iGpmXe5GnbG3qyyHAO3yCZxlGV2uBLWDcJVMZKc7UrnfIBvQI+pHpxbS34ZaNkK7gYN0yvTDSCXyCZxNJTscFFe/DUH1w3QvpnzPiUPdTXfsvxZDdBGmeQU2SQd9lWQHS5m9J6Ln4/suZCwc96D25qM1formq5/3ApOX1uDkZ7P7JXkENkkK5eqQm3flRtuvitSYgCucKOf0zv01bazcG3Tyz8GKukvSjjrlB3/U5Rw42dqAo29yypKOO8figeX1/gH+zX9JqfOeUwAAAAASUVORK5CYII=";
			bool useCanvasSpoof = false;
			string script = "WebGLRenderingContext.prototype.getParameter = function(origFn) { const paramMap = {};  paramMap[0x9245] = \"" + GPUVENDOR + "\";paramMap[0x9246] = \"" + GPU + "\";paramMap[0x1F00] = \"" + VENDOR + "\";paramMap[0x1F01] = \"" + RENDERER + "\";paramMap[0x1F02] = \"" + GLVERSION + "\";return function(parameter) { return paramMap[parameter] || origFn.call(this, parameter);};} (WebGLRenderingContext.prototype.getParameter);Object.defineProperties(navigator, { hardwareConcurrency: { value: " + CPUCORES + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { availHeight: { value: " + heightBr + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { availWidth: { value: " + widthBr + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { height: { value: " + heightBr + ", configurable: false, enumerable: true, writable: false }});Object.defineProperties(window.screen, { width: { value: " + widthBr + ", configurable: false, enumerable: true, writable: false }});window.cdc_adoQpoasnfa76pfcZLmcfl_Symbol = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Object = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Promise = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Array = null;window.cdc_adoQpoasnfa76pfcZLmcfl_Proxy = null;";
			if (useCanvasSpoof)
			{
				script = script + "const mainFunction = HTMLCanvasElement.prototype.toDataURL; HTMLCanvasElement.prototype.toDataURL = function (type) { if (true) {return '" + PNGDATA + "';} return mainFunction.apply(this, arguments);};";
			}
			AccountSettings.ScriptBrowser = script;
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
		StringBuilder builder = new StringBuilder();
		foreach (OpenQA.Selenium.Cookie item in (IEnumerable<OpenQA.Selenium.Cookie>)allCookies)
		{
			builder.Append(item.Domain + "•" + item.Name + "•" + item.Value + "•" + item.Path + "•" + item.IsHttpOnly + "•" + item.Secure + "•" + item.Expiry.ToString() + "\r\n");
		}
		File.WriteAllText(CookieFile, builder.ToString().TrimEnd());
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
		string pathExt = Path.Combine(Environment.CurrentDirectory, "ChromeDriver\\Ext\\");
		if (File.Exists(pathExt + "manifest.json") && File.Exists(pathExt + "background_template.js"))
		{
			string script = File.ReadAllText(pathExt + "background_template.js");
			script = (AccountSettings.Proxy.IsEmpty ? script.Replace("MODE", "direct") : script.Replace("MODE", "fixed_servers"));
			script = script.Replace("PROXY_HOST", host);
			script = script.Replace("PROXY_PORT", port.ToString());
			script = script.Replace("PROXY_USERNAME", userName);
			script = script.Replace("PROXY_PASSWORD", password);
			StreamWriter streamWriter = File.CreateText(pathExt + "background.js");
			streamWriter.Write(script);
			streamWriter.Close();
			if (File.Exists(Environment.CurrentDirectory + "/extension.crx"))
			{
				File.Delete(Environment.CurrentDirectory + "/extension.crx");
			}
			ZipFile.CreateFromDirectory(pathExt, Environment.CurrentDirectory + "/extension.crx");
		}
	}

	private void StartTitaniumProxyServer(string externalProxyHost = "", int externalProxyPort = -1, string externalProxyUsername = "", string externalProxyPassword = "")
	{
		TitaniumProxyServer.CertificateManager.CertificateEngine = CertificateEngine.DefaultWindows;
		TitaniumProxyServer.CertificateManager.EnsureRootCertificate();
		TitaniumProxyServer.CertificateManager.TrustRootCertificate(machineTrusted: true);
		ExplicitProxyEndPoint explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, numPortTitaniumProxy);
		TitaniumProxyServer.AddEndPoint(explicitEndPoint);
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
		string prefsPath = ProfilePath + "/Default/Preferences";
		if (!File.Exists(prefsPath))
		{
			return;
		}
		string preferences = File.ReadAllText(prefsPath);
		int a = preferences.IndexOf("countryid_at_install\":");
		a = preferences.IndexOf(",", a) + 1;
		int b = preferences.IndexOf("\"data_reduction");
		if (b == -1)
		{
			return;
		}
		string head = preferences.Substring(0, a);
		string tail = preferences.Substring(b, preferences.Length - b);
		string body = "\"custom_links\":{\"initialized\":true,\"list\":[";
		foreach (KeyValuePair<string, string> item in sites)
		{
			string title = item.Key;
			string url = item.Value;
			body += "{";
			body += "\"isMostVisited\":false";
			body += ",";
			body = body + "\"title\":\"" + title + "\"";
			body += ",";
			body = body + "\"url\":\"" + url + "\"";
			body += "},";
		}
		body = body.TrimEnd(',');
		body += "]},";
		string res = head + body + tail;
		File.WriteAllText(prefsPath, res);
	}

	private void LoopName()
	{
		while (!Process.HasExited)
		{
			try
			{
				string currentName = null;
				if (GetActiveWindowTitle() != null)
				{
					currentName = GetActiveWindowTitle().Replace(" - Google Chrome", "");
				}
				if (currentName != AccountSettings.Login)
				{
					CurrentTabName = currentName;
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
			Dictionary<AutomationElement, string> tabs = new Dictionary<AutomationElement, string>();
			AutomationElement root = AutomationElement.FromHandle(Process.MainWindowHandle);
			AutomationElementCollection tabsElements = root.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
			root = TreeWalker.ControlViewWalker.GetParent(tabsElements[0]);
			while (!Process.HasExited)
			{
				try
				{
					bool isDirty = false;
					foreach (string item in WebDriver.WindowHandles)
					{
						if (!tabs.ContainsValue(item))
						{
							isDirty = true;
							break;
						}
					}
					if (isDirty)
					{
						AutomationElementCollection a = root.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
						tabs.Add(a[a.Count - 1], WebDriver.WindowHandles[WebDriver.WindowHandles.Count - 1]);
					}
					string title = CurrentTabName;
					foreach (AutomationElement item2 in tabs.Keys.Where((AutomationElement o) => o.Current.Name == "").ToList())
					{
						tabs.Remove(item2);
					}
					foreach (AutomationElement item3 in tabs.Keys)
					{
						if (item3.Current.Name == title)
						{
							ActiveHandle = tabs[item3];
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
		int timerSleep = 1000;
		string urlWebDriver = null;
		while (!Process.HasExited)
		{
			if (!string.IsNullOrEmpty(ActiveHandle) && ActiveHandle != WebDriver.CurrentWindowHandle)
			{
				WebDriver.SwitchTo().Window(ActiveHandle);
			}
			Thread.Sleep(timerSleep);
			if (!AccountSettings.EnableAutoFillingBrowser)
			{
				continue;
			}
			try
			{
				urlWebDriver = WebDriver.Url;
				if (urlWebDriver.Contains("1cupis.ru/auth") && !string.IsNullOrEmpty(AccountSettings.Cupis.Login))
				{
					IWebElement element = WebDriver.FindElement(By.XPath("/html/body/div/div/div[1]/div[1]/div/form/div[1]/div[1]/input"));
					string valueElement = element.GetAttribute("value");
					if (string.IsNullOrEmpty(valueElement) || valueElement == "+7")
					{
						element.SendKeys(AccountSettings.Cupis.Login.Replace("+7", ""));
						Thread.Sleep(1000);
						element = WebDriver.FindElement(By.XPath("/html/body/div/div/div[1]/div[1]/div/form/div[1]/div[2]/input"));
						element.Click();
						valueElement = element.GetAttribute("value");
						element = WebDriver.FindElement(By.XPath("/html/body/div/div/div[1]/div[1]/div/form/div[3]/label"));
						element.Click();
					}
					Thread.Sleep(timerSleep);
				}
				else if (AccountSettings.NameBCAccount == "_1Win" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element2 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[2]/div[1]/div/div[2]/div/div/form/div[3]/div[1]/div/div/input"));
					if (string.IsNullOrEmpty(element2.GetAttribute("value")))
					{
						element2.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						element2 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[2]/div[1]/div/div[2]/div/div/form/div[3]/div[2]/div/div[1]/input"));
						element2.GetAttribute("value");
						element2.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "_1xbet" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[1]/div/input"));
					string valueElement2 = element3.GetAttribute("value");
					string typeAuth = element3.GetAttribute("name");
					if (string.IsNullOrEmpty(valueElement2))
					{
						if (typeAuth == "auth_id_email" && !AccountSettings.Login.Contains("+7"))
						{
							element3.SendKeys(AccountSettings.Login);
							Thread.Sleep(500);
							element3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[2]/div/input"));
							valueElement2 = element3.GetAttribute("value");
							element3.SendKeys(AccountSettings.Password);
						}
						else if (typeAuth == "auth_id_email" && AccountSettings.Login.Contains("+7"))
						{
							WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[1]/div/button")).Click();
							Thread.Sleep(500);
							element3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[1]/div/div/div[2]/input"));
							element3.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(500);
							element3 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[2]/div/input"));
							valueElement2 = element3.GetAttribute("value");
							element3.SendKeys(AccountSettings.Password);
						}
						WebDriver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[4]/div[2]/div/div/div[2]/div/form/div[3]/div/label")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount == "_1xbit" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element4 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div/div/form/div[1]/div/div[1]/input"));
					element4.Click();
					if (string.IsNullOrEmpty(element4.GetAttribute("value")))
					{
						element4.SendKeys(AccountSettings.Login);
						Thread.Sleep(1000);
						element4 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div/div/form/div[2]/div/div[1]/input"));
						element4.GetAttribute("value");
						element4.SendKeys(AccountSettings.Password);
						WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div/div/form/div[3]/label/span[2]")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount == "Melbet" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element5 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[1]/div/div/div[1]/div/input"));
					element5.Click();
					Thread.Sleep(2000);
					element5 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[1]/div/div/div[1]/div/input"));
					element5.Click();
					if (string.IsNullOrEmpty(element5.GetAttribute("value")))
					{
						element5.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						element5 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[2]/div/div/div[1]/div/input"));
						element5.GetAttribute("value");
						element5.SendKeys(AccountSettings.Password);
					}
					WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[1]/div/div/div[2]/div/div/div/div/div[1]/form/div[3]/div[1]/label/span[2]")).Click();
				}
				else if (AccountSettings.NameBCAccount == "Betwinner" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[2]/div/input"));
					string valueElement3 = element6.GetAttribute("value");
					string placeholder = element6.GetAttribute("placeholder");
					if (string.IsNullOrEmpty(valueElement3))
					{
						if (placeholder == "E-mail или ID" && !AccountSettings.Login.Contains("+7"))
						{
							element6.SendKeys(AccountSettings.Login);
							Thread.Sleep(500);
							element6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[3]/div/input"));
							valueElement3 = element6.GetAttribute("value");
							element6.SendKeys(AccountSettings.Password);
						}
						else if (placeholder == "E-mail или ID" && AccountSettings.Login.Contains("+7"))
						{
							WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[2]/div/button")).Click();
							Thread.Sleep(500);
							element6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[4]/div/div/input"));
							element6.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(500);
							element6 = WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[5]/div/input"));
							valueElement3 = element6.GetAttribute("value");
							element6.SendKeys(AccountSettings.Password);
						}
						WebDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/header/div/span[2]/div/div[2]/div/div/div/form/div[6]/div[1]/label")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount == "_1xstavka" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[1]/div/input"));
					string valueElement4 = element7.GetAttribute("value");
					string typeAuth2 = element7.GetAttribute("name");
					if (string.IsNullOrEmpty(valueElement4))
					{
						if (typeAuth2 == "auth_id_email" && !AccountSettings.Login.Contains("+7"))
						{
							element7.SendKeys(AccountSettings.Login);
							Thread.Sleep(500);
							element7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[2]/div/input"));
							valueElement4 = element7.GetAttribute("value");
							element7.SendKeys(AccountSettings.Password);
						}
						else if (typeAuth2 == "auth_id_email" && AccountSettings.Login.Contains("+7"))
						{
							WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[1]/div/button")).Click();
							Thread.Sleep(500);
							element7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[1]/div/div/div[2]/input"));
							element7.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(500);
							element7 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[2]/div/input"));
							valueElement4 = element7.GetAttribute("value");
							element7.SendKeys(AccountSettings.Password);
						}
						WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div[2]/div/div/div/div[2]/div/form/div[3]/div/label")).Click();
					}
				}
				else if (AccountSettings.NameBCAccount.Contains("Parimatch") && urlWebDriver.Contains("/login"))
				{
					Thread.Sleep(2000);
					if (!string.IsNullOrEmpty(AccountSettings.Login))
					{
						IWebElement element8 = WebDriver.FindElements(By.TagName("input"))[0];
						string valueElement5 = element8.GetAttribute("value");
						if (string.IsNullOrEmpty(valueElement5) || valueElement5 == "+7")
						{
							IWebElement webElement = WebDriver.FindElements(By.TagName("select"))[WebDriver.FindElements(By.TagName("select")).Count - 1];
							webElement.Click();
							SelectElement selector = new SelectElement(webElement);
							if (AccountSettings.Login.Contains("+7"))
							{
								selector.SelectByValue("phone");
							}
							else if (AccountSettings.Login.Contains("@"))
							{
								selector.SelectByValue("email");
							}
							else
							{
								selector.SelectByValue("id");
							}
							Thread.Sleep(200);
							element8.Click();
							element8.SendKeys(AccountSettings.Login);
							element8 = WebDriver.FindElements(By.TagName("input"))[1];
							element8.Click();
							element8.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "BaltbetRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					if (urlWebDriver == "https://old.baltbet.ru/")
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
					else if (urlWebDriver == "https://baltbet.ru/")
					{
						IWebElement element9 = WebDriver.FindElement(By.XPath("/html/body/div[2]/header/div/div[2]/div[3]/div[2]/div/div[1]/form/input[1]"));
						string valueElement6 = element9.GetAttribute("value");
						if (string.IsNullOrEmpty(valueElement6) || valueElement6 == "+7")
						{
							element9.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							element9 = WebDriver.FindElement(By.XPath("/html/body/div[2]/header/div/div[2]/div[3]/div[2]/div/div[1]/form/input[2]"));
							valueElement6 = element9.GetAttribute("value");
							element9.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "Betcity" && urlWebDriver.Contains("login"))
				{
					if (urlWebDriver.Contains("https://m."))
					{
						IWebElement form = WebDriver.FindElements(By.TagName("form"))[0];
						IWebElement element10;
						if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[0].Click();
							Thread.Sleep(500);
							element10 = form.FindElements(By.TagName("input"))[0];
							element10.SendKeys(AccountSettings.Login);
						}
						else if (AccountSettings.Login.Length > 2 && AccountSettings.Login.Remove(2) == "+7")
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[2].Click();
							element10 = form.FindElements(By.TagName("input"))[0];
							if (string.IsNullOrEmpty(element10.GetAttribute("value")))
							{
								element10.SendKeys(AccountSettings.Login);
							}
						}
						element10 = form.FindElements(By.Name("pass"))[0].FindElement(By.TagName("input"));
						element10.Click();
						Thread.Sleep(500);
						if (string.IsNullOrEmpty(element10.GetAttribute("value")))
						{
							element10.SendKeys(AccountSettings.Password);
						}
					}
					else
					{
						IWebElement form2 = WebDriver.FindElements(By.TagName("form"))[0];
						IWebElement element11;
						if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[0].Click();
							Thread.Sleep(500);
							element11 = form2.FindElements(By.TagName("input"))[0];
							element11.SendKeys(AccountSettings.Login);
						}
						else if (AccountSettings.Login.Length > 2 && AccountSettings.Login.Remove(2) == "+7")
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[2].Click();
							element11 = form2.FindElements(By.TagName("input"))[0];
							if (string.IsNullOrEmpty(element11.GetAttribute("value")))
							{
								element11.SendKeys(AccountSettings.Login);
							}
						}
						element11 = form2.FindElements(By.TagName("input"))[1];
						element11.Click();
						Thread.Sleep(500);
						if (string.IsNullOrEmpty(element11.GetAttribute("value")))
						{
							element11.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "BetcityRU" && urlWebDriver.Contains("login"))
				{
					if (!urlWebDriver.Contains("https://m."))
					{
						IWebElement form3 = WebDriver.FindElements(By.TagName("form"))[0];
						IWebElement element12;
						string valueElement7;
						if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[1].Click();
							Thread.Sleep(500);
							element12 = form3.FindElements(By.TagName("input"))[0];
							element12.SendKeys(AccountSettings.Login);
						}
						else if (AccountSettings.Login.Length > 2 && AccountSettings.Login.Remove(2) == "+7")
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[0].Click();
							element12 = form3.FindElements(By.TagName("input"))[0];
							valueElement7 = element12.GetAttribute("value");
							if (string.IsNullOrEmpty(valueElement7) || valueElement7 == "+7")
							{
								element12.SendKeys(AccountSettings.Login.Replace("+7", ""));
							}
						}
						else
						{
							WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[2].Click();
							element12 = form3.FindElements(By.TagName("input"))[0];
							valueElement7 = element12.GetAttribute("value");
							if (string.IsNullOrEmpty(valueElement7) || valueElement7 == "+7")
							{
								element12.SendKeys(AccountSettings.Login.Replace("+7", ""));
							}
						}
						element12 = form3.FindElements(By.TagName("input"))[1];
						element12.Click();
						Thread.Sleep(500);
						valueElement7 = element12.GetAttribute("value");
						for (int i = 0; i <= valueElement7.Length; i++)
						{
							element12.SendKeys(Keys.Backspace);
						}
						if (string.IsNullOrEmpty(valueElement7))
						{
							element12.SendKeys(AccountSettings.Password);
						}
						break;
					}
					IWebElement form4 = WebDriver.FindElements(By.TagName("form"))[0];
					IWebElement element13;
					if (AccountSettings.Login.Remove(2) == "79" || AccountSettings.Login.Any(char.IsLetter))
					{
						WebDriver.FindElement(By.ClassName("login__tabs")).FindElements(By.TagName("div"))[1].Click();
						Thread.Sleep(500);
						element13 = form4.FindElements(By.TagName("input"))[1];
						element13.SendKeys(AccountSettings.Login);
					}
					else
					{
						element13 = form4.FindElements(By.TagName("input"))[0];
						element13.SendKeys(AccountSettings.Login.Replace("+7", ""));
					}
					element13 = form4.FindElements(By.Name("password"))[0].FindElement(By.TagName("input"));
					element13.Click();
					Thread.Sleep(500);
					string valueElement8 = element13.GetAttribute("value");
					for (int j = 0; j <= valueElement8.Length; j++)
					{
						element13.SendKeys(Keys.Backspace);
					}
					element13.SendKeys(AccountSettings.Password);
				}
				else if (AccountSettings.NameBCAccount == "LigaStavokRU")
				{
					try
					{
						IWebElement element14 = WebDriver.FindElement(By.Id("login"));
						if (string.IsNullOrEmpty(element14.GetAttribute("value")))
						{
							element14.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							element14 = WebDriver.FindElement(By.Id("password"));
							element14.GetAttribute("value");
							element14.SendKeys(AccountSettings.Password);
						}
						break;
					}
					catch (Exception ex)
					{
						Console.WriteLine("LigaStavokRU  LoopAutoPaste" + ex.Message);
					}
				}
				else if (AccountSettings.NameBCAccount == "FonbetRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element15 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[1]/div[1]/div/input"));
					if (string.IsNullOrEmpty(element15.GetAttribute("value")))
					{
						element15.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						element15 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[2]/div[2]/div[1]/div/input"));
						element15.GetAttribute("value");
						element15.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "ParibetRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					try
					{
						WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div/div/span")).Click();
					}
					catch
					{
					}
					IWebElement element16 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[1]/div[1]/div/input"));
					if (string.IsNullOrEmpty(element16.GetAttribute("value")))
					{
						element16.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						element16 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[2]/div[2]/div[1]/div/input"));
						element16.GetAttribute("value");
						element16.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "BetteryRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element17 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[1]/div[1]/div/input"));
					if (string.IsNullOrEmpty(element17.GetAttribute("value")))
					{
						element17.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						element17 = WebDriver.FindElement(By.XPath("/html/body/application/div[3]/div[2]/div/div/div/div/form/div/div[2]/div[2]/div[2]/div[1]/div/input"));
						element17.GetAttribute("value");
						element17.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "MelbetRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element18 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/form/div[1]/div[1]/div/div[1]/input"));
					if (string.IsNullOrEmpty(element18.GetAttribute("value")))
					{
						element18.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						element18 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div/form/div[1]/div[2]/div/div/input"));
						element18.GetAttribute("value");
						element18.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "Marathon" && urlWebDriver.Contains(AccountSettings.Host))
				{
					if (AccountSettings.Login.Contains("+7"))
					{
						IWebElement element19 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/button[1]"));
						element19.Click();
						Thread.Sleep(500);
						element19 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[1]/div/div/div[2]/div/div/div[1]/input"));
						element19.Click();
						if (string.IsNullOrEmpty(element19.GetAttribute("value")))
						{
							element19.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							element19 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div/div/div[1]/div[1]/input"));
							element19.GetAttribute("value");
							element19.SendKeys(AccountSettings.Password);
						}
					}
					else
					{
						IWebElement element19 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[1]/button[2]"));
						element19.Click();
						Thread.Sleep(500);
						element19 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[2]/div/div[1]/div[1]/div/div/div/div/input"));
						element19.Click();
						if (string.IsNullOrEmpty(element19.GetAttribute("value")))
						{
							element19.SendKeys(AccountSettings.Login.Replace("+7", ""));
							Thread.Sleep(1500);
							element19 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[2]/div/div[1]/div[2]/div/div/div[1]/div[1]/input"));
							element19.GetAttribute("value");
							element19.SendKeys(AccountSettings.Password);
						}
					}
				}
				else if (AccountSettings.NameBCAccount == "MarathonRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element20 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[1]/div/div/div[1]/div[2]/input"));
					element20.Click();
					if (string.IsNullOrEmpty(element20.GetAttribute("value")))
					{
						element20.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						element20 = WebDriver.FindElement(By.XPath("/html/body/div[7]/div/div[4]/span/div[2]/div[3]/div/div/div/div/div[2]/div/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div/div/div[1]/div[1]/input"));
						element20.GetAttribute("value");
						element20.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "BetBoom" && urlWebDriver == "https://betboom.ru/")
				{
					try
					{
						IWebElement balance = null;
						try
						{
							balance = WebDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/header/div/div[2]/div[1]/div/div"));
						}
						catch
						{
						}
						if (balance == null)
						{
							ReadOnlyCollection<IWebElement> fields = WebDriver.FindElements(By.TagName("input"));
							IWebElement element21 = fields[0];
							string valueElement9 = element21.GetAttribute("value");
							if (string.IsNullOrEmpty(valueElement9) || valueElement9 == "+7 ___-___-__-__")
							{
								element21.Click();
								element21.SendKeys(AccountSettings.Login.Replace("+7", ""));
								Thread.Sleep(1500);
								element21 = fields[1];
								valueElement9 = element21.GetAttribute("value");
								element21.SendKeys(AccountSettings.Password);
							}
						}
					}
					catch
					{
					}
				}
				else if (AccountSettings.NameBCAccount == "Leon" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element22 = WebDriver.FindElement(By.XPath("/html/body/div[3]/div/div/div[2]/div/div[1]/div/div[2]/form/div/div[1]/div/div/div/div[1]/div[2]/div/div[2]/div/input"));
					if (string.IsNullOrEmpty(element22.GetAttribute("value")))
					{
						element22.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						element22 = WebDriver.FindElement(By.XPath("/html/body/div[3]/div/div/div[2]/div/div[1]/div/div[2]/form/div/div[2]/div/div[1]/div[2]/div/div/input"));
						element22.GetAttribute("value");
						element22.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "LeonRU" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement form__control = WebDriver.FindElement(By.XPath("/html/body/div[3]/div/div/div/div[2]/div/div[1]/div/div[2]/form/div[1]"));
					IWebElement element23 = form__control.FindElements(By.TagName("input"))[0];
					element23.Click();
					string valueElement10 = element23.GetAttribute("value");
					if (string.IsNullOrEmpty(valueElement10) || valueElement10 == "on")
					{
						for (int k = 0; k <= valueElement10.Length; k++)
						{
							element23.SendKeys(Keys.Backspace);
						}
						element23.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						element23 = form__control.FindElements(By.TagName("input"))[1];
						element23.Click();
						valueElement10 = element23.GetAttribute("value");
						for (int l = 0; l <= valueElement10.Length; l++)
						{
							element23.SendKeys(Keys.Backspace);
						}
						element23.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "Winline" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element24 = WebDriver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/cdk-dialog-container/ww-feature-login/div/div/form/div/div/ww-shared-phone-input/div/input[2]"));
					if (string.IsNullOrEmpty(element24.GetAttribute("value")))
					{
						element24.SendKeys(AccountSettings.Login.Replace("+7", ""));
						Thread.Sleep(1500);
						element24 = WebDriver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/cdk-dialog-container/ww-feature-login/div/div/form/div/div/div[1]/ww-shared-password-input/div/input"));
						element24.GetAttribute("value");
						element24.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "OlimpCom" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element25 = WebDriver.FindElement(By.XPath("/html/body/header/div[1]/div[2]/form/div/input[1]"));
					if (string.IsNullOrEmpty(element25.GetAttribute("value")))
					{
						element25.SendKeys(AccountSettings.Login);
						Thread.Sleep(1000);
						element25 = WebDriver.FindElement(By.XPath("/html/body/header/div[1]/div[2]/form/div/input[2]"));
						element25.GetAttribute("value");
						element25.SendKeys(AccountSettings.Password);
					}
				}
				else if (AccountSettings.NameBCAccount == "OlimpBet" && urlWebDriver.Contains(AccountSettings.Host))
				{
					try
					{
						IWebElement element26 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/form/div[1]/div/input"));
						if (string.IsNullOrEmpty(element26.GetAttribute("value")))
						{
							element26.SendKeys(AccountSettings.Login.Replace("+7", ""));
						}
					}
					catch
					{
					}
					try
					{
						IWebElement element27 = WebDriver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/form/div[2]/div/input"));
						if (string.IsNullOrEmpty(element27.GetAttribute("value")))
						{
							element27.SendKeys(AccountSettings.Password);
						}
					}
					catch
					{
					}
				}
				else if (AccountSettings.NameBCAccount == "ZenitWin" && urlWebDriver.Contains(AccountSettings.Host))
				{
					IWebElement element28 = WebDriver.FindElement(By.XPath("/html/body/div[7]/main/div[2]/div[2]/div[9]/div[2]/div[2]/input"));
					if (string.IsNullOrEmpty(element28.GetAttribute("value")))
					{
						element28.SendKeys(AccountSettings.Login);
						Thread.Sleep(1500);
						element28 = WebDriver.FindElement(By.XPath("/html/body/div[7]/main/div[2]/div[2]/div[9]/div[3]/input"));
						element28.GetAttribute("value");
						element28.SendKeys(AccountSettings.Password);
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
			GetResponseBodyCommandSettings cmd = new GetResponseBodyCommandSettings();
			cmd.RequestId = e.RequestId;
			GetResponseBodyCommandResponse data = await Domains.Network.GetResponseBody(cmd);
			if (data == null)
			{
				return;
			}
			if (AccountSettings.NameBCAccount.Contains("Parimatch"))
			{
				if (e.Response.Url.Contains("/RoutingLogin"))
				{
					JProperty jp = (from p in ((JContainer)JsonConvert.DeserializeObject(data.Body)).Descendants().OfType<JProperty>()
						where p.Name == "token"
						select p).FirstOrDefault();
					if (jp != null)
					{
						AccountSettings.SetParameterSession("token", jp.Value.ToString());
					}
				}
				else if (e.Response.Url.Contains("/user/getAccountInfo"))
				{
					string status = (from p in ((JContainer)JsonConvert.DeserializeObject(data.Body)).Descendants().OfType<JProperty>()
						where p.Name == "status"
						select p).First().Value.ToString();
					AccountSettings.SetParameterSession("status", status);
				}
			}
			else if (AccountSettings.NameBCAccount == "LigaStavokRU" && e.Response.Url.Contains("ligastavok"))
			{
				if (e.Response.Url.Contains("/getUserInfo"))
				{
					string vipClient = (from p in ((JContainer)JsonConvert.DeserializeObject(data.Body)).Descendants().OfType<JProperty>()
						where p.Name == "vipClient"
						select p).First().Value.ToString();
					AccountSettings.SetParameter("vipClient", vipClient);
				}
				else if (e.Response.Url.Contains("/getBalance"))
				{
					JProperty jp2 = (from p in ((JContainer)JsonConvert.DeserializeObject(data.Body)).Descendants().OfType<JProperty>()
						where p.Name == "number"
						select p).FirstOrDefault();
					if (jp2 != null)
					{
						AccountSettings.SetParameter("AccountNumber", jp2.Value.ToString());
					}
				}
			}
			else if (AccountSettings.NameBCAccount == "BetBoom")
			{
				if (e.Response.Url.Contains("/api/auth/login"))
				{
					dynamic jsonResponce = JObject.Parse(data.Body);
					string token = (string)jsonResponce.token;
					string refreshToken = (string)jsonResponce.refresh_token;
					if (!string.IsNullOrEmpty(token))
					{
						AccountSettings.SetParameterSession("x_access_token", token);
						AccountSettings.SetParameterSession("RefreshToken", refreshToken);
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
						string strDev = e.Response.Url;
						strDev = strDev.Remove(0, strDev.IndexOf("&dev") + 5);
						strDev = strDev.Remove(strDev.IndexOf("&"));
						AccountSettings.SetParameterSession("dev", strDev);
						string block_status = (from p in ((JContainer)JsonConvert.DeserializeObject(data.Body)).Descendants().OfType<JProperty>()
							where p.Name == "block_status"
							select p).First().Value.ToString();
						AccountSettings.SetParameter("block_status", block_status);
					}
				}
				else if (AccountSettings.NameBCAccount == "FonbetRU" || AccountSettings.NameBCAccount == "BetteryRU")
				{
					if (e.Response.Url.Contains("/session/info"))
					{
						JContainer obj = (JContainer)JsonConvert.DeserializeObject(data.Body);
						int limitGroup = (int)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "limitGroup"
							select p).First().Value;
						int betToBetDelay = (int)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "betToBetDelay"
							select p).First().Value;
						bool betBlocked = (bool)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "betBlocked"
							select p).First().Value;
						string clientId = (string?)(from p in obj.Descendants().OfType<JProperty>()
							where p.Name == "clientId"
							select p).First().Value;
						string comment = "";
						if (betToBetDelay != 0)
						{
							comment = comment + " ! (тайм.=" + betToBetDelay + ")";
						}
						if (betBlocked)
						{
							comment += " ! БЛОК !";
						}
						if (!betBlocked && betToBetDelay == 0 && limitGroup == 1)
						{
							comment += " (проверен!)";
						}
						AccountSettings.SetParameter("commentDisplay", AccountSettings.Login + comment);
						AccountSettings.SetParameter("clientId", clientId);
					}
					else if (e.Response.Url.Contains("/session/login"))
					{
						JContainer obj2 = (JContainer)JsonConvert.DeserializeObject(data.Body);
						string client = obj2["session"]["client"].ToString();
						string fsid = obj2["session"]["fsid"].ToString();
						AccountSettings.SetParameter("clientId", client);
						AccountSettings.SetParameterSession("fsid", fsid);
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
						JContainer obj3 = (JContainer)JsonConvert.DeserializeObject(data.Body);
						int limitGroup2 = (int)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "limitGroup"
							select p).First().Value;
						int betToBetDelay2 = (int)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "betToBetDelay"
							select p).First().Value;
						bool betBlocked2 = (bool)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "betBlocked"
							select p).First().Value;
						string clientId2 = (string?)(from p in obj3.Descendants().OfType<JProperty>()
							where p.Name == "clientId"
							select p).First().Value;
						string comment2 = "";
						if (betToBetDelay2 != 0)
						{
							comment2 = comment2 + " ! (тайм.=" + betToBetDelay2 + ")";
						}
						if (betBlocked2)
						{
							comment2 += " ! БЛОК !";
						}
						if (!betBlocked2 && betToBetDelay2 == 0 && limitGroup2 == 1)
						{
							comment2 += " (проверен!)";
						}
						AccountSettings.SetParameter("commentDisplay", AccountSettings.Login + comment2);
						AccountSettings.SetParameter("clientId", clientId2);
					}
					else if (e.Response.Url.Contains("/session/login"))
					{
						JContainer obj4 = (JContainer)JsonConvert.DeserializeObject(data.Body);
						string clientId3 = obj4["session"]["client"].ToString();
						string fsid2 = obj4["session"]["fsid"].ToString();
						AccountSettings.SetParameter("clientId", clientId3);
						AccountSettings.SetParameterSession("fsid", fsid2);
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
			string ua = (string)ExecuteScript("return navigator.userAgent;");
			CookieCollection cookies = QratorCookies.GetCookies("https://www.ligastavok.ru/", ua, AccountSettings.Proxy.webProxy);
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
		string cookie = CookieContainer.GetCookies(new Uri("https://www.ligastavok.ru/"))[0].Value;
		string nonce = cookie.Split('-')[0];
		string qsessid = cookie.Split('-')[1];
		int pow = GetPow();
		string uri = "https://ligastavok.ru/__qrator/validate?pow=" + pow + "&nonce=" + nonce + "&qsessid=" + qsessid;
		UpdateJsid(uri);
		CookieCollection collection = CookieContainer.GetCookies(new Uri("https://www.ligastavok.ru/"));
		if (collection == null)
		{
			return;
		}
		foreach (System.Net.Cookie item in collection)
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
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "GET";
		request.Headers.Add("Accept-Encoding: gzip, deflate, br");
		request.Headers.Add("Accept-Language: ru,en;q=0.9");
		request.Headers.Add("Origin: https://www.ligastavok.ru");
		request.Accept = "*/*";
		request.UserAgent = AccountSettings.UserAgent;
		request.Referer = "https://www.ligastavok.ru/";
		request.CookieContainer = CookieContainer;
		request.AutomaticDecompression = DecompressionMethods.GZip;
		request.KeepAlive = true;
		if (!AccountSettings.Proxy.IsEmpty)
		{
			request.Proxy = AccountSettings.Proxy.webProxy;
		}
		request.ContinueTimeout = 10000;
		ServicePoint sp = request.ServicePoint;
		sp.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sp, (byte)0, null);
		string result = "";
		try
		{
			WebResponse response = request.GetResponse();
			result = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	private string POSTResponse(string url)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "POST";
		request.Headers.Add("Accept-Encoding: gzip, deflate, br");
		request.Headers.Add("Accept-Language: ru,en;q=0.9");
		request.Headers.Add("Origin: https://ligastavok.ru");
		request.Accept = "*/*";
		string userAgent = (request.UserAgent = AccountSettings.UserAgent);
		request.UserAgent = userAgent;
		request.CookieContainer = CookieContainer;
		request.ContentType = "application/json";
		request.AutomaticDecompression = DecompressionMethods.GZip;
		request.KeepAlive = true;
		if (!AccountSettings.Proxy.IsEmpty)
		{
			request.Proxy = AccountSettings.Proxy.webProxy;
		}
		request.ServicePoint.ConnectionLimit = 5;
		request.ContinueTimeout = 10000;
		request.ServicePoint.Expect100Continue = false;
		ServicePoint sp = request.ServicePoint;
		sp.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sp, (byte)0, null);
		string result = "";
		try
		{
			StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
			streamWriter.Write(0);
			streamWriter.Flush();
			WebResponse response = request.GetResponse();
			result = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	private int GetPow()
	{
		int next = 2;
		int a = 0;
		string u = "";
		string jsr = CookieContainer.GetCookies(new Uri("https://ligastavok.ru"))[0].Value.Split('-')[0];
		for (int i = 0; i < 10000; i++)
		{
			switch (next)
			{
			case 2:
			{
				a++;
				string d = jsr + a;
				u = CreateMD5(d);
				if (a % 1000 != 0)
				{
					next = 8;
				}
				break;
			}
			case 8:
				if (u.Substring(0, 2) != "00")
				{
					next = 2;
					break;
				}
				return a;
			}
		}
		return -1;
	}

	public string CreateMD5(string input)
	{
		using MD5 md5 = MD5.Create();
		byte[] inputBytes = Encoding.ASCII.GetBytes(input);
		byte[] hashBytes = md5.ComputeHash(inputBytes);
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < hashBytes.Length; i++)
		{
			sb.Append(hashBytes[i].ToString("X2"));
		}
		return sb.ToString();
	}

	private string GetActiveWindowTitle()
	{
		StringBuilder Buff = new StringBuilder(256);
		if (GetWindowText(Process.MainWindowHandle, Buff, 256) > 0)
		{
			return Buff.ToString();
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
				string httpOnly = array2[4];
				string secure = array2[5];
				_ = array2[6];
				try
				{
					System.Net.Cookie newCookie = new System.Net.Cookie();
					newCookie.Name = name;
					newCookie.Value = value;
					newCookie.Domain = domain;
					newCookie.Path = path;
					newCookie.HttpOnly = Convert.ToBoolean(httpOnly);
					newCookie.Secure = Convert.ToBoolean(secure);
					cookieCollection.Add(newCookie);
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
