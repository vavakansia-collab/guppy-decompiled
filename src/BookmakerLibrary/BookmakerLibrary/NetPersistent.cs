using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookmakerLibrary;

public class NetPersistent
{
	public class ParseDetails
	{
		public string result;

		public int entryIndex;

		public int startIndex;

		public int endIndex;
	}

	private BookmakerCompany BookmakerCompany;

	private string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.80 Safari/537.36";

	private CookieContainer cookieContainer = new CookieContainer();

	private Dictionary<string, string> headers = new Dictionary<string, string>();

	public void SetBookmakerCompany(BookmakerCompany bc)
	{
		BookmakerCompany = bc;
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

	public void EnableAllProtocols()
	{
		SetProtocol(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
	}

	public void AddCookiesFromFile(string filepath, string domain)
	{
		if (File.Exists(filepath))
		{
			cookieContainer.PerDomainCapacity = 100;
			string text = File.ReadAllText(filepath);
			text = text.Replace("Cookie: ", "").Replace(" ", "");
			string[] array = text.Split(';');
			Uri uri = new Uri(domain);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string name = text2.Split('=')[0];
				string value = text2.Split('=')[1];
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
		if (BookmakerCompany != null && BookmakerCompany.Proxy != null && !BookmakerCompany.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = BookmakerCompany.Proxy.webProxy;
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
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			result = streamReader.ReadToEnd();
			response.Close();
		}
		catch
		{
		}
		return result;
	}

	public HttpWebResponse GetGETResponse(string url, List<string> headers = null, WebProxy proxy = null)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "GET";
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = UserAgent;
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.KeepAlive = true;
		if (BookmakerCompany != null && BookmakerCompany.Proxy != null && !BookmakerCompany.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = BookmakerCompany.Proxy.webProxy;
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
				default:
					httpWebRequest.Headers.Add(header);
					break;
				case "content-type":
					break;
				}
			}
		}
		try
		{
			WebResponse response = httpWebRequest.GetResponse();
			Console.WriteLine("SUCCESS: " + url);
			return response as HttpWebResponse;
		}
		catch (WebException ex)
		{
			Console.WriteLine("FAIL: " + url);
			return ex.Response as HttpWebResponse;
		}
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
		if (BookmakerCompany != null && BookmakerCompany.Proxy != null && !BookmakerCompany.Proxy.IsEmpty)
		{
			request.Proxy = BookmakerCompany.Proxy.webProxy;
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

	public string ReadResponse(HttpWebResponse response)
	{
		try
		{
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			return streamReader.ReadToEnd();
		}
		catch
		{
			return null;
		}
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
		if (BookmakerCompany != null && BookmakerCompany.Proxy != null && !BookmakerCompany.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = BookmakerCompany.Proxy.webProxy;
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
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			text = streamReader.ReadToEnd();
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
		if (BookmakerCompany != null && BookmakerCompany.Proxy != null && !BookmakerCompany.Proxy.IsEmpty)
		{
			request.Proxy = BookmakerCompany.Proxy.webProxy;
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
			Stream newStream = await request.GetRequestStreamAsync();
			newStream.Write(data, 0, data.Length);
			newStream.Close();
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
		StreamReader sr = new StreamReader(st, Encoding.GetEncoding("utf-8"));
		string result = sr.ReadToEnd();
		response.Close();
		sr.Close();
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
		if (BookmakerCompany != null && BookmakerCompany.Proxy != null && !BookmakerCompany.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = BookmakerCompany.Proxy.webProxy;
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
		string text2 = "";
		try
		{
			httpWebRequest.GetRequestStream().Write(data, 0, data.Length);
			WebResponse response = httpWebRequest.GetResponse();
			return response as HttpWebResponse;
		}
		catch (Exception ex)
		{
			text2 = "@ERROR:" + ex.Message;
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
		return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000.0);
	}

	public string Parse(string source, string start, string end, int searchStartIndex = 0)
	{
		ParseDetails parseDetails = ParseDetailed(source, start, end, searchStartIndex);
		return (parseDetails == null) ? "" : parseDetails.result;
	}

	public ParseDetails ParseDetailed(string source, string start, string end, int searchStartIndex = 0)
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
}
