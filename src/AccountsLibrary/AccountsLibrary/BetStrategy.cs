using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using StrategyLibrary;

namespace AccountsLibrary;

public class BetStrategy : INotifyPropertyChanged
{
	private string infoHeader = "";

	private double timeLife = 0.0;

	[IgnoreDataMember]
	public OnProcessBetDelegate OnCompliteBetStrategy;

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public Bet Bet { get; set; }

	public StrategyBase CustomStrategy { get; set; }

	public AccountSettings AccountSettings { get; set; }

	public StrategySettingsBase StrategySettings { get; set; }

	public string InfoHeader
	{
		get
		{
			return infoHeader;
		}
		set
		{
			infoHeader = value;
			RaisePropertyChanged("InfoHeader");
		}
	}

	public string UserInfoHeader { get; set; } = "";

	public string Discription { get; set; } = "";

	public string Comment { get; set; } = "";

	public int TotalRandomBets { get; set; } = 0;

	public double TimeLife
	{
		get
		{
			return timeLife;
		}
		set
		{
			timeLife = Math.Round(value, 2);
		}
	}

	public long NumberSDP { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public BetStrategy()
	{
	}

	public BetStrategy(AccountSettings initAccountSettings, Bet initBet, StrategyBase initCustomStrategy, StrategySettingsBase initStrategySettingsBase = null)
	{
		AccountSettings = initAccountSettings;
		Bet = initBet;
		CustomStrategy = initCustomStrategy;
		StrategySettings = initStrategySettingsBase;
	}

	public override string ToString()
	{
		return AccountSettings.NameBCAccount + " " + Bet.TypeBet.ToString() + " " + AccountSettings.Login;
	}
}
