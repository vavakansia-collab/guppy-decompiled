using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AccountsLibrary;

namespace AccountsManager;

public class AMSettings : INotifyPropertyChanged
{
	private OwnerAccount ownerAccounts = new OwnerAccount();

	private double windowStartUpLeft = 50.0;

	private double windowStartUpTop = 50.0;

	private double windowStartUpWidth = 900.0;

	private double windowStartUpHeight = 600.0;

	private double windowCupisDatabaseStartUpLeft = 50.0;

	private double windowCupisDatabaseStartUpTop = 50.0;

	private double windowCupisDatabaseStartUpWidth = 600.0;

	private double windowCupisDatabaseStartUpHeight = 700.0;

	private string lastLogin = "";

	private string lastPassword = "";

	private bool isSavePassword = true;

	private bool soundPlay = true;

	private bool imageSportBlack;

	public Dictionary<string, CupisDatabaseWindow.CupisClientAdditionals> CupisClientsAdditionals = new Dictionary<string, CupisDatabaseWindow.CupisClientAdditionals>();

	public Dictionary<string, bool> VisibilityOwners = new Dictionary<string, bool>();

	public OwnerAccount OwnerAccounts
	{
		get
		{
			return ownerAccounts;
		}
		set
		{
			ownerAccounts = value;
			RaisePropertyChanged("OwnerAccounts");
		}
	}

	public double WindowStartUpLeft
	{
		get
		{
			return windowStartUpLeft;
		}
		set
		{
			windowStartUpLeft = value;
			RaisePropertyChanged("WindowStartUpLeft");
		}
	}

	public double WindowStartUpTop
	{
		get
		{
			return windowStartUpTop;
		}
		set
		{
			windowStartUpTop = value;
			RaisePropertyChanged("WindowStartUpTop");
		}
	}

	public double WindowStartUpWidth
	{
		get
		{
			return windowStartUpWidth;
		}
		set
		{
			windowStartUpWidth = value;
			RaisePropertyChanged("WindowStartUpWidth");
		}
	}

	public double WindowStartUpHeight
	{
		get
		{
			return windowStartUpHeight;
		}
		set
		{
			windowStartUpHeight = value;
			RaisePropertyChanged("WindowStartUpHeight");
		}
	}

	public double WindowCupisDatabaseStartUpLeft
	{
		get
		{
			return windowCupisDatabaseStartUpLeft;
		}
		set
		{
			windowCupisDatabaseStartUpLeft = value;
			RaisePropertyChanged("WindowCupisDatabaseStartUpLeft");
		}
	}

	public double WindowCupisDatabaseStartUpTop
	{
		get
		{
			return windowCupisDatabaseStartUpTop;
		}
		set
		{
			windowCupisDatabaseStartUpTop = value;
			RaisePropertyChanged("WindowCupisDatabaseStartUpTop");
		}
	}

	public double WindowCupisDatabaseStartUpWidth
	{
		get
		{
			return windowCupisDatabaseStartUpWidth;
		}
		set
		{
			windowCupisDatabaseStartUpWidth = value;
			RaisePropertyChanged("WindowCupisDatabaseStartUpWidth");
		}
	}

	public double WindowCupisDatabaseStartUpHeight
	{
		get
		{
			return windowCupisDatabaseStartUpHeight;
		}
		set
		{
			windowCupisDatabaseStartUpHeight = value;
			RaisePropertyChanged("WindowCupisDatabaseStartUpHeight");
		}
	}

	public string LastLogin
	{
		get
		{
			return lastLogin;
		}
		set
		{
			lastLogin = value;
			RaisePropertyChanged("LastLogin");
		}
	}

	public string LastPassword
	{
		get
		{
			return lastPassword;
		}
		set
		{
			lastPassword = value;
			RaisePropertyChanged("LastPassword");
		}
	}

	public bool IsSavePassword
	{
		get
		{
			return isSavePassword;
		}
		set
		{
			isSavePassword = value;
			RaisePropertyChanged("IsSavePassword");
		}
	}

	public bool SoundPlay
	{
		get
		{
			return soundPlay;
		}
		set
		{
			soundPlay = value;
			RaisePropertyChanged("SoundPlay");
		}
	}

	public bool ImageSportBlack
	{
		get
		{
			return imageSportBlack;
		}
		set
		{
			imageSportBlack = value;
			RaisePropertyChanged("ImageSportBlack");
		}
	}

	public bool AccountsCreatedChecked { get; set; } = true;

	public bool AccountsReadyChecked { get; set; } = true;

	public bool AccountsDelayChecked { get; set; } = true;

	public bool AccountsoWorkedOutChecked { get; set; } = true;

	public bool AccountsStartedChecked { get; set; } = true;

	public bool AccountsStopedChecked { get; set; } = true;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public void Update()
	{
		WindowStartUpLeft = WindowAccountsManager.instance.Left;
		WindowStartUpTop = WindowAccountsManager.instance.Top;
		WindowStartUpWidth = WindowAccountsManager.instance.Width;
		WindowStartUpHeight = WindowAccountsManager.instance.Height;
		AccountsCreatedChecked = ViewModelMainWindow.instance.AccountsCreatedChecked;
		AccountsReadyChecked = ViewModelMainWindow.instance.AccountsReadyChecked;
		AccountsDelayChecked = ViewModelMainWindow.instance.AccountsDelayChecked;
		AccountsoWorkedOutChecked = ViewModelMainWindow.instance.AccountsoWorkedOutChecked;
		AccountsStartedChecked = ViewModelMainWindow.instance.AccountsStartedChecked;
		AccountsStopedChecked = ViewModelMainWindow.instance.AccountsStopedChecked;
		if (ViewModelMainWindow.CupisDatabaseWindow != null)
		{
			WindowCupisDatabaseStartUpLeft = ViewModelMainWindow.CupisDatabaseWindow.Left;
			WindowCupisDatabaseStartUpTop = ViewModelMainWindow.CupisDatabaseWindow.Top;
			WindowCupisDatabaseStartUpWidth = ViewModelMainWindow.CupisDatabaseWindow.Width;
			WindowCupisDatabaseStartUpHeight = ViewModelMainWindow.CupisDatabaseWindow.Height;
			CupisClientsAdditionals = CupisDatabaseWindow.additionals;
		}
	}

	public void Apply()
	{
		CupisDatabaseWindow.additionals = CupisClientsAdditionals;
		ViewModelAuth.AMSettings = this;
	}
}
