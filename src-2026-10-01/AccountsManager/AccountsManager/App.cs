using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using CommonLibrary;

namespace AccountsManager;

public class App : Application
{
	private static Mutex _syncObject;

	private const string _syncObjectName = "{E063F-AE0D-480e-9FCA-4BE4B8CDB4E2}";

	private bool _contentLoaded;

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		if (CommonStaticFunction.IsAfalika())
		{
			_syncObject = new Mutex(initiallyOwned: true, "{E063F-AE0D-480e-9FCA-4BE4B8CDB4E2}", out var createdNew);
			if (!createdNew)
			{
				MessageBox.Show("AccountsManager уже запущен!");
				Application.Current.Shutdown();
			}
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
			Uri resourceLocater = new Uri("/AccountsManager;component/app.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}
	}

	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public static void Main()
	{
		App app = new App();
		app.InitializeComponent();
		app.Run();
	}
}
