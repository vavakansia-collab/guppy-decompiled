using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using BookmakerLibrary;
using CommonLibrary;
using CompareLibrary;
using SnapshotsLibrary;

namespace SnapshotsViewer;

public class VMSnapshotMatch : INotifyPropertyChanged
{
	private Snapshot snapshot;

	private CompareMatch compareMatch;

	private CollectionViewSource cvsTimeLines;

	private bool showScore = true;

	private bool showCoupons;

	private IssueType issueTypeChoose;

	private RelayCommand refreshViewCommand;

	private RelayCommand chooseTypesCoupons;

	private RelayCommand couponSelectedChangedCommand;

	public string NameWindow { get; set; }

	public string NameMatch { get; set; }

	public string NameChampionship { get; set; }

	public Snapshot Snapshot
	{
		get
		{
			return snapshot;
		}
		set
		{
			snapshot = value;
			RaisePropertyChanged("Snapshot");
		}
	}

	public CompareMatch CompareMatch
	{
		get
		{
			return compareMatch;
		}
		set
		{
			compareMatch = value;
			RaisePropertyChanged("CompareMatch");
		}
	}

	public CollectionViewSource CVSTimeLines
	{
		get
		{
			return cvsTimeLines;
		}
		set
		{
			cvsTimeLines = value;
			RaisePropertyChanged("CVSTimeLines");
		}
	}

	public bool ShowScore
	{
		get
		{
			return showScore;
		}
		set
		{
			showScore = value;
			RaisePropertyChanged("ShowScore");
		}
	}

	public bool ShowCoupons
	{
		get
		{
			return showCoupons;
		}
		set
		{
			showCoupons = value;
			RaisePropertyChanged("ShowCoupons");
		}
	}

	public IssueType IssueTypeChoose
	{
		get
		{
			return issueTypeChoose;
		}
		set
		{
			issueTypeChoose = value;
			RaisePropertyChanged("IssueTypeChoose");
		}
	}

	public RelayCommand RefreshViewCommand => refreshViewCommand ?? (refreshViewCommand = new RelayCommand(delegate
	{
		RefreshView();
	}));

	public RelayCommand ChooseTypesCoupons => chooseTypesCoupons ?? (chooseTypesCoupons = new RelayCommand(delegate
	{
		bool isChoose = TypesCoupons.First().IsChoose;
		TypesCoupons.ForEach(delegate(TypeCoupons tc)
		{
			tc.IsChoose = !isChoose;
		});
	}));

	public RelayCommand CouponSelectedChangedCommand => couponSelectedChangedCommand ?? (couponSelectedChangedCommand = new RelayCommand(delegate(object obj)
	{
		IssueTypeChoose = (obj as TypeCoupons).IssueType;
	}));

	public ObservableCollection<TimeLine> TimeLines { get; set; } = new ObservableCollection<TimeLine>();

	public List<SnapshotsLibrary.Bookmaker> Bookmakers { get; set; } = new List<SnapshotsLibrary.Bookmaker>();

	public List<TypeCoupons> TypesCoupons { get; set; } = new List<TypeCoupons>();

	public List<ProviderMatch> ProvidersMatch { get; set; } = new List<ProviderMatch>();

