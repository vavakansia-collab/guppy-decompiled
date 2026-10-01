using System.Collections.Generic;
using System.Threading;
using System.Xml;

namespace AccountsManager;

public static class RSS
{
	public static Queue<string> urlRSS;

	private static XmlTextReader rssReader;

	private static XmlDocument rssDoc;

	private static XmlNode nodeRss;

	private static XmlNode nodeChannel;

	private static XmlNode nodeItem;

	public static Queue<string> rssMessages;

	public static bool runningAnumationsRSSMessage;

	public static Thread ThreadRSS;

	private static WindowAccountsManager windowMain { get; set; }

	static RSS()
	{
		urlRSS = new Queue<string>();
		FillUrlRSS();
		rssMessages = new Queue<string>();
		runningAnumationsRSSMessage = false;
	}

	public static void RunRSS(WindowAccountsManager initWindow)
	{
		windowMain = initWindow;
		ThreadRSS = new Thread(FillRSSMessages);
		ThreadRSS.Name = "FillRSS";
		ThreadRSS.IsBackground = true;
		ThreadRSS.Start();
	}

	public static void FillUrlRSS()
	{
		urlRSS.Enqueue("https://www.sport-express.ru/services/materials/news/se/");
		urlRSS.Enqueue("http://ss.sport-express.ru/rss/public_news.rss");
	}

	public static void FillRSSMessages()
	{
		if (urlRSS.Count == 0)
		{
			FillUrlRSS();
			return;
		}
		rssReader = new XmlTextReader(urlRSS.Dequeue());
		rssDoc = new XmlDocument();
		try
		{
			rssDoc.Load(rssReader);
		}
		catch
		{
			FillRSSMessages();
		}
		for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
		{
			if (rssDoc.ChildNodes[i].Name == "rss")
			{
				nodeRss = rssDoc.ChildNodes[i];
			}
		}
		if (nodeRss == null)
		{
			FillUrlRSS();
			FillRSSMessages();
		}
		for (int j = 0; j < nodeRss.ChildNodes.Count; j++)
		{
			if (nodeRss.ChildNodes[j].Name == "channel")
			{
				nodeChannel = nodeRss.ChildNodes[j];
			}
		}
		for (int k = 0; k < nodeChannel.ChildNodes.Count; k++)
		{
			if (nodeChannel.ChildNodes[k].Name == "item")
			{
				nodeItem = nodeChannel.ChildNodes[k];
				string innerText = nodeItem["title"].InnerText;
				innerText = innerText.Replace("&quot;", "\"");
				innerText = innerText.Replace("&ndash;", "\"");
				string innerText2 = nodeItem["description"].InnerText;
				innerText2 = innerText2.Replace("&quot;", "");
				innerText2 = innerText2.Replace("&ndash;", "");
				rssMessages.Enqueue(innerText);
				rssMessages.Enqueue(innerText2);
			}
		}
		if (rssMessages.Count > 0)
		{
			WindowAccountsManager.instance.RunRSSMessage(rssMessages.Dequeue());
		}
	}
}
