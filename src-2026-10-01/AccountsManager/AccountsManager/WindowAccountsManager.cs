using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using AccountsManager.DropCoef;
using AccountsManager.Properties;

namespace AccountsManager;

public class WindowAccountsManager : Window, INotifyPropertyChanged, IComponentConnector
{
	public static WindowAccountsManager instance;

	private DoubleAnimation animation;

	private double pxStep = 20.0;

	private double duration = 0.2;

	private bool showLeftMenu;

	internal Grid grid;

	internal RowDefinition rowBoard;

	internal Grid gridTitle;

	internal Grid gridMain;

	internal Border borderMenu;

	internal Grid gridLogoNews;

	internal Canvas canvasRSS;

	internal TextBlock rssTextBlock;

	internal ListBox listBoxBets;

	internal Ellipse BlinkingIndicator;

	private bool _contentLoaded;

	private DelRunRSS delRunRSS { get; set; }

	public bool ShowLeftMenu
	{
		get
		{
			return showLeftMenu;
		}
		set
		{
			showLeftMenu = value;
			RaisePropertyChanged("ShowLeftMenu");
		}
	}

	private DispatcherTimer timer { get; set; } = new DispatcherTimer();

	private double valueOpacity { get; set; } = 0.007;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public WindowAccountsManager()
	{
		InitializeComponent();
		instance = this;
		base.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/AccountsManager;component/Resources/am_logo.ico"));
		Hide();
		delRunRSS = RunRSSMessage;
	}

	private void OnWindowClosing(object sender, CancelEventArgs e)
	{
		Process.GetCurrentProcess().Kill();
		Application.Current.Shutdown();
	}

	private void timerOpacity_Tick(object sender, EventArgs e)
	{
		if (base.Opacity > 1.0)
		{
			base.Opacity = 1.0;
		}
		else if (base.Opacity < 0.0)
		{
			base.Opacity = 0.0;
		}
		base.Opacity += valueOpacity;
		if (valueOpacity < 0.0)
		{
			if (base.Opacity <= 0.0)
			{
				timer.Stop();
				Hide();
				return;
			}
		}
		else if (base.Opacity >= 1.0)
		{
			timer.Stop();
			return;
		}
		if (base.Opacity < 0.0 || base.Opacity > 1.0)
		{
			timer.Stop();
		}
	}

	public static bool ShowWindowDialog(string text)
	{
		return new WindowDialog
		{
			textBlockMessage = 
			{
				Text = text
			},
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
			Topmost = true
		}.ShowDialog().Value;
	}

