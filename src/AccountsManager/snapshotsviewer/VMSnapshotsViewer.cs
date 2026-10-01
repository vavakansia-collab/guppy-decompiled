using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using BookmakerLibrary;
using CommonLibrary;
using CompareLibrary;
using SnapshotsLibrary;

namespace SnapshotsViewer;

public class VMSnapshotsViewer : INotifyPropertyChanged
{
	public static VMSnapshotsViewer instance;

	private CollectionViewSource cvsProviders;

	private CollectionViewSource cvsCompareMatches;

	private CollectionViewSource cvsMatchesProvider;

	private CollectionViewSource cvsCnapshots;

	private RelayCommand selectionChangedMatchCommand;

	private RelayCommand selectionChangedCompareMatchCommand;

	private RelayCommand downloadByDataCommand;

	private RelayCommand mouseDoubleClickCommand;

	private RelayCommand searchBySportCommand;

	private RelayCommand clearSearch;

	private SnapshotsLibrary.Match snapshotChooseMatch;

	private Snapshot snapshotChoose;

	private CompareMatch compareMatchChoose;

	private DateTime dataPoint = DateTime.Now;

	private Sport sportChoose;

	private string searchText;

	public CollectionViewSource CVSProviders
	{
		get
		{
			return cvsProviders;
		}
		set
		{
			cvsProviders = value;
			RaisePropertyChanged("CVSProviders");
		}
	}

	public CollectionViewSource CVSCompareMatches
	{
		get
		{
			return cvsCompareMatches;
		}
		set
		{
			cvsCompareMatches = value;
			RaisePropertyChanged("CVSCompareMatches");
		}
	}

	public CollectionViewSource CVSMatchesProvider
	{
		get
		{
			return cvsMatchesProvider;
		}
		set
		{
			cvsMatchesProvider = value;
			RaisePropertyChanged("CVSMatchesProvider");
		}
	}

	public CollectionViewSource CVSCnapshots
	{
		get
		{
			return cvsCnapshots;
		}
		set
		{
			cvsCnapshots = value;
			RaisePropertyChanged("CVSCnapshots");
		}
	}

	public RelayCommand SelectionChangedMatchCommand => selectionChangedMatchCommand ?? (selectionChangedMatchCommand = new RelayCommand(delegate(object obj)
	{
		SnapshotChooseMatch = (SnapshotsLibrary.Match)(obj as ListBox).SelectedItem;
	}));

	public RelayCommand SelectionChangedCompareMatchCommand => selectionChangedCompareMatchCommand ?? (selectionChangedCompareMatchCommand = new RelayCommand(delegate(object obj)
	{
		CompareMatchChoose = (CompareMatch)(obj as ListBox).SelectedItem;
	}));

	public RelayCommand DownloadByDataCommand => downloadByDataCommand ?? (downloadByDataCommand = new RelayCommand(delegate
	{
		DownloadData();
	}));

	public RelayCommand MouseDoubleClickCommand => mouseDoubleClickCommand ?? (mouseDoubleClickCommand = new RelayCommand(delegate
	{
		OnOpenWindowSnapshotMatch();
	}));

	public RelayCommand SearchBySportCommand => searchBySportCommand ?? (searchBySportCommand = new RelayCommand(delegate(object obj)
	{
		SportChoose = (Sport)obj;
		DownloadData();
	}));

	public RelayCommand ClearSearch => clearSearch ?? (clearSearch = new RelayCommand(delegate
	{
		if (!string.IsNullOrEmpty(SearchText))
		{
			SearchText = "";
			CVSCompareMatches.View.Refresh();
		}
	}));

	public SnapshotsLibrary.Match SnapshotChooseMatch
	{
		get
		{
			return snapshotChooseMatch;
		}
		set
		{
			snapshotChooseMatch = value;
			RaisePropertyChanged("SnapshotChooseMatch");
		}
	}

	public Snapshot SnapshotChoose
	{
		get
		{
			return snapshotChoose;
		}
		set
		{
			snapshotChoose = value;
			RaisePropertyChanged("SnapshotChoose");
		}
	}

