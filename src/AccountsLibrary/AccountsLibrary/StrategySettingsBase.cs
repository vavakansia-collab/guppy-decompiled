using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookmakerLibrary;
using CommonLibrary;

namespace AccountsLibrary;

public class StrategySettingsBase : INotifyPropertyChanged
{
	private string name;

	private bool isActive = true;

	private double minBalance = 0.0;

	private double maxBalance = 1000000.0;

	private double minSumBet = 1000.0;

	private double maxSumBet = 300000.0;

	private bool setPauseBetweenBets = true;

	private double minimalPayoutRatio = 1.3;

	private double maximalPayoutRatio = 999.0;

	private double balancePercent = 100.0;

	private double sumBetConstant = 0.0;

	private bool regularMatches = true;

	private bool cyberMatches = true;

	private bool shortMatches = true;

	private double minMaxSum = 1000.0;

	private double maxMaxSum = 10000000.0;

	private double timeLife = 0.0;

	private double timeLifeUntil = 300.0;

	private bool putAutoRandomBet = false;

	private bool putAutoRandomBetOnlySuccessBet = false;

	private int autoRandomBetsCountMin = 0;

	private int autoRandomBetsCountMax = 3;

	private string filterNamesTags = "";

	private int pauseBetweenBetsMinutes = 2;

