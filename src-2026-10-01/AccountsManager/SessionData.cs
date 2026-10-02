using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AccountsLibrary;
using Newtonsoft.Json;

public class SessionData
{
	private static string DATA_DIRECTORY;

	static SessionData()
	{
		DATA_DIRECTORY = Environment.CurrentDirectory + "\\SessionData\\";
		if (!Directory.Exists(DATA_DIRECTORY))
		{
			Directory.CreateDirectory(DATA_DIRECTORY);
		}
	}

	public static void Save(AccountSettings accountSettings, CookieContainer cookieContainer)
	{
		Save(GetKey(accountSettings), cookieContainer);
	}

	public static void Save(AccountSettings accountSettings, object data)
	{
		Save(GetKey(accountSettings), data);
	}

	public static void Save(string name, CookieContainer cookieContainer)
	{
		Dictionary<string, string[]> data = ExtractCookies(cookieContainer);
		Save(name, data);
	}

	public static void Save(string name, object data)
	{
		string rawData = JsonConvert.SerializeObject(data);
		File.WriteAllText(DATA_DIRECTORY + name + ".json", rawData);
	}

	public static T Load<T>(AccountSettings accountSettings) where T : class
	{
		return Load<T>(GetKey(accountSettings));
	}

	public static dynamic Load(AccountSettings accountSettings)
	{
		return Load(GetKey(accountSettings));
	}

	public static dynamic Load(string name)
	{
		string filepath = DATA_DIRECTORY + name + ".json";
		if (!File.Exists(filepath))
		{
			return null;
		}
		return JsonConvert.DeserializeObject(File.ReadAllText(filepath));
	}

	public static T Load<T>(string name) where T : class
	{
		string filepath = DATA_DIRECTORY + name + ".json";
		if (!File.Exists(filepath))
		{
			return null;
		}
		if (typeof(T).Name == "CookieContainer")
		{
			Dictionary<string, string[]>? dictionary = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(filepath));
			CookieContainer cookieContainer = new CookieContainer
			{
				PerDomainCapacity = 100
			};
			foreach (KeyValuePair<string, string[]> item in dictionary)
			{
				Uri url = new Uri(item.Key);
				string[] value = item.Value;
				for (int i = 0; i < value.Length; i++)
				{
					string[] cookie_data = value[i].Split('•');
					Cookie cookie = new Cookie(cookie_data[0], cookie_data[1]);
					cookieContainer.Add(url, cookie);
				}
			}
			return cookieContainer as T;
		}
		return JsonConvert.DeserializeObject<T>(File.ReadAllText(filepath));
	}

	public static void Delete(string name)
	{
		string filepath = DATA_DIRECTORY + name + ".json";
		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}
	}

	public static string GetKey(AccountSettings accountSettings)
	{
		return $"{accountSettings.NameProvider.ToLower()}_{accountSettings.GUID}";
	}

	private static Dictionary<string, string[]> ExtractCookies(CookieContainer cookieContainer)
	{
		Dictionary<string, string[]> result = new Dictionary<string, string[]>();
		Hashtable table = (Hashtable)cookieContainer.GetType().InvokeMember("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, cookieContainer, new object[0]);
		foreach (object tableKey in table.Keys)
		{
			string str_tableKey = (string)tableKey;
			if (str_tableKey[0] == '.')
			{
				str_tableKey = str_tableKey.Substring(1);
			}
			foreach (object listKey in ((SortedList)table[tableKey].GetType().InvokeMember("m_list", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, table[tableKey], new object[0])).Keys)
			{
				string url = "https://" + str_tableKey + (string)listKey;
				string[] domain_cookies = (from Cookie x in cookieContainer.GetCookies(new Uri(url))
					select x.Name + "•" + x.Value).ToArray();
				result.Add(url, domain_cookies);
			}
		}
		return result;
	}
}