	public CompareMatch CompareMatchChoose
	{
		get
		{
			return compareMatchChoose;
		}
		set
		{
			compareMatchChoose = value;
			RaisePropertyChanged("CompareMatchChoose");
		}
	}

	public DateTime DataPoint
	{
		get
		{
			return dataPoint;
		}
		set
		{
			dataPoint = value;
			RaisePropertyChanged("DataPoint");
		}
	}

	public Sport SportChoose
	{
		get
		{
			return sportChoose;
		}
		set
		{
			sportChoose = value;
			RaisePropertyChanged("SportChoose");
		}
	}

	public string SearchText
	{
		get
		{
			return searchText;
		}
		set
		{
			searchText = value;
			RaisePropertyChanged("SearchText");
		}
	}

	public ObservableCollection<CompareMatch> CompareMatches { get; set; } = new ObservableCollection<CompareMatch>();

	public Snapshot SnapshotHeaders { get; set; } = new Snapshot();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public VMSnapshotsViewer()
	{
		instance = this;
		SportChoose = SportBL.Sports.First((Sport sp) => sp.IdSport == 10);
		CVSCompareMatches = new CollectionViewSource();
		CVSCompareMatches.SortDescriptions.Add(new SortDescription("TimeCreate", ListSortDirection.Ascending));
		CVSCompareMatches.Filter += CVSCompareMatches_Filter;
		CVSCompareMatches.Source = CompareMatches;
		WatsonClientExchange.Start();
		DownloadData();
	}

	private void CVSCompareMatches_Filter(object sender, FilterEventArgs e)
	{
		if (string.IsNullOrEmpty(SearchText) || SearchText.Trim().Length < 2)
		{
			e.Accepted = true;
			return;
		}
		CompareMatch compareMatch = e.Item as CompareMatch;
		string nameCompareMatch = GetNameCompareMatch(compareMatch);
		string championshipCompareMatch = GetChampionshipCompareMatch(compareMatch);
		if (nameCompareMatch.ToLower().Contains(SearchText.ToLower()) || championshipCompareMatch.ToLower().Contains(SearchText.ToLower()))
		{
			e.Accepted = true;
		}
		else
		{
			e.Accepted = false;
		}
	}

	private void DownloadData()
	{
		CompareMatches.Clear();
		List<CompareMatch> compareMatchesByData = WatsonClientExchange.GetCompareMatchesByData(SportChoose.IdSport, DataPoint);
		if (compareMatchesByData != null)
		{
			CompareMatches.Clear();
			compareMatchesByData.ForEach(delegate(CompareMatch cm)
			{
				CompareMatches.Add(cm);
			});
		}
		List<Snapshot> snapshotHeadersByData = WatsonClientExchange.GetSnapshotHeadersByData(SportChoose.IdSport, DataPoint);
		if (snapshotHeadersByData != null)
		{
			SnapshotHeaders = SnapshotBL.MergeSnapshots(snapshotHeadersByData);
		}
	}

	public string GetNameCompareMatch(CompareMatch compareMatch)
	{
		string result = "";
		SnapshotsLibrary.Match match = SnapshotBL.GetMatch(SnapshotHeaders, compareMatch.MatchesCMP.First().NameProvider, compareMatch.MatchesCMP.First().IdMatch);
		if (match != null)
		{
			result = match.Team1 + " - " + match.Team2;
		}
		return result;
	}

	public string GetChampionshipCompareMatch(CompareMatch compareMatch)
	{
		string result = "";
		SnapshotsLibrary.Match match = SnapshotBL.GetMatch(SnapshotHeaders, compareMatch.MatchesCMP.First().NameProvider, compareMatch.MatchesCMP.First().IdMatch);
		if (match != null)
		{
			result = match.Championship ?? "";
		}
		return result;
	}

	private void OnOpenWindowSnapshotMatch()
	{
		new WindowSnapshotMatch(SnapshotChoose, CompareMatchChoose).Show();
	}

	public void OnSearchTextChanged()
	{
		CVSCompareMatches.View.Refresh();
	}
}
