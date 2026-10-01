using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary;

public class Net
{
	public class ParseDetails
	{
		public string result;

		public int entryIndex;

		public int startIndex;

		public int endIndex;
	}

	private string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36";

	private Proxy Proxy;

	private CookieContainer cookieContainer = new CookieContainer();

	private Dictionary<string, string> headers = new Dictionary<string, string>();

	public Net(Proxy initProxy = null, string initUserAgent = "")
	{
		Proxy = initProxy;
		if (!string.IsNullOrEmpty(initUserAgent))
		{
			UserAgent = initUserAgent;
		}
	}

	public CookieContainer GetCookieContainer()
	{
		return cookieContainer;
	}

	public void SetCookieContainer(CookieContainer container)
	{
		cookieContainer = container;
	}

	public void SetMaxConnections(int value)
	{
		ServicePointManager.DefaultConnectionLimit = value;
	}

	public void SetProtocol(SecurityProtocolType protocol)
	{
		ServicePointManager.SecurityProtocol = protocol;
	}

	public void SetCookiesLimit(int value)
	{
		cookieContainer.PerDomainCapacity = value;
	}

	public void AddCookiesFromFile(string filepath, string domain)
	{
		if (File.Exists(filepath))
		{
			cookieContainer.PerDomainCapacity = 100;
			string[] array = File.ReadAllText(filepath).Replace("Cookie: ", "").Replace(" ", "")
				.Split(';');
			Uri uri = new Uri(domain);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string name = text.Split('=')[0];
				string value = text.Split('=')[1];
				cookieContainer.Add(uri, new Cookie(name, value));
			}
		}
	}

	public void AddHeader(string key, string value)
	{
		if (!headers.ContainsKey(key))
		{
			headers.Add(key, value);
		}
		else
		{
			headers[key] = value;
		}
	}

	public void AddCookie(string domain, string value)
	{
		cookieContainer.Add(new Uri(domain), new Cookie(value.Split('=')[0], value.Split('=')[1]));
	}

	public void AddCookie(string domain, string cookieName, string cookieValue)
	{
		try
		{
			cookieContainer.Add(new Uri(domain), new Cookie(cookieName, cookieValue));
		}
		catch
		{
		}
	}

	public void AddCookies(CookieCollection collection)
	{
		try
		{
			cookieContainer.Add(collection);
		}
		catch
		{
		}
	}

	public string GETResponse(string url, List<string> headers = null)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "GET";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.KeepAlive = true;
		httpWebRequest.UserAgent = UserAgent;
		httpWebRequest.Timeout = 5000;
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string header in headers)
			{
				string[] array = header.ToLower().Split(':');
				switch (array[0].ToLower())
				{
				case "accept":
					httpWebRequest.Accept = array[1];
					break;
				case "useragent":
					httpWebRequest.UserAgent = array[1];
					break;
				case "user-agent":
					httpWebRequest.UserAgent = array[1];
					break;
				case "referer":
					httpWebRequest.Referer = array[1] + ":" + array[2];
					break;
				case "host":
					httpWebRequest.Host = array[1];
					break;
				case "content-type":
					httpWebRequest.ContentType = array[1];
					break;
				default:
					httpWebRequest.Headers.Add(header);
					break;
				}
			}
		}
		string result = "";
		try
		{
			WebResponse response = httpWebRequest.GetResponse();
			result = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch
		{
		}
		return result;
	}

	public async Task<string> GETResponseAsync(string url, List<string> headers = null)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "GET";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.KeepAlive = true;
		httpWebRequest.UserAgent = UserAgent;
		httpWebRequest.Timeout = 5000;
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string header in headers)
			{
				string[] array = header.ToLower().Split(':');
				switch (array[0].ToLower())
				{
				case "accept":
					httpWebRequest.Accept = array[1];
					break;
				case "useragent":
					httpWebRequest.UserAgent = array[1];
					break;
				case "user-agent":
					httpWebRequest.UserAgent = array[1];
					break;
				case "referer":
					httpWebRequest.Referer = array[1] + ":" + array[2];
					break;
				case "host":
					httpWebRequest.Host = array[1];
					break;
				case "content-type":
					httpWebRequest.ContentType = array[1];
					break;
				default:
					httpWebRequest.Headers.Add(header);
					break;
				}
			}
		}
		HttpWebResponse httpWebResponse;
		try
		{
			httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
		}
		catch
		{
			return null;
		}
		StreamReader streamReader = null;
		string result;
		try
		{
			Stream stream = httpWebResponse.GetResponseStream();
			if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
			{
				stream = new GZipStream(stream, CompressionMode.Decompress);
			}
			streamReader = new StreamReader(stream, Encoding.UTF8);
			result = streamReader.ReadToEnd();
		}
		catch
		{
			httpWebResponse.Close();
			streamReader.Close();
			return null;
		}
		httpWebResponse.Close();
		streamReader.Close();
		return result;
	}

	public string POSTResponse(string url, string data, List<string> headers = null)
	{
		return POSTResponse(url, Encoding.UTF8.GetBytes(data), headers);
	}

	public string POSTResponse(string url, byte[] data, List<string> headers = null)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "POST";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.KeepAlive = true;
		httpWebRequest.Timeout = 5000;
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		httpWebRequest.UserAgent = UserAgent;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string header in headers)
			{
				string[] array = header.ToLower().Split(':');
				switch (array[0].ToLower())
				{
				case "accept":
					httpWebRequest.Accept = array[1];
					break;
				case "useragent":
					httpWebRequest.UserAgent = array[1];
					break;
				case "user-agent":
					httpWebRequest.UserAgent = array[1];
					break;
				case "referer":
					httpWebRequest.Referer = array[1] + ":" + array[2];
					break;
				case "host":
					httpWebRequest.Host = array[1];
					break;
				case "content-type":
					httpWebRequest.ContentType = array[1];
					break;
				default:
					httpWebRequest.Headers.Add(header);
					break;
				}
			}
		}
		string text = "";
		try
		{
			httpWebRequest.GetRequestStream().Write(data, 0, data.Length);
			WebResponse response = httpWebRequest.GetResponse();
			text = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			text = "@ERROR:" + ex.Message;
		}
		return text;
	}

	public async Task<string> POSTResponseAsync(string url, string data, List<string> headers = null)
	{
		return await POSTResponseAsync(url, Encoding.UTF8.GetBytes(data), headers);
	}

	public async Task<string> POSTResponseAsync(string url, byte[] data, List<string> headers = null)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "POST";
		request.Accept = "*/*";
		request.CookieContainer = cookieContainer;
		request.ContentType = "application/json";
		request.KeepAlive = true;
		request.Timeout = 5000;
		request.AutomaticDecompression = DecompressionMethods.GZip;
		request.UserAgent = UserAgent;
		request.ContentLength = data.Length;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			request.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string header in headers)
			{
				string[] array = header.ToLower().Split(':');
				switch (array[0].ToLower())
				{
				case "accept":
					request.Accept = array[1];
					break;
				case "useragent":
					request.UserAgent = array[1];
					break;
				case "user-agent":
					request.UserAgent = array[1];
					break;
				case "referer":
					request.Referer = array[1] + ":" + array[2];
					break;
				case "host":
					request.Host = array[1];
					break;
				case "content-type":
					request.ContentType = array[1];
					break;
				default:
					request.Headers.Add(header);
					break;
				}
			}
		}
		HttpWebResponse httpWebResponse;
		try
		{
			Stream obj = await request.GetRequestStreamAsync();
			obj.Write(data, 0, data.Length);
			obj.Close();
			httpWebResponse = (HttpWebResponse)(await request.GetResponseAsync());
		}
		catch
		{
			return null;
		}
		Stream stream = httpWebResponse.GetResponseStream();
		if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
		{
			stream = new GZipStream(stream, CompressionMode.Decompress);
		}
		StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
		string result = streamReader.ReadToEnd();
		httpWebResponse.Close();
		streamReader.Close();
		return result;
	}

	public HttpWebResponse GetPOSTResponse(string url, string data, List<string> headers = null)
	{
		return GetPOSTResponse(url, Encoding.UTF8.GetBytes(data), headers);
	}

	public HttpWebResponse GetPOSTResponse(string url, byte[] data, List<string> headers = null)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "POST";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.KeepAlive = true;
		httpWebRequest.Timeout = 5000;
		httpWebRequest.UserAgent = UserAgent;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = Proxy.webProxy;
		}
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		if (headers != null)
		{
			foreach (string header in headers)
			{
				string[] array = header.ToLower().Split(':');
				string text = array[1];
				for (int i = 2; i < array.Length; i++)
				{
					text = text + ":" + array[i];
				}
				switch (array[0])
				{
				case "accept":
					httpWebRequest.Accept = text;
					break;
				case "useragent":
					httpWebRequest.UserAgent = text;
					break;
				case "user-agent":
					httpWebRequest.UserAgent = text;
					break;
				case "referer":
					httpWebRequest.Referer = text;
					break;
				case "content-type":
					httpWebRequest.ContentType = text;
					break;
				default:
					httpWebRequest.Headers.Add(header);
					break;
				}
			}
		}
		try
		{
			httpWebRequest.GetRequestStream().Write(data, 0, data.Length);
			return httpWebRequest.GetResponse() as HttpWebResponse;
		}
		catch (Exception ex)
		{
			_ = "@ERROR:" + ex.Message;
		}
		return null;
	}

	public async Task<string> UploadFile(string url, byte[] data, HttpContent[] additions = null)
	{
		HttpClient val = new HttpClient();
		MultipartFormDataContent val2 = new MultipartFormDataContent();
		if (additions != null)
		{
			foreach (HttpContent val3 in additions)
			{
				((MultipartContent)val2).Add(val3);
			}
		}
		val2.Add((HttpContent)new ByteArrayContent(data), "photo");
		return await (await val.PostAsync(url, (HttpContent)(object)val2)).Content.ReadAsStringAsync();
	}

	public void DowloadFile(string url, string filepath)
	{
		using WebClient webClient = new WebClient();
		webClient.DownloadFile(url, filepath);
	}

	public long GetTimestamp()
	{
		return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000;
	}

	public static string Parse(string source, string start, string end, int searchStartIndex = 0)
	{
		ParseDetails parseDetails = ParseDetailed(source, start, end, searchStartIndex);
		if (parseDetails != null)
		{
			return parseDetails.result;
		}
		return "";
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
		return new ParseDetails
		{
			entryIndex = num,
			startIndex = num2,
			endIndex = num3,
			result = source.Substring(num2, num4)
		};
	}
}
