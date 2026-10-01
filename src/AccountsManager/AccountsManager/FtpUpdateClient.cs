using System;
using System.IO;
using System.Net;
using FluentFTP;

namespace AccountsManager;

public class FtpUpdateClient
{
	private const string ADDRESS = "31.31.196.215";

	private const int PORT = 21;

	private const string USERNAME = "u0377646_guppy";

	private const string PASSWORD = "Sto9cm19by50!";

	private static readonly string TIMESTAMPPATH = Environment.CurrentDirectory + "/timestamp";

	public static (bool, string) GetVesrion()
	{
		if (!File.Exists(TIMESTAMPPATH))
		{
			File.WriteAllText(TIMESTAMPPATH, "");
		}
		FtpClient ftpClient = new FtpClient("31.31.196.215", 21);
		ftpClient.Credentials = new NetworkCredential("u0377646_guppy", "Sto9cm19by50!");
		try
		{
			ftpClient.Connect();
		}
		catch
		{
			return (false, "");
		}
		long ticks = ftpClient.GetModifiedTime("update.zip").Ticks;
		return (File.ReadAllText(TIMESTAMPPATH) != ticks.ToString(), ticks.ToString().Substring(0, 11));
	}
}
