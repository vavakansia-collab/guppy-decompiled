using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BookmakerLibrary;
using CommonLibrary;

namespace AccountsLibrary;

public class PoslegolSettings : StrategySettingsBase, INotifyPropertyChanged
{
	private bool isBetMuchTimeScore = false;

	private SortKoefEnum sortKoef = SortKoefEnum.Down;

	public bool IsBetMuchTimeScore
	{
		get
		{
			return isBetMuchTimeScore;
		}
		set
		{
			isBetMuchTimeScore = value;
			RaisePropertyChanged("IsBetMuchTimeScore");
		}
	}

	public SortKoefEnum SortKoef
	{
		get
		{
			return sortKoef;
		}
		set
		{
			sortKoef = value;
		}
	}

	public PoslegolSettings()
	{
	}

	public PoslegolSettings(AccountSettings accountSettings)
	{
		base.TypeStrategy = TypesStrategy.Poslegoal;
		AddParameterCheckBC(new ParameterAccount
		{
			NameParameter = "Скауты",
			IsChecked = false
		});
		List<string> list = ProvidersCommon.ProvidersNames.Where((string pr) => !pr.EndsWith("Line")).ToList();
		ProvidersCommon.ProvidersNames.Where((string pr) => !pr.EndsWith("Line")).ToList().ForEach(delegate(string pr)
		{
			AddParameterCheckBC(new ParameterAccount
			{
				NameParameter = pr
			});
		});
		(from sp in SportBL.Sports
			orderby !sp.IsSlowTimeGoal, sp.IdSport
			select sp).ToList().ForEach(delegate(Sport sp)
		{
			AddParameterSport(new ParametersSport
			{
				IdSport = sp.IdSport
			});
		});
		base.ParametersCheckBC.Where((ParameterAccount par) => par.NameParameter != "BetcityLine").ToList().ForEach(delegate(ParameterAccount par)
		{
			par.IsChecked = par.NameParameter != "_1Win" && par.NameParameter != "BaltbetRU" && par.NameParameter != "BetBoom" && par.NameParameter != "Betcity" && par.NameParameter != "Marathon" && par.NameParameter != "LeonRU" && par.NameParameter != "LigaStavokRU" && par.NameParameter != "OlimpBet" && par.NameParameter != "Parimatch" && par.NameParameter != "Tennisi" && par.NameParameter != "ZenitWin";
		});
		base.IssueTypeParameters = new ObservableCollection<IssueTypeParameter>();
		foreach (IssueType value in Enum.GetValues(typeof(IssueType)))
		{
			if (value != IssueType._empty)
			{
				AddIssueTypeParameter(new IssueTypeParameter
				{
					IssueType = value
				});
			}
		}
	}
}
