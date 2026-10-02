using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommonLibrary;

public static class ProxyBL
{
	public static List<Proxy> ListProxy;

	private static Random Random;

	public static string Proxies;

	static ProxyBL()
	{
		ListProxy = new List<Proxy>();
		Random = new Random();
		Proxies = "45.196.142.92:64272:hAHif1dG:BchQXgjC\r\n170.168.195.76:62530:hAHif1dG:BchQXgjC\r\n135.106.105.42:63290:hAHif1dG:BchQXgjC\r\n201.4.124.220:64156:hAHif1dG:BchQXgjC";
		Regex regex = new Regex("(?<ip>\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(?<port>\\d{1,5}):(?<username>\\w*):(?<password>\\w*)");
		if (!regex.IsMatch(Proxies))
		{
			return;
		}
		foreach (Match item in regex.Matches(Proxies))
		{
			string ip = item.Groups["ip"].Value;
			int port = Convert.ToInt32(item.Groups["port"].Value);
			string userName = item.Groups["username"].Value;
			string password = item.Groups["password"].Value;
			Proxy newProxy = new Proxy(ip, port, userName, password);
			ListProxy.Add(newProxy);
		}
	}

	public static Proxy GetRandomProxy()
	{
		return ListProxy[Random.Next(0, ListProxy.Count)];
	}
}