	public List<PeriodMatch> PeriodsMatch { get; set; } = new List<PeriodMatch>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public VMSnapshotMatch(Snapshot snapshotHeader, CompareMatch initCompareMatch)
	{
		CompareMatch = initCompareMatch;
		NameMatch = VMSnapshotsViewer.instance.GetNameCompareMatch(CompareMatch);
		NameWindow = "Ход матче " + NameMatch;
		NameChampionship = VMSnapshotsViewer.instance.GetChampionshipCompareMatch(CompareMatch);
		foreach (IssueType value in Enum.GetValues(typeof(IssueType)))
		{
			TypesCoupons.Add(new TypeCoupons
			{
				IssueType = value
			});
		}
		string periodName = SportBL.GetSportByID(initCompareMatch.IDSport).PeriodName;
		for (int i = 0; i <= 5; i++)
		{
			string text = "";
			switch (periodName)
			{
			case "тайм":
			case "период":
			case "сет":
			case "иннинг":
				text = "-й";
				break;
			case "четверть":
			case "партия":
				text = "-я";
				break;
			}
			string name = ((i == 0) ? "Весь матч" : (i + text + " " + periodName));
			PeriodsMatch.Add(new PeriodMatch
			{
				Period = i,
				Name = name
			});
		}
		CVSTimeLines = new CollectionViewSource();
		CVSTimeLines.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Ascending));
		CVSTimeLines.Source = TimeLines;
		Snapshot = WatsonClientExchange.GetSnapshotMatch(CompareMatch);
		if (Snapshot != null)
		{
			Snapshot.Bookmakers.OrderBy((SnapshotsLibrary.Bookmaker pr) => pr.Name).ToList().ForEach(delegate(SnapshotsLibrary.Bookmaker pr)
			{
				ProvidersMatch.Add(new ProviderMatch
				{
					Name = pr.Name
				});
			});
			RefreshView();
		}
	}

	public void RefreshView()
	{
		TimeLines.Clear();
		foreach (CompareMatchParameter cmp in CompareMatch.MatchesCMP)
		{
			if (!ProvidersMatch.Single((ProviderMatch pr) => pr.Name == cmp.NameProvider).IsChoose)
			{
				continue;
			}
			SnapshotsLibrary.Match match = Snapshot.Bookmakers.Single((SnapshotsLibrary.Bookmaker bc) => bc.Name == cmp.NameProvider).Matches.Single((SnapshotsLibrary.Match m) => m.Id == cmp.IdMatch);
			if (ShowScore)
			{
				foreach (SnapshotsLibrary.Match.ScoreFrame scoreFrame in match.ScoreHistory)
				{
					int periodScoreFrame = Score.GetCurrentPeriod(scoreFrame.Value);
					if (PeriodsMatch.SingleOrDefault((PeriodMatch p) => p.IsChoose && p.Period == periodScoreFrame) != null)
					{
						TimeLine timeLine = TimeLines.SingleOrDefault((TimeLine tl) => tl.Timestamp.ToString() == scoreFrame.Timestamp.ToString());
						if (timeLine == null)
						{
							timeLine = new TimeLine();
							timeLine.Period = periodScoreFrame;
							timeLine.Timestamp = scoreFrame.Timestamp;
							TimeLines.Add(timeLine);
						}
						TimestampProvider timestampProvider = timeLine.TimestampProviders.SingleOrDefault((TimestampProvider t) => t.NameProvider == cmp.NameProvider);
						if (timestampProvider != null)
						{
							timestampProvider.Score = scoreFrame.Value;
						}
					}
				}
			}
			if (!ShowCoupons)
			{
				continue;
			}
			foreach (SnapshotsLibrary.Coupon coupon in match.Coupons.Where((SnapshotsLibrary.Coupon c) => (from tc in TypesCoupons
				where tc.IsChoose
				select tc.IssueType).Contains(c.IssueType)))
			{
				if (PeriodsMatch.SingleOrDefault((PeriodMatch p) => p.IsChoose && p.Period == coupon.Period) == null)
				{
					continue;
				}
				foreach (SnapshotsLibrary.Coupon.CoefficientFrame coefficientFrame in coupon.CoefficientHistory.OrderBy((SnapshotsLibrary.Coupon.CoefficientFrame h) => h.Timestamp))
				{
					TimeLine timeLine2 = TimeLines.SingleOrDefault((TimeLine tl) => tl.Timestamp.ToString() == coefficientFrame.Timestamp.ToString());
					if (timeLine2 == null)
					{
						timeLine2 = new TimeLine();
						timeLine2.Timestamp = coefficientFrame.Timestamp;
						TimeLines.Add(timeLine2);
					}
					TimestampProvider timestampProvider2 = timeLine2.TimestampProviders.SingleOrDefault((TimestampProvider t) => t.NameProvider == cmp.NameProvider);
					if (timestampProvider2 != null)
					{
						BookmakerLibrary.Coupon coupon2 = new BookmakerLibrary.Coupon();
						coupon2.Availability = coupon.Availability;
						coupon2.IssueType = coupon.IssueType;
						coupon2.IssueValue = coupon.IssueValue;
						coupon2.Period = coupon.Period;
						coupon2.GameTennis = coupon.GameTennis;
						coupon2.PayoutRatio = coefficientFrame.Value;
						timestampProvider2.Coupons.Add(coupon2);
					}
				}
			}
		}
		List<string> emptyNamesProviders = new List<string>();
		ProvidersCommon.ProvidersNames.ForEach(delegate(string pr)
		{
			emptyNamesProviders.Add(pr);
		});
		foreach (TimeLine timeLine3 in TimeLines)
		{
			foreach (TimestampProvider timestampProvider3 in timeLine3.TimestampProviders)
			{
				if (!string.IsNullOrEmpty(timestampProvider3.Score) || timestampProvider3.Coupons.Count > 0)
				{
					emptyNamesProviders.Remove(timestampProvider3.NameProvider);
				}
			}
		}
		List<TimestampProvider> emptyTsp = new List<TimestampProvider>();
		foreach (TimeLine timeLine4 in TimeLines)
		{
			foreach (TimestampProvider timestampProvider4 in timeLine4.TimestampProviders)
			{
				if (emptyNamesProviders.Contains(timestampProvider4.NameProvider))
				{
					emptyTsp.Add(timestampProvider4);
				}
			}
			timeLine4.TimestampProviders.RemoveAll((TimestampProvider tsp) => emptyTsp.Contains(tsp));
			emptyTsp.Clear();
		}
		ProvidersCommon.ProvidersNames.Where((string pr) => !emptyNamesProviders.Contains(pr)).ToList().ForEach(delegate(string pr)
		{
			Bookmakers.Add(new SnapshotsLibrary.Bookmaker
			{
				Name = pr
			});
		});
	}

	private (List<DateTime>, List<double>) GetScoreCoordinates(SnapshotsLibrary.Bookmaker bookmaker, int period)
	{
		List<DateTime> list = new List<DateTime>();
		List<double> list2 = new List<double>();
		foreach (SnapshotsLibrary.Match.ScoreFrame item in bookmaker.Matches.First().ScoreHistory)
		{
			string value = item.Value;
			if (period <= 0 || Score.GetCurrentPeriod(value) == period)
			{
				string scorePeriod = Score.GetScorePeriod(value, period);
				if (period <= 0 || !(scorePeriod == "0:0"))
				{
					int num = int.Parse(scorePeriod.Split(':')[0]);
					int num2 = int.Parse(scorePeriod.Split(':')[1]);
					int num3 = num + num2;
					list.Add(item.Timestamp);
					list2.Add(num3);
				}
			}
		}
		return (list, list2);
	}

	private (List<DateTime>, List<double>) GetCouponCoordinates(SnapshotsLibrary.Bookmaker bookmaker, int period)
	{
		List<DateTime> list = new List<DateTime>();
		List<double> list2 = new List<double>();
		SnapshotsLibrary.Coupon coupon = bookmaker.Matches.First().Coupons.Where((SnapshotsLibrary.Coupon c) => c.Period == period && c.IssueType == IssueTypeChoose).FirstOrDefault();
		if (coupon == null)
		{
			return (list, list2);
		}
		foreach (SnapshotsLibrary.Coupon.CoefficientFrame item in coupon.CoefficientHistory)
		{
			list.Add(item.Timestamp);
			list2.Add(item.Value);
		}
		return (list, list2);
	}
}
