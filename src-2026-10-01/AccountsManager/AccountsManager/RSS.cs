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
				string title = nodeItem["title"].InnerText;
				title = title.Replace("&quot;", "\"");
				title = title.Replace("&ndash;", "\"");
				string description = nodeItem["description"].InnerText;
				description = description.Replace("&quot;", "");
				description = description.Replace("&ndash;", "");
				rssMessages.Enqueue(title);
				rssMessages.Enqueue(description);
			}
		}
		if (rssMessages.Count > 0)
		{
			WindowAccountsManager.instance.RunRSSMessage(rssMessages.Dequeue());
		}
	}
}
