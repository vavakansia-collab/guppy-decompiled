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
			foreach (string item in array2)
			{
				string name = item.Split('=')[0];
				string value = item.Split('=')[1];
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
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "GET";
		request.Accept = "*/*";
		request.CookieContainer = cookieContainer;
		request.KeepAlive = true;
		request.UserAgent = UserAgent;
		request.Timeout = 5000;
		request.AutomaticDecompression = DecompressionMethods.GZip;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			request.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string item in headers)
			{
				string[] header = item.ToLower().Split(':');
				switch (header[0].ToLower())
				{
				case "accept":
					request.Accept = header[1];
					break;
				case "useragent":
					request.UserAgent = header[1];
					break;
				case "user-agent":
					request.UserAgent = header[1];
					break;
				case "referer":
					request.Referer = header[1] + ":" + header[2];
					break;
				case "host":
					request.Host = header[1];
					break;
				case "content-type":
					request.ContentType = header[1];
					break;
				default:
					request.Headers.Add(item);
					break;
				}
			}
		}
		string result = "";
		try
		{
			WebResponse response = request.GetResponse();
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
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "GET";
		request.Accept = "*/*";
		request.CookieContainer = cookieContainer;
		request.KeepAlive = true;
		request.UserAgent = UserAgent;
		request.Timeout = 5000;
		request.AutomaticDecompression = DecompressionMethods.GZip;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			request.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string item in headers)
			{
				string[] header = item.ToLower().Split(':');
				switch (header[0].ToLower())
				{
				case "accept":
					request.Accept = header[1];
					break;
				case "useragent":
					request.UserAgent = header[1];
					break;
				case "user-agent":
					request.UserAgent = header[1];
					break;
				case "referer":
					request.Referer = header[1] + ":" + header[2];
					break;
				case "host":
					request.Host = header[1];
					break;
				case "content-type":
					request.ContentType = header[1];
					break;
				default:
					request.Headers.Add(item);
					break;
				}
			}
		}
		HttpWebResponse response;
		try
		{
			response = (HttpWebResponse)(await request.GetResponseAsync());
		}
		catch
		{
			return null;
		}
		StreamReader sr = null;
		string result;
		try
		{
			Stream responseStream = response.GetResponseStream();
			if (response.ContentEncoding.ToLower().Contains("gzip"))
			{
				responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
			}
			sr = new StreamReader(responseStream, Encoding.UTF8);
			result = sr.ReadToEnd();
		}
		catch
		{
			response.Close();
			sr.Close();
			return null;
		}
		response.Close();
		sr.Close();
		return result;
	}

	public string POSTResponse(string url, string data, List<string> headers = null)
	{
		return POSTResponse(url, Encoding.UTF8.GetBytes(data), headers);
	}

	public string POSTResponse(string url, byte[] data, List<string> headers = null)
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
		if (Proxy != null && !Proxy.IsEmpty)
		{
			request.Proxy = Proxy.webProxy;
		}
		if (headers != null)
		{
			foreach (string item in headers)
			{
				string[] header = item.ToLower().Split(':');
				switch (header[0].ToLower())
				{
				case "accept":
					request.Accept = header[1];
					break;
				case "useragent":
					request.UserAgent = header[1];
					break;
				case "user-agent":
					request.UserAgent = header[1];
					break;
				case "referer":
					request.Referer = header[1] + ":" + header[2];
					break;
				case "host":
					request.Host = header[1];
					break;
				case "content-type":
					request.ContentType = header[1];
					break;
				default:
					request.Headers.Add(item);
					break;
				}
			}
		}
		string result = "";
		try
		{
			request.GetRequestStream().Write(data, 0, data.Length);
			WebResponse response = request.GetResponse();
			result = new StreamReader(response.GetResponseStream()).ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			result = "@ERROR:" + ex.Message;
		}
		return result;
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
			foreach (string item in headers)
			{
				string[] header = item.ToLower().Split(':');
				switch (header[0].ToLower())
				{
				case "accept":
					request.Accept = header[1];
					break;
				case "useragent":
					request.UserAgent = header[1];
					break;
				case "user-agent":
					request.UserAgent = header[1];
					break;
				case "referer":
					request.Referer = header[1] + ":" + header[2];
					break;
				case "host":
					request.Host = header[1];
					break;
				case "content-type":
					request.ContentType = header[1];
					break;
				default:
					request.Headers.Add(item);
					break;
				}
			}
		}
		HttpWebResponse response;
		try
		{
			Stream obj = await request.GetRequestStreamAsync();
			obj.Write(data, 0, data.Length);
			obj.Close();
			response = (HttpWebResponse)(await request.GetResponseAsync());
		}
		catch
		{
			return null;
		}
		Stream st = response.GetResponseStream();
		if (response.ContentEncoding.ToLower().Contains("gzip"))
		{
			st = new GZipStream(st, CompressionMode.Decompress);
		}
		StreamReader streamReader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
		string result = streamReader.ReadToEnd();
		response.Close();
		streamReader.Close();
		return result;
	}

	public HttpWebResponse GetPOSTResponse(string url, string data, List<string> headers = null)
	{
		return GetPOSTResponse(url, Encoding.UTF8.GetBytes(data), headers);
	}

	public HttpWebResponse GetPOSTResponse(string url, byte[] data, List<string> headers = null)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "POST";
		request.Accept = "*/*";
		request.CookieContainer = cookieContainer;
		request.ContentType = "application/json";
		request.KeepAlive = true;
		request.Timeout = 5000;
		request.UserAgent = UserAgent;
		if (Proxy != null && !Proxy.IsEmpty)
		{
			request.Proxy = Proxy.webProxy;
		}
		request.AutomaticDecompression = DecompressionMethods.GZip;
		if (headers != null)
		{
			foreach (string item in headers)
			{
				string[] header = item.ToLower().Split(':');
				string value = header[1];
				for (int i = 2; i < header.Length; i++)
				{
					value = value + ":" + header[i];
				}
				switch (header[0])
				{
				case "accept":
					request.Accept = value;
					break;
				case "useragent":
					request.UserAgent = value;
					break;
				case "user-agent":
					request.UserAgent = value;
					break;
				case "referer":
					request.Referer = value;
					break;
				case "content-type":
					request.ContentType = value;
					break;
				default:
					request.Headers.Add(item);
					break;
				}
			}
		}
		try
		{
			request.GetRequestStream().Write(data, 0, data.Length);
			return request.GetResponse() as HttpWebResponse;
		}
		catch (Exception ex)
		{
			_ = "@ERROR:" + ex.Message;
		}
		return null;
	}

	public async Task<string> UploadFile(string url, byte[] data, HttpContent[] additions = null)
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
		ParseDetails result = ParseDetailed(source, start, end, searchStartIndex);
		if (result != null)
		{
			return result.result;
		}
		return "";
	}

	public static ParseDetails ParseDetailed(string source, string start, string end, int searchStartIndex = 0)
	{
		if (start.Length >= source.Length)
		{
			return null;
		}
		int entryIndex = source.IndexOf(start, searchStartIndex);
		if (entryIndex == -1)
		{
			return null;
		}
		int startIndex = entryIndex + start.Length;
		int endIndex = source.IndexOf(end, startIndex);
		int length = endIndex - startIndex;
		if (length < 0)
		{
			return null;
		}
		return new ParseDetails
		{
			entryIndex = entryIndex,
			startIndex = startIndex,
			endIndex = endIndex,
			result = source.Substring(startIndex, length)
		};
	}
}
