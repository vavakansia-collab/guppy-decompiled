using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace AccountsLibrary;

public class RandomSettings : INotifyPropertyChanged
{
	private EnumFinanceOptionsRandomBet typeRandomBet = EnumFinanceOptionsRandomBet.Percent;

	private double percent = 7.0;

	private double minSumBet = 50.0;

	private double maxSumBet = 70.0;

	private double minPayoutRatio = 1.2;

	private double maxPayoutRatio = 2.0;

	private bool sendBetTelegram = false;

	public EnumFinanceOptionsRandomBet TypeRandomBet
	{
		get
		{
			return typeRandomBet;
		}
		set
		{
			typeRandomBet = value;
			RaisePropertyChanged("TypeRandomBet");
		}
	}

	public double Percent
	{
		get
		{
			return percent;
		}
		set
		{
			if (value > 100.0)
			{
				value = 100.0;
			}
			percent = value;
			RaisePropertyChanged("Percent");
		}
	}

	public double MinSumBet
	{
		get
		{
			return minSumBet;
		}
		set
		{
			minSumBet = value;
			RaisePropertyChanged("MinSumBet");
		}
	}

	public double MaxSumBet
	{
		get
		{
			return maxSumBet;
		}
		set
		{
			maxSumBet = value;
			RaisePropertyChanged("MaxSumBet");
		}
	}

	public double MinPayoutRatio
	{
		get
		{
			return minPayoutRatio;
		}
		set
		{
			minPayoutRatio = value;
			RaisePropertyChanged("MinPayoutRatio");
		}
	}

	public double MaxPayoutRatio
	{
		get
		{
			return maxPayoutRatio;
		}
		set
		{
			maxPayoutRatio = value;
			RaisePropertyChanged("MaxPayoutRatio");
		}
	}

	public bool SendBetTelegram
	{
		get
		{
			return sendBetTelegram;
		}
		set
		{
			sendBetTelegram = value;
			RaisePropertyChanged("SendBetTelegram");
		}
	}

	public ObservableCollection<IssueTypeChoose> IssueTypeParameters { get; set; }

	public ObservableCollection<ParametersSport> SportsParameters { get; set; } = new ObservableCollection<ParametersSport>();

	public event Action OnUpdate;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		this.OnUpdate?.Invoke();
	}

	public RandomSettings()
	{
	}

	public RandomSettings(AccountSettings accountSettings)
	{
		(from sp in SportBL.Sports
			orderby !sp.IsSlowTimeGoal, sp.IdSport
			select sp).ToList().ForEach(delegate(Sport sp)
		{
			SportsParameters.Add(new ParametersSport
			{
				IdSport = sp.IdSport,
				IsChecked = true
			});
		});
		IssueTypeParameters = new ObservableCollection<IssueTypeChoose>();
		foreach (IssueType value in Enum.GetValues(typeof(IssueType)))
		{
			if (value != IssueType._empty)
			{
				IssueTypeChoose item = new IssueTypeChoose
				{
					IssueType = value
				};
				IssueTypeParameters.Add(item);
			}
		}
	}
}