	private void btnHide_Click(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void btnWindow_Click(object sender, RoutedEventArgs e)
	{
		if (base.WindowState != WindowState.Maximized)
		{
			base.WindowState = WindowState.Maximized;
		}
		else
		{
			base.WindowState = WindowState.Normal;
		}
	}

	private void btnClose_Click(object sender, RoutedEventArgs e)
	{
		if (!ShowWindowDialog("Вы действительно хотите завершить работу?"))
		{
			return;
		}
		ViewModelMainWindow.IsAppBlock = true;
		Settings.SaveApp();
		if (VMDropCoef.instance != null)
		{
			VMDropCoef.instance.Save();
		}
		Task.Run(delegate
		{
			AccountsCenterClient.Disconnect();
			BettingCenterClient.Disconnect();
		});
		RSS.ThreadRSS.Abort();
		if (Settings.AMSettings.SoundPlay)
		{
			SoundPlayer soundPlayer = new SoundPlayer(AccountsManager.Properties.Resources.exit);
			base.Opacity = 1.0;
			valueOpacity = -0.03;
			timer = new DispatcherTimer();
			timer.Tick += timerOpacity_Tick;
			timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
			timer.Start();
			soundPlayer.Play();
			soundPlayer.Dispose();
		}
		Task.Run(delegate
		{
			Thread.Sleep(4000);
			base.Dispatcher.Invoke(delegate
			{
				Close();
				Environment.Exit(1);
			});
		});
	}

	private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	public void RunRSSMessage(string rssMessage)
	{
		if (rssTextBlock.Dispatcher.Thread != Thread.CurrentThread)
		{
			base.Dispatcher.BeginInvoke(delRunRSS, rssMessage);
			return;
		}
		rssTextBlock.Text = rssMessage;
		Canvas.SetLeft(rssTextBlock, canvasRSS.ActualWidth);
		animation = new DoubleAnimation();
		animation.Duration = TimeSpan.FromSeconds(duration);
		animation.Completed += RSSAnimation_Completed;
		RSSAnimation(Canvas.GetLeft(rssTextBlock), Canvas.GetLeft(rssTextBlock) - pxStep);
	}

	private void RSSAnimation(double from, double to)
	{
		if (Canvas.GetLeft(rssTextBlock) + rssTextBlock.ActualWidth > 0.0)
		{
			animation.From = from;
			animation.To = to;
			rssTextBlock.BeginAnimation(Canvas.LeftProperty, animation);
		}
		else if (RSS.rssMessages.Count > 0)
		{
			rssTextBlock.Text = RSS.rssMessages.Dequeue();
			animation.From = canvasRSS.ActualWidth;
			animation.To = canvasRSS.ActualWidth - pxStep;
			rssTextBlock.BeginAnimation(Canvas.LeftProperty, animation);
		}
		else if (RSS.rssMessages.Count <= 0)
		{
			RSS.FillUrlRSS();
			RSS.FillRSSMessages();
		}
	}

	private void RSSAnimation_Completed(object sender, EventArgs e)
	{
		RSSAnimation(Canvas.GetLeft(rssTextBlock), Canvas.GetLeft(rssTextBlock) - pxStep);
	}

	private void borderMenu_MouseLeave(object sender, MouseEventArgs e)
	{
		if (ShowLeftMenu)
		{
			AnumateLeftMenu();
		}
	}

	private void AnumateLeftMenu()
	{
		ShowLeftMenu = !ShowLeftMenu;
		if (ShowLeftMenu)
		{
			ThicknessAnimation animation = new ThicknessAnimation();
			animation.From = new Thickness(-90.0, 0.0, 0.0, 0.0);
			animation.To = new Thickness(0.0, 0.0, 0.0, 0.0);
			animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
			borderMenu.BeginAnimation(FrameworkElement.MarginProperty, animation);
			animation = null;
		}
		else if (!ShowLeftMenu)
		{
			ThicknessAnimation animation2 = new ThicknessAnimation();
			animation2.From = new Thickness(0.0, 0.0, 0.0, 0.0);
			animation2.To = new Thickness(-90.0, 0.0, 0.0, 0.0);
			animation2.Duration = new Duration(TimeSpan.FromSeconds(0.5));
			borderMenu.BeginAnimation(FrameworkElement.MarginProperty, animation2);
			animation2 = null;
		}
	}

	private void btnMenu_Click(object sender, MouseButtonEventArgs e)
	{
		AnumateLeftMenu();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		base.Opacity = 0.0;
		Show();
		valueOpacity = 0.03;
		timer = new DispatcherTimer();
		timer.Tick += timerOpacity_Tick;
		timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
		timer.Start();
	}

	private void SearchAccountsByText_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			ViewModelMainWindow.instance.OnSearchTextChanged();
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocater = new Uri("/AccountsManager;component/_accountsmanager/windowaccountsmanager.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((WindowAccountsManager)target).Loaded += Window_Loaded;
			((WindowAccountsManager)target).Closing += OnWindowClosing;
			break;
		case 2:
			grid = (Grid)target;
			break;
		case 3:
			rowBoard = (RowDefinition)target;
			break;
		case 4:
			gridTitle = (Grid)target;
			gridTitle.MouseDown += gridTitle_MouseDown;
			break;
		case 5:
			((Button)target).Click += btnHide_Click;
			break;
		case 6:
			((Button)target).Click += btnWindow_Click;
			break;
		case 7:
			((Button)target).Click += btnClose_Click;
			break;
		case 8:
			gridMain = (Grid)target;
			break;
		case 9:
			borderMenu = (Border)target;
			borderMenu.MouseLeave += borderMenu_MouseLeave;
			break;
		case 10:
			((StackPanel)target).MouseLeftButtonDown += btnMenu_Click;
			break;
		case 11:
			gridLogoNews = (Grid)target;
			break;
		case 12:
			canvasRSS = (Canvas)target;
			break;
		case 13:
			rssTextBlock = (TextBlock)target;
			break;
		case 14:
			listBoxBets = (ListBox)target;
			break;
		case 15:
			BlinkingIndicator = (Ellipse)target;
			break;
		case 16:
			((TextBox)target).KeyDown += SearchAccountsByText_KeyDown;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
