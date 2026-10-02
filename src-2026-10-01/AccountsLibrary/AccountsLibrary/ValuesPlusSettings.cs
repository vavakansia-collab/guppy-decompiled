using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BookmakerLibrary;
using CommonLibrary;

namespace AccountsLibrary;

public class ValuesPlusSettings : StrategySettingsBase, INotifyPropertyChanged
{
	private bool halfPeriodFirst = false;

	private bool halfPeriodSecond = true;

	private bool onlyValueEmpty = false;

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

	public bool OnlyValueEmpty
	{
		get
		{
			return onlyValueEmpty;
		}
		set
		{
			onlyValueEmpty = value;
			RaisePropertyChanged("OnlyValueEmpty");
		}
	}

	public ValuesPlusSettings()
	{
	}

	public ValuesPlusSettings(AccountSettings accountSettings, TypesStrategy initTypesStrategy)
	{
		base.TypeStrategy = initTypesStrategy;
		base.IssueTypeParameters = new ObservableCollection<IssueTypeParameter>();
		ProvidersCommon.ProvidersNames.Where((string pr) => !pr.EndsWith("Line")).ToList().ForEach(delegate(string pr)
		{
			AddParameterCheckBC(new ParameterAccount
			{
				NameParameter = pr,
				IsChecked = (pr != "1Win" && pr != "BaltbetRU" && pr != "BetBoom" && pr != "LeonRU" && pr != "LigaStavokRU" && pr != "Marathon" && pr != "OlimpBet" && pr != "Parimatch" && pr != "Tennisi" && pr != "ZenitWin")
			});
		});
		(from sp in SportBL.Sports
			where !sp.IsSlowTimeGoal
			orderby sp.IdSport
			select sp).ToList().ForEach(delegate(Sport sp)
		{
			AddParameterSport(new ParametersSport
			{
				IdSport = sp.IdSport,
				IsChecked = true
			});
		});
		foreach (IssueType issueType in Enum.GetValues(typeof(IssueType)))
		{
			if (issueType != IssueType._empty && (issueType.ToString().Contains("Total") || issueType.ToString().Contains("OddsTeam_")) && !issueType.ToString().Contains("Tennis") && !issueType.ToString().Contains("Type"))
			{
				AddIssueTypeParameter(new IssueTypeParameter
				{
					IssueType = issueType
				});
			}
		}
	}
}
