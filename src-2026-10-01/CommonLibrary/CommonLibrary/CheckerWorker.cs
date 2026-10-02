using System;
using System.Threading;

namespace CommonLibrary;

public class CheckerWorker
{
	public void Start(WatsonAppCheckerClient transport)
	{
		while (true)
		{
			Thread.Sleep(TimeSpan.FromSeconds(10.0));
			transport.SendMessage("Ping1");
		}
	}
}
