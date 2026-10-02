using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace UpdateUploader;

public class Program
{
	private const string ADDRESS = "31.31.196.215";

	private const int PORT = 21;

	private const string USERNAME = "u0377646";

	private const string PASSWORD = "Sto9cm19by50";

	private static readonly string UPDATEDIRECTORY = Environment.CurrentDirectory + "/PreUpdate";

	private static readonly string UPDATEFILEPATH = UPDATEDIRECTORY + "/update.zip";

	public static void Main(string[] args)
	{
		if (!Environment.CurrentDirectory.EndsWith("\\AccountsManager\\bin\\x64\\Release"))
		{
			Console.WriteLine("Родительская папка не \"/AccountsManager/bin/x64/Release/\". Приложение закроется через 5 секунд");
			Thread.Sleep(5000);
			return;
		}
		FtpClient client = new FtpClient("31.31.196.215", 21);
		client.Credentials = new NetworkCredential("u0377646", "Sto9cm19by50");
		while (!client.IsConnected)
		{
			try
			{
				client.Connect();
			}
			catch
			{
				Console.WriteLine(string.Format("Ошибка подключения: {0}:{1}", "31.31.196.215", 21));
			}
			Thread.Sleep(1000);
		}
		string filesDirectory = UPDATEDIRECTORY + "/tmp";
		if (Directory.Exists(UPDATEDIRECTORY))
		{
			try
			{
				Directory.Delete(UPDATEDIRECTORY, recursive: true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.ReadLine();
			}
		}
		if (!Directory.Exists(UPDATEDIRECTORY))
		{
			Directory.CreateDirectory(UPDATEDIRECTORY);
		}
		if (!Directory.Exists(filesDirectory))
		{
			Directory.CreateDirectory(filesDirectory);
		}
		if (File.Exists(UPDATEFILEPATH))
		{
			try
			{
				File.Delete(UPDATEFILEPATH);
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.Message);
				Console.ReadLine();
			}
		}
		Console.WriteLine("Создание обновления...");
		List<string> exclude = new List<string> { "ChromeDriver", "PreUpdate", "Update", "Updater.exe", "timestamp", "Update.zip", "UpdateUploader.exe" };
		foreach (string item in Directory.EnumerateFiles(Environment.CurrentDirectory))
		{
			if (!exclude.Contains(Path.GetFileName(item)))
			{
				try
				{
					File.Copy(item, filesDirectory + "/" + Path.GetFileName(item), overwrite: true);
				}
				catch
				{
					Console.WriteLine("Ошибка копирования: " + Path.GetFileName(item));
				}
			}
		}
		ZipFile.CreateFromDirectory(filesDirectory, UPDATEFILEPATH);
		Thread.Sleep(1000);
		FtpStatus status = FtpStatus.Failed;
		double progress = 0.0;
		bool isReady = false;
		Task.Run(delegate
		{
			while (!isReady)
			{
				Console.Clear();
				Console.WriteLine("Загрузка обновления на сервер: " + progress.ToString("0.0") + "%");
				Thread.Sleep(500);
			}
		});
		try
		{
			status = client.UploadFile(UPDATEFILEPATH, "update/update.zip", FtpRemoteExists.Overwrite, createRemoteDir: false, FtpVerify.None, delegate(FtpProgress x)
			{
				progress = x.Progress;
			});
		}
		catch (Exception ex3)
		{
			isReady = true;
			Thread.Sleep(1000);
			Console.WriteLine(ex3.Message + " " + ex3.InnerException?.Message);
			Console.ReadLine();
		}
		isReady = true;
		Thread.Sleep(1000);
		if (status == FtpStatus.Success)
		{
			Console.Clear();
			Console.WriteLine("Готово!");
			try
			{
				Directory.Delete(UPDATEDIRECTORY, recursive: true);
			}
			catch (Exception ex4)
			{
				Console.WriteLine(ex4.Message);
				Console.ReadLine();
			}
			Thread.Sleep(3000);
		}
		else
		{
			Console.Clear();
			Console.WriteLine("Ошибка отправки на сервер");
			Console.ReadLine();
		}
	}

	private static void CopyDirectory(string from, string to, List<string> errors = null)
	{
		if (!Directory.Exists(to))
		{
			Directory.CreateDirectory(to);
		}
		foreach (string item in Directory.EnumerateFiles(from))
		{
			try
			{
				File.Copy(item, to + "/" + Path.GetFileName(item), overwrite: true);
			}
			catch
			{
				errors?.Add(item);
			}
		}
		foreach (string item2 in Directory.EnumerateDirectories(from))
		{
			CopyDirectory(item2, to + "/" + Path.GetFileName(item2), errors);
		}
	}
}
