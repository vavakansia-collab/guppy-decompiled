using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BookmakerLibrary;
using CommonLibrary;

namespace AccountsLibrary;

public class ValueSettings : StrategySettingsBase, INotifyPropertyChanged
{
	private double minPayoutRatioCompare = 1.01;

	private double maxPayoutRatioCompare = 1.5;

	private double minPercentage = 50.0;

	private bool halfPeriodFirst = false;

	private bool halfPeriodSecond = true;

	public double MinPayoutRatioCompare
	{
		get
		{
			return minPayoutRatioCompare;
		}
		set
		{
			minPayoutRatioCompare = value;
			RaisePropertyChanged("MinPayoutRatioCompare");
		}
	}

	public double MaxPayoutRatioCompare
	{
		get
		{
			return maxPayoutRatioCompare;
		}
		set
		{
			maxPayoutRatioCompare = value;
			RaisePropertyChanged("MaxPayoutRatioCompare");
		}
	}

	public double MinPercentage
	{
		get
		{
			return minPercentage;
		}
		set
		{
			minPercentage = value;
			RaisePropertyChanged("MinPercentage");
		}
	}

	public bool HalfPeriodFirst
	{
		get
		{
			return halfPeriodFirst;
		}
		set
		{
			halfPeriodFirst = value;
			RaisePropertyChanged("HalfPeriodFirst");
		}
	}

	public bool HalfPeriodSecond
	{
		get
		{
			return halfPeriodSecond;
		}
		set
		{
			halfPeriodSecond = value;
			RaisePropertyChanged("HalfPeriodSecond");
		}
	}

	public ValueSettings()
	{
	}

	public ValueSettings(AccountSettings accountSettings, TypesStrategy initTypesStrategy)
	{
		base.TypeStrategy = initTypesStrategy;
		base.IssueTypeParameters = new ObservableCollection<IssueTypeParameter>();
		ProvidersCommon.ProvidersNames.Where((string pr) => !pr.EndsWith("Line")).ToList().ForEach(delegate(string pr)
		{
			AddParameterCheckBC(new ParameterAccount
			{
				NameParameter = pr,
				IsChecked = (pr != "1Win" && pr != "BaltbetRU" && pr != "LigaStavokRU" && pr != "Marathon" && pr != "OlimpBet" && pr != "Parimatch" && pr != "Tennisi" && pr != "ZenitWin")
			});
		});
		(from sp in SportBL.Sports
			orderby !sp.IsSlowTimeGoal, sp.IdSport
			select sp).ToList().ForEach(delegate(Sport sp)
		{
			AddParameterSport(new ParametersSport
			{
				IdSport = sp.IdSport,
				IsChecked = !sp.IsSlowTimeGoal
			});
		});
		foreach (IssueType issueType in Enum.GetValues(typeof(IssueType)))
		{
			if (issueType != IssueType._empty)
			{
				AddIssueTypeParameter(new IssueTypeParameter
				{
					IssueType = issueType,
					Points0 = false
				});
			}
		}
	}
}
