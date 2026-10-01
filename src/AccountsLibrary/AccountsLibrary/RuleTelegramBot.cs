using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class RuleTelegramBot : INotifyPropertyChanged
{
	private bool availability = false;

	private bool addMatches = false;

	private bool editMatches = false;

	private int maxActiveAccounts = 10;

	public bool Availability
	{
		get
		{
			return availability;
		}
		set
		{
			availability = value;
			RaisePropertyChanged("Availability");
		}
	}

	public bool AddMatches
	{
		get
		{
			return addMatches;
		}
		set
		{
			addMatches = value;
			RaisePropertyChanged("AddMatches");
		}
	}

	public bool EditMatches
	{
		get
		{
			return editMatches;
		}
		set
		{
			editMatches = value;
			RaisePropertyChanged("EditMatches");
		}
	}

	public int MaxActiveAccounts
	{
		get
		{
			return maxActiveAccounts;
		}
		set
		{
			maxActiveAccounts = value;
			RaisePropertyChanged("MaxActiveAccounts");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
