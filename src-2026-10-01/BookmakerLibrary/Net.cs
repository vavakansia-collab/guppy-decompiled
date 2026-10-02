using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class Net
{
	public class ExceptionResponse : WebResponse
	{
		public override string ContentType { get; set; }

		public override WebHeaderCollection Headers { get; } = new WebHeaderCollection();

		public ExceptionResponse(string message)
		{
			ContentType = "%ERR%: " + message;
		}
	}

	public class ParseDetails
	{
		public string result;

		public int entryIndex;

		public int startIndex;

		public int endIndex;
	}

	public class ProxyData
	{
		public string host;

		public int port;

		public string username;

		public string password;

		public string raw;

		private WebProxy webProxy;

		public ProxyData(string rawProxy)
		{
			Init(rawProxy);
		}

		public ProxyData(WebProxy proxy)
		{
			string rawProxy = ExtractRawFromWebProxy(proxy);
			Init(rawProxy);
		}

		private void Init(string rawProxy)
		{
			if (!string.IsNullOrEmpty(rawProxy))
			{
				string[] array = rawProxy.Split(':');
				host = array[0];
				port = int.Parse(array[1]);
				if (array.Length > 2)
				{
					username = array[2];
					password = array[3];
				}
				raw = rawProxy;
				webProxy = CreateWebProxy(rawProxy);
			}
		}

		public WebProxy GetWebProxy()
		{
			return webProxy;
		}
	}

	public const string ERROR_SIGN = "%ERR%";

	private static CookieContainer defaultCookieContainer = new CookieContainer();

	private static string defaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36";

	public static string GETResponse(string url, List<string> headers = null, WebProxy proxy = null, CookieContainer cookieContainer = null, int timeout = 20000)
	{
		WebResponse gETResponse = GetGETResponse(url, headers, proxy, cookieContainer, timeout);
		if (IsError(gETResponse))
		{
			return gETResponse.ContentType;
		}
		string result;
		try
		{
			StreamReader streamReader = new StreamReader(gETResponse.GetResponseStream());
			result = streamReader.ReadToEnd();
			gETResponse.Close();
		}
		catch (Exception ex)
		{
			result = "%ERR%: " + ex.Message;
		}
		return result;
	}

	public static WebResponse GetGETResponse(string url, List<string> headers = null, WebProxy proxy = null, CookieContainer cookieContainer = null, int timeout = 20000, bool allowAutoRedirect = true)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "GET";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = defaultUserAgent;
		httpWebRequest.KeepAlive = true;
		httpWebRequest.AllowAutoRedirect = allowAutoRedirect;
		if (proxy != null)
		{
			httpWebRequest.Proxy = proxy;
		}
		httpWebRequest.Timeout = timeout;
		httpWebRequest.ReadWriteTimeout = timeout;
		if (cookieContainer != null)
		{
			httpWebRequest.CookieContainer = cookieContainer;
		}
		else
		{
			httpWebRequest.CookieContainer = defaultCookieContainer;
		}
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		FillHeaders(headers, httpWebRequest);
		try
		{
			WebResponse response = httpWebRequest.GetResponse();
			return response as HttpWebResponse;
		}
		catch (WebException ex)
		{
			return new ExceptionResponse(ex.Message);
		}
	}

	public static string POSTResponse(string url, string data, List<string> headers = null, WebProxy proxy = null, CookieContainer cookieContainer = null, int timeout = 20000)
	{
		return POSTResponse(url, Encoding.UTF8.GetBytes(data), headers, proxy, cookieContainer, timeout);
	}

	public static string POSTResponse(string url, byte[] data, List<string> headers = null, WebProxy proxy = null, CookieContainer cookieContainer = null, int timeout = 20000)
	{
		WebResponse pOSTResponse = GetPOSTResponse(url, data, headers, proxy, cookieContainer, timeout);
		if (IsError(pOSTResponse))
		{
			return pOSTResponse.ContentType;
		}
		string result;
		try
		{
			StreamReader streamReader = new StreamReader(pOSTResponse.GetResponseStream());
			result = streamReader.ReadToEnd();
			pOSTResponse.Close();
		}
		catch (Exception ex)
		{
			result = "%ERR%: " + ex.Message;
		}
		return result;
	}

	public static WebResponse GetPOSTResponse(string url, string data, List<string> headers = null, WebProxy proxy = null, CookieContainer cookieContainer = null, int timeout = 20000)
	{
		return GetPOSTResponse(url, Encoding.UTF8.GetBytes(data), headers, proxy, cookieContainer, timeout);
	}

	public static WebResponse GetPOSTResponse(string url, byte[] data, List<string> headers = null, WebProxy proxy = null, CookieContainer cookieContainer = null, int timeout = 20000)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "POST";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = defaultUserAgent;
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.KeepAlive = true;
		httpWebRequest.AllowAutoRedirect = false;
		if (proxy != null)
		{
			httpWebRequest.Proxy = proxy;
		}
		httpWebRequest.Timeout = timeout;
		httpWebRequest.ReadWriteTimeout = timeout;
		if (cookieContainer != null)
		{
			httpWebRequest.CookieContainer = cookieContainer;
		}
		else
		{
			httpWebRequest.CookieContainer = defaultCookieContainer;
		}
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		FillHeaders(headers, httpWebRequest);
		try
		{
			httpWebRequest.GetRequestStream().Write(data, 0, data.Length);
			WebResponse response = httpWebRequest.GetResponse();
			return response as HttpWebResponse;
		}
		catch (WebException ex)
		{
			return new ExceptionResponse(ex.Message);
		}
	}

	public static string ReadResponseWithDiagnostics(WebResponse response)
	{
		if (response == null)
		{
			return "Response is null";
		}
		HttpWebResponse httpWebResponse = response as HttpWebResponse;
		StringBuilder stringBuilder = new StringBuilder();
		if (httpWebResponse != null)
		{
			stringBuilder.AppendLine("Status: " + (int)httpWebResponse.StatusCode + " " + httpWebResponse.StatusDescription);
			stringBuilder.AppendLine("ResponseUri: " + httpWebResponse.ResponseUri);
		}
		stringBuilder.AppendLine("ContentType: " + response.ContentType);
		stringBuilder.AppendLine("Headers:");
		string[] allKeys = response.Headers.AllKeys;
		foreach (string text in allKeys)
		{
			stringBuilder.AppendLine(text + ": " + response.Headers[text]);
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Body:");
		try
		{
			using Stream stream = response.GetResponseStream();
			if (stream == null)
			{
				return stringBuilder.ToString();
			}
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			stringBuilder.AppendLine(streamReader.ReadToEnd());
		}
		catch (Exception ex)
		{
			stringBuilder.AppendLine("Cannot read body: " + ex.Message);
		}
		finally
		{
			response.Close();
		}
		return stringBuilder.ToString();
	}

	public static void FillHeaders(List<string> headers, HttpWebRequest request)
	{
		if (headers == null)
		{
			return;
		}
		foreach (string header in headers)
		{
			string text = header.Split(':')[0].ToLower();
			string text2 = header.Substring(text.Length + 1).Trim();
			switch (text)
			{
			case "accept":
				request.Accept = text2;
				break;
			case "useragent":
				request.UserAgent = text2;
				break;
			case "user-agent":
				request.UserAgent = text2;
				break;
			case "referer":
				request.Referer = text2;
				break;
			case "host":
				request.Host = text2;
				break;
			case "content-type":
				request.ContentType = text2;
				break;
			default:
				request.Headers.Add(header);
				break;
			}
		}
	}

	public static async Task<string> UploadFile(string url, byte[] data, HttpContent[] additions = null)
	{
		HttpClient client = new HttpClient();
		MultipartFormDataContent content = new MultipartFormDataContent();
		if (additions != null)
		{
			foreach (HttpContent item in additions)
			{
				((MultipartContent)content).Add(item);
			}
		}
		content.Add((HttpContent)new ByteArrayContent(data), "photo");
		return await (await client.PostAsync(url, (HttpContent)(object)content)).Content.ReadAsStringAsync();
	}

	public static void DowloadFile(string url, string filepath, Action<DownloadProgressChangedEventArgs> onDownloadProgressChanged = null)
	{
		using WebClient webClient = new WebClient();
		webClient.DownloadProgressChanged += delegate(object a, DownloadProgressChangedEventArgs b)
		{
			onDownloadProgressChanged?.Invoke(b);
		};
		webClient.DownloadFileTaskAsync(url, filepath).Wait();
	}

	public static void SetMaxConnections(int value)
	{
		ServicePointManager.DefaultConnectionLimit = value;
	}

	public static void SetProtocol(SecurityProtocolType protocol)
	{
		ServicePointManager.SecurityProtocol = protocol;
	}

	public static void EnableAllProtocols()
	{
		SetProtocol(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
	}

	public static void SetCookiesLimit(int value)
	{
		defaultCookieContainer.PerDomainCapacity = value;
	}

	public static void AddCookiesFromFile(string filepath, string domain)
	{
		if (!File.Exists(filepath))
		{
			return;
		}
		defaultCookieContainer.PerDomainCapacity = 100;
		string text = File.ReadAllText(filepath);
		text = text.Replace("Cookie: ", "").Replace(" ", "");
		string[] array = text.Split(';');
		Uri uri = new Uri(domain);
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (!(text2 == ""))
			{
				string name = text2.Split('=')[0];
				string value = text2.Split('=')[1];
				defaultCookieContainer.Add(uri, new Cookie(name, value));
			}
		}
	}

	public static void FillCookieContainer(CookieContainer cookieContainer, string filepath, string domain)
	{
		if (File.Exists(filepath))
		{
			cookieContainer.PerDomainCapacity = 100;
			string rawCookies = File.ReadAllText(filepath);
			FillCookieContainerByRawCookies(cookieContainer, rawCookies, domain);
		}
	}

	public static void FillCookieContainer(CookieContainer cookieContainer, Dictionary<string, string[]> data)
	{
		cookieContainer.PerDomainCapacity = 100;
		foreach (string key in data.Keys)
		{
			Uri uri = new Uri(key);
			string[] array = data[key];
			foreach (string text in array)
			{
				string name = text.Split('•')[0];
				string value = text.Split('•')[1];
				cookieContainer.Add(uri, new Cookie(name, value));
			}
		}
	}

	public static void FillCookieContainerByRawCookies(CookieContainer cookieContainer, string rawCookies, string domain)
	{
		cookieContainer.PerDomainCapacity = 100;
		rawCookies = rawCookies.Replace("Cookie: ", "").Replace(" ", "");
		string[] array = rawCookies.Split(';');
		Uri uri = new Uri(domain);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!(text == ""))
			{
				string name = text.Split('=')[0];
				string value = text.Split('=')[1];
				cookieContainer.Add(uri, new Cookie(name, value));
			}
		}
	}

	public static CookieContainer CreateCookieContainer(string filepath, string domain)
	{
		if (!File.Exists(filepath))
		{
			return null;
		}
		CookieContainer cookieContainer = new CookieContainer();
		cookieContainer.PerDomainCapacity = 100;
		string rawCookies = File.ReadAllText(filepath);
		FillCookieContainerByRawCookies(cookieContainer, rawCookies, domain);
		return cookieContainer;
	}

	public static CookieContainer GetCookieContainer()
	{
		return defaultCookieContainer;
	}

	public static void AddCookies(CookieCollection collection, CookieContainer cookieContainer = null)
	{
		if (cookieContainer == null)
		{
			cookieContainer = defaultCookieContainer;
		}
		cookieContainer.Add(collection);
	}

	public static void SetCookies(string url, CookieCollection collection, CookieContainer cookieContainer = null)
	{
		if (cookieContainer == null)
		{
			cookieContainer = defaultCookieContainer;
		}
		Uri uri = new Uri(url);
		foreach (Cookie item in collection)
		{
			cookieContainer.SetCookies(uri, item.Name + "=" + item.Value);
		}
	}

	public static void AddCookie(string domain, string value, CookieContainer cookieContainer = null)
	{
		if (cookieContainer == null)
		{
			cookieContainer = defaultCookieContainer;
		}
		cookieContainer.Add(new Uri(domain), new Cookie(value.Split('=')[0], value.Split('=')[1]));
	}

	public static void AddCookie(string domain, string cookieName, string cookieValue, CookieContainer cookieContainer = null)
	{
		if (cookieContainer == null)
		{
			cookieContainer = defaultCookieContainer;
		}
		cookieContainer.Add(new Uri(domain), new Cookie(cookieName, cookieValue));
	}

	public static void SetDefaultUserAgent(string userAgent)
	{
		defaultUserAgent = userAgent;
	}

	public static bool IsError(string response)
	{
		return response.StartsWith("%ERR%");
	}

	public static bool IsError(WebResponse response)
	{
		return response.ContentType.StartsWith("%ERR%");
	}

	public static string ReadError(string response)
	{
		if (IsError(response))
		{
			return response.Replace("%ERR%: ", "");
		}
		return null;
	}

	public static string ReadResponse(WebResponse response, Encoding encoding = null)
	{
		if (encoding == null)
		{
			encoding = Encoding.UTF8;
		}
		StreamReader streamReader = new StreamReader(response.GetResponseStream(), encoding);
		return streamReader.ReadToEnd();
	}

	public static WebProxy CreateWebProxy(string rawData)
	{
		try
		{
			string[] array = rawData.Split(':');
			string arg = array[0];
			int num = int.Parse(array[1]);
			WebProxy webProxy = new WebProxy($"http://{arg}:{num}");
			if (array.Length > 2)
			{
				string userName = array[2];
				string password = array[3];
				webProxy.Credentials = new NetworkCredential(userName, password);
			}
			return webProxy;
		}
		catch
		{
			return null;
		}
	}

	public static WebProxy[] CreateWebProxyFromRawData(string proxiesData)
	{
		List<WebProxy> list = new List<WebProxy>();
		string[] array = proxiesData.Split(new string[1] { "\r\n" }, StringSplitOptions.None);
		string[] array2 = array;
		foreach (string rawData in array2)
		{
			WebProxy webProxy = CreateWebProxy(rawData);
			if (webProxy != null)
			{
				list.Add(webProxy);
			}
		}
		return list.ToArray();
	}

	public static WebProxy[] CreateWebProxyFromFile(string path)
	{
		if (!File.Exists(path))
		{
			return new WebProxy[0];
		}
		List<WebProxy> list = new List<WebProxy>();
		string[] array = File.ReadAllLines(path);
		string[] array2 = array;
		foreach (string rawData in array2)
		{
			WebProxy webProxy = CreateWebProxy(rawData);
			if (webProxy != null)
			{
				list.Add(webProxy);
			}
		}
		return list.ToArray();
	}

	public static bool CheckProxy(string rawProxy, int timeout = 3000)
	{
		WebProxy proxy = CreateWebProxy(rawProxy);
		string response = GETResponse("https://api.ipify.org/", null, proxy, null, timeout);
		return !IsError(response);
	}

	public static Dictionary<string, string[]> ExtractCookies(CookieContainer cookieContainer)
	{
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		Hashtable hashtable = (Hashtable)cookieContainer.GetType().InvokeMember("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, cookieContainer, new object[0]);
		foreach (object key in hashtable.Keys)
		{
			string text = (string)key;
			if (text[0] == '.')
			{
				text = "www" + text;
			}
			SortedList sortedList = (SortedList)hashtable[key].GetType().InvokeMember("m_list", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, hashtable[key], new object[0]);
			foreach (object key2 in sortedList.Keys)
			{
				string text2 = "https://" + text + (string)key2;
				Uri uri = new Uri(text2);
				string[] value = (from Cookie x in cookieContainer.GetCookies(uri)
					select x.Name + "•" + x.Value).ToArray();
				dictionary.Add(text2, value);
			}
		}
		return dictionary;
	}

	public static string Parse(string source, string start, string end, int searchStartIndex = 0)
	{
		ParseDetails parseDetails = ParseDetailed(source, start, end, searchStartIndex);
		return (parseDetails == null) ? "" : parseDetails.result;
	}

	public static ParseDetails ParseDetailed(string source, string start, string end, int searchStartIndex = 0)
	{
		if (start.Length >= source.Length)
		{
			return null;
		}
		int num = source.IndexOf(start, searchStartIndex);
		if (num == -1)
		{
			return null;
		}
		int num2 = num + start.Length;
		int num3 = source.IndexOf(end, num2);
		int num4 = num3 - num2;
		if (num4 < 0)
		{
			return null;
		}
		ParseDetails parseDetails = new ParseDetails();
		parseDetails.entryIndex = num;
		parseDetails.startIndex = num2;
		parseDetails.endIndex = num3;
		parseDetails.result = source.Substring(num2, num4);
		return parseDetails;
	}

	public static ProxyData CreateProxyData(string rawProxy)
	{
		return new ProxyData(rawProxy);
	}

	public static ProxyData CreateProxyData(WebProxy proxy)
	{
		return new ProxyData(proxy);
	}

	public static string ExtractRawFromWebProxy(WebProxy proxy)
	{
		string text = $"{proxy.Address.Host}:{proxy.Address.Port}";
		if (proxy.Credentials != null)
		{
			NetworkCredential networkCredential = (NetworkCredential)proxy.Credentials;
			text = text + ":" + networkCredential.UserName + ":" + networkCredential.Password;
		}
		return text;
	}

	public static int GetTimestamp()
	{
		return (int)((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000000);
	}

	public static void Log(string msg)
	{
		Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]: " + msg);
	}

	public static string CreateMD5(string input)
	{
		using MD5 mD = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