	private double timeLastScoreUpdateUntil = 3.0;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public TypesStrategy TypeStrategy { get; set; }

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
			RaisePropertyChanged("Name");
		}
	}

	public DateTime DateTimeCreate { get; set; } = DateTime.Now;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			RaisePropertyChanged("IsActive");
		}
	}

	public double MinBalance
	{
		get
		{
			return minBalance;
		}
		set
		{
			minBalance = value;
			RaisePropertyChanged("MinBalance");
		}
	}

	public double MaxBalance
	{
		get
		{
			return maxBalance;
		}
		set
		{
			maxBalance = value;
			RaisePropertyChanged("MaxBalance");
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

	public bool SetPauseBetweenBets
	{
		get
		{
			return setPauseBetweenBets;
		}
		set
		{
			setPauseBetweenBets = value;
			RaisePropertyChanged("SetPauseBetweenBets");
		}
	}

	public double MinimalPayoutRatio
	{
		get
		{
			return minimalPayoutRatio;
		}
		set
		{
			minimalPayoutRatio = value;
			RaisePropertyChanged("MinimalPayoutRatio");
		}
	}

	public double MaximalPayoutRatio
	{
		get
		{
			return maximalPayoutRatio;
		}
		set
		{
			maximalPayoutRatio = value;
			RaisePropertyChanged("MaximalPayoutRatio");
		}
	}

	public double BalancePercent
	{
		get
		{
			return balancePercent;
		}
		set
		{
			if (value > 100.0)
			{
				value = 100.0;
			}
			balancePercent = value;
			RaisePropertyChanged("BalancePercent");
		}
	}

	public double SumBetConstant
	{
		get
		{
			return sumBetConstant;
		}
		set
		{
			sumBetConstant = value;
			RaisePropertyChanged("SumBetConstant");
		}
	}

	public bool RegularMatches
	{
		get
		{
			return regularMatches;
		}
		set
		{
			regularMatches = value;
			RaisePropertyChanged("RegularMatches");
		}
	}

	public bool CyberMatches
	{
		get
		{
			return cyberMatches;
		}
		set
		{
			cyberMatches = value;
			RaisePropertyChanged("CyberMatches");
		}
	}

	public bool ShortMatches
	{
		get
		{
			return shortMatches;
		}
		set
		{
			shortMatches = value;
			RaisePropertyChanged("ShortMatches");
		}
	}

	public double MinMaxSum
	{
		get
		{
			return minMaxSum;
		}
		set
		{
			minMaxSum = value;
			RaisePropertyChanged("MinMaxSum");
		}
	}

	public double MaxMaxSum
	{
		get
		{
			return maxMaxSum;
		}
		set
		{
			maxMaxSum = value;
			RaisePropertyChanged("MaxMaxSum");
		}
	}

	public double TimeLife
	{
		get
		{
			return timeLife;
		}
		set
		{
			timeLife = value;
			RaisePropertyChanged("TimeLife");
		}
	}

	public double TimeLifeUntil
	{
		get
		{
			return timeLifeUntil;
		}
		set
		{
			timeLifeUntil = value;
			RaisePropertyChanged("TimeLifeUntil");
		}
	}

	public bool PutAutoRandomBet
	{
		get
		{
			return putAutoRandomBet;
		}
		set
		{
			putAutoRandomBet = value;
			RaisePropertyChanged("PutAutoRandomBet");
		}
	}

	public bool PutAutoRandomBetOnlySuccessBet
	{
		get
		{
			return putAutoRandomBetOnlySuccessBet;
		}
		set
		{
			putAutoRandomBetOnlySuccessBet = value;
			RaisePropertyChanged("PutAutoRandomBetOnlySuccessBet");
		}
	}

	public int AutoRandomBetsCountMin
	{
		get
		{
			return autoRandomBetsCountMin;
		}
		set
		{
			autoRandomBetsCountMin = value;
			RaisePropertyChanged("AutoRandomBetsCountMin");
		}
	}

	public int AutoRandomBetsCountMax
	{
		get
		{
			return autoRandomBetsCountMax;
		}
		set
		{
			autoRandomBetsCountMax = value;
			RaisePropertyChanged("AutoRandomBetsCountMax");
		}
	}

	public string FilterNamesTags
	{
		get
		{
			return filterNamesTags;
		}
		set
		{
			filterNamesTags = value;
			RaisePropertyChanged("FilterNamesTags");
		}
	}

	public int PauseBetweenBetsMinutes
	{
		get
		{
			return pauseBetweenBetsMinutes;
		}
		set
		{
			pauseBetweenBetsMinutes = Math.Max(1, value);
			RaisePropertyChanged("PauseBetweenBetsMinutes");
		}
	}

	public TypeInclude TypeIncludeFilterNamesTags { get; set; } = TypeInclude.Filter;

	public double TimeLastScoreUpdateUntil
	{
		get
		{
			return timeLastScoreUpdateUntil;
		}
		set
		{
			timeLastScoreUpdateUntil = value;
			RaisePropertyChanged("TimeLastScoreUpdateUntil");
		}
	}

	public Currency Currency { get; set; }

	public ObservableCollection<IssueTypeParameter> IssueTypeParameters { get; set; }

	public ObservableCollection<ParametersSport> SportsParameters { get; set; } = new ObservableCollection<ParametersSport>();

	public ObservableCollection<ParameterAccount> ParametersCheckBC { get; set; } = new ObservableCollection<ParameterAccount>();

	public event Action OnUpdateStrategySettingsBase;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		this.OnUpdateStrategySettingsBase?.Invoke();
	}

	public void AddIssueTypeParameter(IssueTypeParameter p)
	{
		IssueTypeParameters.Add(p);
		OnAddIssueTypeParameter(p);
	}

	public void OnAddIssueTypeParameter(IssueTypeParameter p)
	{
		p.OnUpdate += delegate
		{
			this.OnUpdateStrategySettingsBase?.Invoke();
		};
	}

	public void AddParameterSport(ParametersSport p)
	{
		SportsParameters.Add(p);
		OnAddParameterSport(p);
	}

	public void OnAddParameterSport(ParametersSport p)
	{
		p.OnUpdate += delegate
		{
			this.OnUpdateStrategySettingsBase?.Invoke();
		};
	}

	public void AddParameterCheckBC(ParameterAccount p)
	{
		if (ParametersCheckBC.SingleOrDefault((ParameterAccount par) => par.NameParameter == p.NameParameter) == null)
		{
			ParametersCheckBC.Add(p);
			OnAddParameterCheckBC(p);
		}
	}

	public void OnAddParameterCheckBC(ParameterAccount p)
	{
		p.OnUpdate += delegate
		{
			this.OnUpdateStrategySettingsBase?.Invoke();
		};
	}

	public ParametersSport GetParametersSports(string sportShortName)
	{
		if (SportsParameters.SingleOrDefault((ParametersSport par) => par.Sport.ShortName == sportShortName) != null)
		{
			return SportsParameters.Single((ParametersSport par) => par.Sport.ShortName == sportShortName);
		}
		ParametersSport parametersSport = null;
		parametersSport = new ParametersSport
		{
			IdSport = SportBL.GetIdSportByName(sportShortName),
			IsChecked = false
		};
		AddParameterSport(parametersSport);
		return parametersSport;
	}
}
