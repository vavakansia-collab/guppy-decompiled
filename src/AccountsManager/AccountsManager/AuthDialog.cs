using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace AccountsManager;

public class AuthDialog : Window, IComponentConnector
{
	internal TextBox textBox_Login;

	internal PasswordBox PasswordBox;

	internal Button button_Auth;

	private bool _contentLoaded;

	public AuthDialog()
	{
		InitializeComponent();
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AccountsManager;component/Resources/am_logo.ico"));
		base.Topmost = true;
		PasswordBox.Password = ViewModelAuth.AMSettings.LastPassword;
		Settings.AuthDialogWindow = this;
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		Close();
		Process.GetCurrentProcess().Kill();
		Application.Current.Shutdown();
	}

	private void btnHide_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
	}

	private void Login_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			ViewModelAuth.instance.AuthCommand.Execute(null);
		}
	}

	private void Password_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			ViewModelAuth.instance.AuthCommand.Execute(null);
		}
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		textBox_Login.Focus();
	}

	private void PasswordChanged_Click(object sender, RoutedEventArgs e)
	{
		ViewModelAuth.AMSettings.LastPassword = PasswordBox.Password;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/AccountsManager;component/authdialog.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((AuthDialog)target).Closing += Window_Closing;
			((AuthDialog)target).Loaded += Window_Loaded;
			break;
		case 2:
			((Grid)target).MouseDown += gridTitle_MouseDown;
			break;
		case 3:
			((Button)target).Click += btnHide_Click;
			break;
		case 4:
			((Button)target).Click += btnClose_Click;
			break;
		case 5:
			textBox_Login = (TextBox)target;
			textBox_Login.KeyDown += Login_KeyDown;
			break;
		case 6:
			PasswordBox = (PasswordBox)target;
			PasswordBox.KeyDown += Password_KeyDown;
			PasswordBox.PasswordChanged += PasswordChanged_Click;
			break;
		case 7:
			button_Auth = (Button)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
