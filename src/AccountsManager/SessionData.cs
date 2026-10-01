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
		string contents = JsonConvert.SerializeObject(data);
		File.WriteAllText(DATA_DIRECTORY + name + ".json", contents);
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
		string path = DATA_DIRECTORY + name + ".json";
		if (!File.Exists(path))
		{
			return null;
		}
		return JsonConvert.DeserializeObject(File.ReadAllText(path));
	}

	public static T Load<T>(string name) where T : class
	{
		string path = DATA_DIRECTORY + name + ".json";
		if (!File.Exists(path))
		{
			return null;
		}
		if (typeof(T).Name == "CookieContainer")
		{
			Dictionary<string, string[]>? dictionary = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(path));
			CookieContainer cookieContainer = new CookieContainer
			{
				PerDomainCapacity = 100
			};
			foreach (KeyValuePair<string, string[]> item in dictionary)
			{
				Uri uri = new Uri(item.Key);
				string[] value = item.Value;
				for (int i = 0; i < value.Length; i++)
				{
					string[] array = value[i].Split('•');
					Cookie cookie = new Cookie(array[0], array[1]);
					cookieContainer.Add(uri, cookie);
				}
			}
			return cookieContainer as T;
		}
		return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
	}

	public static void Delete(string name)
	{
		string path = DATA_DIRECTORY + name + ".json";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public static string GetKey(AccountSettings accountSettings)
	{
		return $"{accountSettings.NameProvider.ToLower()}_{accountSettings.GUID}";
	}

	private static Dictionary<string, string[]> ExtractCookies(CookieContainer cookieContainer)
	{
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		Hashtable hashtable = (Hashtable)cookieContainer.GetType().InvokeMember("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, cookieContainer, new object[0]);
		foreach (object key in hashtable.Keys)
		{
			string text = (string)key;
			if (text[0] == '.')
			{
				text = text.Substring(1);
			}
			foreach (object key2 in ((SortedList)hashtable[key].GetType().InvokeMember("m_list", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, hashtable[key], new object[0])).Keys)
			{
				string text2 = "https://" + text + (string)key2;
				string[] value = (from Cookie x in cookieContainer.GetCookies(new Uri(text2))
					select x.Name + "•" + x.Value).ToArray();
				dictionary.Add(text2, value);
			}
		}
		return dictionary;
	}
}
