using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BookmakerLibrary;
using CommonLibrary;

namespace AccountsLibrary;

public class ValueScoreSettings : StrategySettingsBase, INotifyPropertyChanged
{
	private bool halfPeriodFirst = false;

	private bool halfPeriodSecond = true;

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

	public ValueScoreSettings()
	{
	}

	public ValueScoreSettings(AccountSettings accountSettings, TypesStrategy initTypesStrategy)
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
		foreach (IssueType value in Enum.GetValues(typeof(IssueType)))
		{
			if (value != IssueType._empty && !value.ToString().Contains("Under") && !value.ToString().Contains("Type"))
			{
				AddIssueTypeParameter(new IssueTypeParameter
				{
					IssueType = value
				});
			}
		}
	}
}
