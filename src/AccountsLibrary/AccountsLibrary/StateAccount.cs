using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class StateAccount : INotifyPropertyChanged
{
	private bool isRunning = false;

	private double balance = 0.0;

	private string statusService = "";

	public Guid GUIDAccount { get; set; }

	public bool IsRunning
	{
		get
		{
			return isRunning;
		}
		set
		{
			isRunning = value;
			RaisePropertyChanged("IsRunning");
		}
	}

	public double Balance
	{
		get
		{
			return balance;
		}
		set
		{
			balance = value;
			RaisePropertyChanged("Balance");
		}
	}

	public string StatusService
	{
		get
		{
			return statusService;
		}
		set
		{
			statusService = value;
			RaisePropertyChanged("StatusService");
		}
	}

	public ObservableCollection<BetStrategy> Bets { get; set; } = new ObservableCollection<BetStrategy>();

	public ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public StateAccount(Guid initGUIDAccount)
	{
		GUIDAccount = initGUIDAccount;
	}

	public override string ToString()
	{
		return GUIDAccount.ToString();
	}
}
