using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace AccountsLibrary;

public class QratorCookies
{
	private CookieContainer cookieContainer;

	private AccountSettings Account { get; set; }

	public QratorCookies(AccountSettings account)
	{
		Account = account;
	}

	public CookieCollection GetCookies(string url)
	{
		try
		{
			UpdateCookies(url);
			return cookieContainer.GetCookies(new Uri(url));
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void UpdateCookies(string url)
	{
		cookieContainer = new CookieContainer();
		GETResponse(url);
		GETResponse(url + "__qrator/qauth_utm_v2.js");
		GETResponse(url + "favicon.ico");
		string value = cookieContainer.GetCookies(new Uri(url))[0].Value;
		string text = value.Split('-')[0];
		string text2 = value.Split('-')[1];
		int pow = GetPow(url);
		string url2 = url + "__qrator/validate?pow=" + pow + "&nonce=" + text + "&qsessid=" + text2;
		UpdateJsid(url2);
		CookieCollection cookies = cookieContainer.GetCookies(new Uri(url));
		if (cookies.Count < 3)
		{
			Console.WriteLine("Something went wrong :(");
		}
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
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36 OPR/92.0.0.0";
		httpWebRequest.Referer = url;
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		httpWebRequest.KeepAlive = true;
		if (!Account.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = Account.Proxy.webProxy;
		}
		httpWebRequest.ContinueTimeout = 10000;
		string text = "";
		try
		{
			WebResponse response = httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			text = streamReader.ReadToEnd();
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
		httpWebRequest.Headers.Add("Origin: " + url);
		httpWebRequest.Accept = "*/*";
		httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 YaBrowser/21.5.3.740 Yowser/2.5 Safari/537.36";
		httpWebRequest.CookieContainer = cookieContainer;
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
		httpWebRequest.KeepAlive = true;
		if (!Account.Proxy.IsEmpty)
		{
			httpWebRequest.Proxy = Account.Proxy.webProxy;
		}
		httpWebRequest.ServicePoint.ConnectionLimit = 5;
		httpWebRequest.ContinueTimeout = 10000;
		httpWebRequest.ServicePoint.Expect100Continue = false;
		string text = "";
		try
		{
			StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
			streamWriter.Write(0);
			streamWriter.Flush();
			WebResponse response = httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(response.GetResponseStream());
			text = streamReader.ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return text;
	}

	private int GetPow(string url)
	{
		int num = 2;
		int num2 = 0;
		string text = "";
		string text2 = cookieContainer.GetCookies(new Uri(url))[0].Value.Split('-')[0];
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

	private string CreateMD5(string input)
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
}
