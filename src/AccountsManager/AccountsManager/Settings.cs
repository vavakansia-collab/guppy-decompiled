using System;
using System.IO;
using CommonLibrary;
using Newtonsoft.Json;

namespace AccountsManager;

public static class Settings
{
	public static readonly string APPSETTINGSPATH = Environment.CurrentDirectory + "\\AMSettings.gb";

	public static AMSettings AMSettings = null;

	public static AuthDialog AuthDialogWindow = null;

	public static CupisDatabaseWindow CupisDatabaseWindow = null;

	public static ViewModelAuth ViewModelAuth { get; set; } = null;

	public static ViewModelMainWindow ViewModelMainWindow { get; set; } = null;

	public static void LoadApp()
	{
		if (File.Exists(APPSETTINGSPATH))
		{
			try
			{
				AMSettings = JsonConvert.DeserializeObject<AMSettings>(CommonStaticFunction.DecompressString(File.ReadAllText(APPSETTINGSPATH)));
			}
			catch
			{
			}
		}
		if (AMSettings == null)
		{
			AMSettings = new AMSettings();
		}
		AMSettings.Apply();
	}

	public static void SaveApp()
	{
		AMSettings.Update();
		try
		{
			string text = JsonConvert.SerializeObject(AMSettings);
			File.WriteAllText(APPSETTINGSPATH, CommonStaticFunction.CompressString(text));
		}
		catch
		{
		}
	}
}
