using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary;
using FluentFTP;

namespace AccountsManagerUpdater;

public class ViewModelUpdater : INotifyPropertyChanged
{
	private static readonly string UPDATEDIRECTORY = Environment.CurrentDirectory + "/Update";

	private static readonly string UPDATEFILEPATH = UPDATEDIRECTORY + "/update.zip";

	private static readonly string TIMESTAMPPATH = Environment.CurrentDirectory + "/timestamp";

	private const string ADDRESS = "31.31.196.215";

	private const int PORT = 21;

	private const string USERNAME = "u0377646_guppy";

	private const string PASSWORD = "Anubis1234";

	private static readonly string ExeNameApp = AppDomain.CurrentDomain.FriendlyName;

	private static readonly string ExePathApp = Assembly.GetEntryAssembly().Location;

	public static ViewModelUpdater instance = null;

	private static FtpClient FtpClient;

	private string statusUI;

	private double progressBar = 0.0;

	private RelayCommand closeCommand;

	public string StatusUI
	{
		get
		{
			return statusUI;
		}
		set
		{
			statusUI = value;
			RaisePropertyChanged("StatusUI");
		}
	}

	public double ProgressBar
	{
		get
		{
			return progressBar;
		}
		set
		{
			progressBar = value;
			RaisePropertyChanged("ProgressBar");
		}
	}

	public RelayCommand CloseCommand => closeCommand ?? (closeCommand = new RelayCommand(delegate
	{
		CloseApp();
	}));

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public ViewModelUpdater()
	{
		if (!Directory.Exists(UPDATEDIRECTORY))
		{
			Directory.CreateDirectory(UPDATEDIRECTORY);
		}
		if (!File.Exists(TIMESTAMPPATH))
		{
			File.WriteAllText(TIMESTAMPPATH, "");
		}
		FtpClient = new FtpClient("31.31.196.215", 21);
		FtpClient.Credentials = new NetworkCredential("u0377646_guppy", "Anubis1234");
		Task.Run(delegate
		{
			Update();
		});
	}

	private void Update()
	{
		try
		{
			StatusUI = "Подключение...";
			while (!FtpClient.IsConnected)
			{
				try
				{
					FtpClient.Connect();
				}
				catch
				{
				}
				Thread.Sleep(1000);
			}
			long ticks = FtpClient.GetModifiedTime("update.zip").Ticks;
			string text = File.ReadAllText(TIMESTAMPPATH);
			if (text != ticks.ToString())
			{
				StatusUI = "Загрузка обновлений...";
				FtpStatus ftpStatus = FtpClient.DownloadFile(UPDATEFILEPATH, "update.zip", FtpLocalExists.Overwrite, FtpVerify.None, OnProgress);
				if (ftpStatus == FtpStatus.Success)
				{
					StatusUI = "Установка...";
					string fullPath = Path.GetFullPath(UPDATEDIRECTORY);
					string text2 = fullPath + "\\tmp";
					if (Directory.Exists(text2))
					{
						Directory.Delete(text2, recursive: true);
					}
					Directory.CreateDirectory(text2);
					ZipFile.ExtractToDirectory(UPDATEFILEPATH, text2);
					string path = Environment.CurrentDirectory + "/AccountsManager.exe";
					if (File.Exists(path))
					{
						try
						{
							Process[] processesByName = Process.GetProcessesByName("AccountsManager");
							Process[] array = processesByName;
							foreach (Process process in array)
							{
								if (process.MainModule.FileName == Path.GetFullPath(path))
								{
									process.Kill();
								}
							}
						}
						catch
						{
						}
					}
					Thread.Sleep(1000);
					List<string> list = new List<string>();
					CopyDirectory(text2, Environment.CurrentDirectory, list);
					if (list.Count > 3)
					{
						throw new Exception("Files copy error");
					}
					Directory.Delete(UPDATEDIRECTORY, recursive: true);
					File.WriteAllText(TIMESTAMPPATH, ticks.ToString());
					StatusUI = "Готово";
					StartAM();
				}
				else
				{
					StatusUI = "Ошибка соединения...";
					Task.Run(delegate
					{
						Thread.Sleep(2000);
						Update();
					});
				}
			}
			else
			{
				StatusUI = "Обновление не требуется";
				try
				{
					Thread.Sleep(500);
					Directory.Delete(UPDATEDIRECTORY, recursive: true);
				}
				catch
				{
				}
				StartAM();
			}
		}
		catch (Exception)
		{
			StatusUI = "Произошла ошибка во время выполнения...";
			Task.Run(delegate
			{
				Thread.Sleep(5000);
				CloseApp();
			});
		}
	}

	private void StartAM()
	{
		Process.Start("AccountsManager.exe");
		Thread.Sleep(1000);
		CloseApp();
	}

	private void CloseApp()
	{
		MainWindow._dispatcher.Invoke(delegate
		{
			MainWindow.instance.Close();
		});
		Process.GetCurrentProcess().Kill();
		Application.Current.Shutdown();
	}

	private void OnProgress(FtpProgress progress)
	{
		ProgressBar = progress.Progress;
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
