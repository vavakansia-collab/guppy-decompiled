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
		string cookie = cookieContainer.GetCookies(new Uri(url))[0].Value;
		string nonce = cookie.Split('-')[0];
		string qsessid = cookie.Split('-')[1];
		int pow = GetPow(url);
		string _url = url + "__qrator/validate?pow=" + pow + "&nonce=" + nonce + "&qsessid=" + qsessid;
		UpdateJsid(_url);
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
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "GET";
		request.Headers.Add("Accept-Encoding: gzip, deflate, br");
		request.Headers.Add("Accept-Language: ru,en;q=0.9");
		request.Accept = "*/*";
		request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36 OPR/92.0.0.0";
		request.Referer = url;
		request.CookieContainer = cookieContainer;
		request.AutomaticDecompression = DecompressionMethods.GZip;
		request.KeepAlive = true;
		if (!Account.Proxy.IsEmpty)
		{
			request.Proxy = Account.Proxy.webProxy;
		}
		request.ContinueTimeout = 10000;
		string result = "";
		try
		{
			WebResponse response = request.GetResponse();
			StreamReader streamreader = new StreamReader(response.GetResponseStream());
			result = streamreader.ReadToEnd();
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
		request.Headers.Add("Origin: " + url);
		request.Accept = "*/*";
		request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 YaBrowser/21.5.3.740 Yowser/2.5 Safari/537.36";
		request.CookieContainer = cookieContainer;
		request.ContentType = "application/json";
		request.AutomaticDecompression = DecompressionMethods.GZip;
		request.KeepAlive = true;
		if (!Account.Proxy.IsEmpty)
		{
			request.Proxy = Account.Proxy.webProxy;
		}
		request.ServicePoint.ConnectionLimit = 5;
		request.ContinueTimeout = 10000;
		request.ServicePoint.Expect100Continue = false;
		string result = "";
		try
		{
			StreamWriter streamwriter = new StreamWriter(request.GetRequestStream());
			streamwriter.Write(0);
			streamwriter.Flush();
			WebResponse response = request.GetResponse();
			StreamReader streamreader = new StreamReader(response.GetResponseStream());
			result = streamreader.ReadToEnd();
			response.Close();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	private int GetPow(string url)
	{
		int next = 2;
		int a = 0;
		string u = "";
		string jsr = cookieContainer.GetCookies(new Uri(url))[0].Value.Split('-')[0];
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

	private string CreateMD5(string input)
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
}
