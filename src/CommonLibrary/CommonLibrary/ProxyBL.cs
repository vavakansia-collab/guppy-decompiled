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
		Proxies = "81.177.23.64:23712:IDih5a:V3kFPY\r\n193.0.202.68:18156:IAwkWR:IcxQ6w\r\n146.185.223.247:13496:98jkbv:qlwbin\r\n193.233.60.111:23228:CkjcIq:7tsjJY\r\n147.45.207.240:16451:dHwv4I:GjK8Ch\r\n91.243.60.35:21042:IhG3Bw:1U6FnM";
		Regex regex = new Regex("(?<ip>\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(?<port>\\d{1,5}):(?<username>\\w*):(?<password>\\w*)");
		if (!regex.IsMatch(Proxies))
		{
			return;
		}
		foreach (Match item2 in regex.Matches(Proxies))
		{
			string value = item2.Groups["ip"].Value;
			int initPort = Convert.ToInt32(item2.Groups["port"].Value);
			string value2 = item2.Groups["username"].Value;
			string value3 = item2.Groups["password"].Value;
			Proxy item = new Proxy(value, initPort, value2, value3);
			ListProxy.Add(item);
		}
	}

	public static Proxy GetRandomProxy()
	{
		return ListProxy[Random.Next(0, ListProxy.Count)];
	}
}
