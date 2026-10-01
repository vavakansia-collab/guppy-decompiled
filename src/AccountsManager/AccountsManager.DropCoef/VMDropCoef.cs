using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BookmakerLibrary;
using CommonLibrary;
using LiveChartsCore.Defaults;
using Newtonsoft.Json;
using SkiaSharp;
using SnapshotsLibrary;

namespace AccountsManager.DropCoef;

public class VMDropCoef : INotifyPropertyChanged
{
	public static readonly string DCSETTINGSPATH = Environment.CurrentDirectory + "\\DCSettings.gb";

	public static VMDropCoef instance = null;

	private WindowMatchLine WindowMatchLine;

	private string windowName = "Падение коэф-тов";

	private bool uiActive = true;

	private MatchLineVM matchLineVMSelected;

	private bool showNotActiveCoef;

	private double minCurPercent = -1000.0;

	private double maxCurPercent = 1000.0;

	private double minDevPercent = -1000.0;

	private double maxDevPercent = 1000.0;

	private CollectionViewSource cvsMatchesProvider;

	private string searchText;

	private RelayCommand clearSearch;

	private RelayCommand selectionChangedMatch;

	private RelayCommand hyperlinkMatchUrlCommand;

	private RelayCommand searchBySportCommand;

	private RelayCommand searchByProviderCommand;

	private RelayCommand favoriteMatchLineCommand;

	private RelayCommand onlyFavoritesDisplayCommand;

	private RelayCommand copyUrlCommand;

	private int favoriteTotal;

	private DateTime dataPoint = DateTime.Now.AddHours(2.0);

	private readonly Random _random = new Random();

	private TimeIntervalItem selectedInterval;

	private TimeType selectedTimeType;

	private DateTime dateStartPoint = DateTime.Now;

	public string WindowName
	{
		get
		{
			return windowName;
		}
		set
		{
			windowName = value;
			RaisePropertyChanged("WindowName");
		}
	}

	public bool UIActive
	{
		get
		{
			return uiActive;
		}
		set
		{
			uiActive = value;
			RaisePropertyChanged("UIActive");
		}
	}

	public MatchLineVM MatchLineVMSelected
	{
		get
		{
			return matchLineVMSelected;
		}
		set
		{
			MatchLineVM matchLineVM = matchLineVMSelected;
			matchLineVMSelected = value;
			if (value != null && matchLineVMSelected != matchLineVM)
			{
				CreateWindowMatchLine();
			}
			RaisePropertyChanged("MatchLineVMSelected");
		}
	}

	public bool ShowNotActiveCoef
	{
		get
		{
			return showNotActiveCoef;
		}
		set
		{
			showNotActiveCoef = value;
			RaisePropertyChanged("ShowNotActiveCoef");
			CVSMatchesProviderRefresh();
		}
	}

	public double MinCurPercent
	{
		get
		{
			return minCurPercent;
		}
		set
		{
			minCurPercent = value;
			RaisePropertyChanged("MinCurPercent");
			CVSMatchesProviderRefresh();
		}
	}

	public double MaxCurPercent
	{
		get
		{
			return maxCurPercent;
		}
		set
		{
			maxCurPercent = value;
			RaisePropertyChanged("MaxCurPercent");
			CVSMatchesProviderRefresh();
		}
	}

	public double MinDevPercent
	{
		get
		{
			return minDevPercent;
		}
		set
		{
			minDevPercent = value;
			RaisePropertyChanged("MinDevPercent");
			CVSMatchesProviderRefresh();
		}
	}

	public double MaxDevPercent
	{
		get
		{
			return maxDevPercent;
		}
		set
		{
			maxDevPercent = value;
			RaisePropertyChanged("MaxDevPercent");
			CVSMatchesProviderRefresh();
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

	public RelayCommand ClearSearch => clearSearch ?? (clearSearch = new RelayCommand(delegate
	{
		if (!string.IsNullOrEmpty(SearchText))
		{
			SearchText = "";
			CVSMatchesProviderRefresh();
		}
	}));

	public RelayCommand SelectionChangedMatch => selectionChangedMatch ?? (selectionChangedMatch = new RelayCommand(delegate(object obj)
	{
		if ((obj as DataGrid).SelectedValue != null)
		{
			MatchLineVMSelected = (obj as DataGrid).SelectedValue as MatchLineVM;
			UpdateDataMatchLine();
			if (WindowMatchLine != null)
			{
				WindowMatchLine.VMMatchLine.UpdateChars();
			}
		}
	}));

	public RelayCommand HyperlinkMatchUrlCommand => hyperlinkMatchUrlCommand ?? (hyperlinkMatchUrlCommand = new RelayCommand(delegate
	{
		if (MatchLineVMSelected != null && !string.IsNullOrEmpty(MatchLineVMSelected.MatchLine.Url))
		{
			Process.Start(MatchLineVMSelected.MatchLine.Url);
		}
	}));

	public RelayCommand SearchBySportCommand => searchBySportCommand ?? (searchBySportCommand = new RelayCommand(delegate(object obj)
	{
		SportsChoosed.ForEach(delegate(SportChoose sp)
		{
			sp.IsSelected = false;
		});
		((SportChoose)obj).IsSelected = true;
		DownloadData();
	}));

	public RelayCommand SearchByProviderCommand => searchByProviderCommand ?? (searchByProviderCommand = new RelayCommand(delegate(object obj)
	{
		ProvidersChooses.ForEach(delegate(ProviderChoose pr)
		{
			pr.IsSelected = false;
		});
		((ProviderChoose)obj).IsSelected = true;
		DownloadData();
	}));

	public RelayCommand FavoriteMatchLineCommand => favoriteMatchLineCommand ?? (favoriteMatchLineCommand = new RelayCommand(delegate(object obj)
	{
		MatchLineVM matchLineVM = (MatchLineVM)obj;
		matchLineVM.IsFavorite = !matchLineVM.IsFavorite;
		MatchLineVM matchLineVM2 = FavoriteMatches.SingleOrDefault((MatchLineVM m) => m.MatchLine.Equals(matchLineVM.MatchLine));
		if (matchLineVM.IsFavorite && matchLineVM2 == null)
		{
			AddFavoriteMatch(matchLineVM);
		}
		else if (!matchLineVM.IsFavorite && matchLineVM2 != null)
		{
			RemoveFavoriteMatch(matchLineVM2);
		}
	}));

	public RelayCommand OnlyFavoritesDisplayCommand => onlyFavoritesDisplayCommand ?? (onlyFavoritesDisplayCommand = new RelayCommand(delegate
	{
		MatchesLineVM.Clear();
		FavoriteMatches.ToList().ForEach(delegate(MatchLineVM f)
		{
			MatchesLineVM.Add(f);
		});
	}));

	public RelayCommand CopyUrlCommand => copyUrlCommand ?? (copyUrlCommand = new RelayCommand(delegate
	{
		Clipboard.SetText(MatchLineVMSelected.MatchLine.Url);
	}));

	public int FavoriteTotal
	{
		get
		{
			return favoriteTotal;
		}
		set
		{
			favoriteTotal = value;
			RaisePropertyChanged("FavoriteTotal");
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

	public List<ProviderChoose> ProvidersChooses { get; set; } = new List<ProviderChoose>();

	public ObservableCollection<MatchLineVM> MatchesLineVM { get; set; } = new ObservableCollection<MatchLineVM>();

	public ObservableCollection<DateTimePoint> CoefPoints { get; set; }

	public List<SportChoose> SportsChoosed { get; set; } = new List<SportChoose>();

	public TimeIntervalItem SelectedInterval
	{
		get
		{
			return selectedInterval;
		}
		set
		{
			if (selectedInterval != value)
			{
				selectedInterval = value;
				RaisePropertyChanged("SelectedInterval");
				if (ProvidersChooses.Count > 0)
				{
					DownloadData();
				}
			}
		}
	}

	public TimeType SelectedTimeType
	{
		get
		{
			return selectedTimeType;
		}
		set
		{
			if (selectedTimeType != value)
			{
				selectedTimeType = value;
				RaisePropertyChanged("SelectedTimeType");
				if (ProvidersChooses.Count > 0)
				{
					DownloadData();
				}
			}
		}
	}

	public DateTime DateStartPoint
	{
		get
		{
			return dateStartPoint;
		}
		set
		{
			if (!(DateTime.Now.Date < value.Date) && !(dateStartPoint.Date == value.Date))
			{
				dateStartPoint = value;
				RaisePropertyChanged("DateStartPoint");
				if (ProvidersChooses.Count > 0)
				{
					DownloadData();
				}
			}
		}
	}

	public IEnumerable<TimeType> TimeTypeOptions { get; }

	public List<TimeIntervalItem> Intervals { get; set; } = new List<TimeIntervalItem>();

	public ObservableCollection<MatchLineVM> FavoriteMatches { get; set; } = new ObservableCollection<MatchLineVM>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public VMDropCoef()
	{
		instance = this;
		Intervals = new List<TimeIntervalItem>
		{
			new TimeIntervalItem
			{
				Name = "Всё время",
				Hours = 0
			},
			new TimeIntervalItem
			{
				Name = "1 час",
				Hours = 1
			},
			new TimeIntervalItem
			{
				Name = "2 часа",
				Hours = 2
			},
			new TimeIntervalItem
			{
				Name = "4 часа",
				Hours = 4
			},
			new TimeIntervalItem
			{
				Name = "6 часов",
				Hours = 6
			},
			new TimeIntervalItem
			{
				Name = "12 часов",
				Hours = 12
			},
			new TimeIntervalItem
			{
				Name = "24 часа",
				Hours = 24
			}
		};
		SelectedInterval = Intervals[2];
		TimeTypeOptions = Enum.GetValues(typeof(TimeType)).Cast<TimeType>();
		SelectedTimeType = TimeType.Current;
		ProvidersChooses.Add(new ProviderChoose
		{
			Provider = new Provider("Betcity")
		});
		ProvidersChooses.Add(new ProviderChoose
		{
			Provider = new Provider("FonbetRU")
		});
		ProvidersChooses.Add(new ProviderChoose
		{
			Provider = new Provider("Winline")
		});
		if (ViewModelMainWindow.AMSettings.OwnerAccounts.Name == "Guppy")
		{
			ProvidersChooses.Single((ProviderChoose pr) => pr.Provider.Name == "FonbetRU").IsSelected = true;
		}
		else
		{
			ProvidersChooses.First().IsSelected = true;
		}
		WatsonDataServerLineClient.Start();
		CVSMatchesProvider = new CollectionViewSource();
		CVSMatchesProvider.SortDescriptions.Add(new SortDescription("MatchLine.StartTime", ListSortDirection.Ascending));
		CVSMatchesProvider.SortDescriptions.Add(new SortDescription("MatchLine.NameChamp", ListSortDirection.Ascending));
		CVSMatchesProvider.SortDescriptions.Add(new SortDescription("MatchLine.Team1", ListSortDirection.Ascending));
		CVSMatchesProvider.Source = MatchesLineVM;
		CVSMatchesProvider.Filter += CVSMatchesProvider_Filter;
		SportBL.Sports.ToList().ForEach(delegate(Sport sp)
		{
			SportsChoosed.Add(new SportChoose
			{
				Sport = sp
			});
		});
		SportsChoosed.Single((SportChoose sp) => sp.Sport.IdSport == 1).IsSelected = true;
		Load();
		Run();
	}

	public void CreateWindowMatchLine()
	{
		if (MatchLineVMSelected != null && (WindowMatchLine == null || WindowMatchLine.VMMatchLine.MatchLineVMSelected != MatchLineVMSelected))
		{
			if (WindowMatchLine == null || !WindowMatchLine.IsLoaded)
			{
				WindowMatchLine = new WindowMatchLine(MatchLineVMSelected);
				WindowMatchLine.Show();
			}
			else
			{
				WindowMatchLine.VMMatchLine.MatchLineVMSelected = MatchLineVMSelected;
			}
		}
	}

	private void SaveColumnsState()
	{
	}

	private void LoadColumnsState()
	{
	}

	private void CVSMatchesProvider_Filter(object sender, FilterEventArgs e)
	{
		MatchLineVM matchLineVM = e.Item as MatchLineVM;
		if (!ShowNotActiveCoef && matchLineVM.DeviationPercent == 0.0 && matchLineVM.MaxDeviationPercent == 0.0)
		{
			e.Accepted = false;
		}
		if (matchLineVM.DeviationPercent < MinCurPercent)
		{
			e.Accepted = false;
		}
		if (matchLineVM.DeviationPercent > MaxCurPercent)
		{
			e.Accepted = false;
		}
		if (matchLineVM.MaxDeviationPercent < MinDevPercent)
		{
			e.Accepted = false;
		}
		if (matchLineVM.MaxDeviationPercent > MaxDevPercent)
		{
			e.Accepted = false;
		}
		if (!string.IsNullOrEmpty(SearchText) && !matchLineVM.MatchLine.Name.ToUpper().Contains(SearchText.ToUpper()))
		{
			e.Accepted = false;
		}
	}

	private void CVSMatchesProviderRefresh()
	{
		CVSMatchesProvider.View.Refresh();
	}

	public async void Run()
	{
		await DownloadData();
		MatchLineVMSelected = MatchesLineVM.FirstOrDefault();
	}

	public void BlockUI()
	{
		UIActive = false;
	}

	public void UnblockUI()
	{
		UIActive = true;
	}

	public async Task DownloadData()
	{
		if (SelectedTimeType == TimeType.Current)
		{
			await DownloadDataCurrent();
		}
		else
		{
			await DownloadAtData();
		}
	}

	private async Task DownloadDataCurrent()
	{
		BlockUI();
		MatchesLineVM.Clear();
		(List<MatchLine>, List<MatchLineStat>, Dictionary<int, int>) tuple = await WatsonDataServerLineClient.GetMatchesCurrent(ProvidersChooses.Single((ProviderChoose pr) => pr.IsSelected).Provider.Name, SportsChoosed.Single((SportChoose sp) => sp.IsSelected).Sport.IdSport, SelectedInterval.Hours);
		if (tuple.Item1 != null)
		{
			foreach (MatchLine matchLine in tuple.Item1)
			{
				MatchLineVM matchLineVM = new MatchLineVM
				{
					MatchLine = matchLine
				};
				matchLineVM.IsFavorite = FavoriteMatches.SingleOrDefault((MatchLineVM m) => m.MatchLine.IsEquals(matchLineVM.MatchLine)) != null;
				MatchLineStat matchLineStat = tuple.Item2.SingleOrDefault((MatchLineStat ml) => ml.NameProvider == matchLine.NameProvider && ml.IdMatch == matchLine.IdMatch);
				if (matchLineStat != null)
				{
					matchLineVM.Score = matchLineStat.Score;
					CouponStat couponStat = matchLineStat.CouponsStats.SingleOrDefault((CouponStat cs) => cs.CouponLine.IssueType == IssueType.VictoryTeam_1);
					CouponStat couponStat2 = matchLineStat.CouponsStats.SingleOrDefault((CouponStat cs) => cs.CouponLine.IssueType == IssueType.VictoryTeam_2);
					if (couponStat != null && couponStat2 != null)
					{
						CouponStat couponStat3 = ((couponStat.CouponLine.PayoutRatio < couponStat2.CouponLine.PayoutRatio) ? couponStat : couponStat2);
						matchLineVM.MaxDeviationPercent = couponStat3.MaxDeviationCoef;
						matchLineVM.Volume = couponStat3.CouponLine.HistoryCoefs.Count;
						if (couponStat3.CouponLine.HistoryCoefs.Count > 1)
						{
							matchLineVM.DeviationPercent = Math.Round((couponStat3.CouponLine.PayoutRatio / couponStat3.CouponLine.HistoryCoefs.First().PayoutRatio - 1.0) * 100.0);
						}
					}
				}
				if (MatchesLineVM.SingleOrDefault((MatchLineVM mv) => mv.MatchLine.NameProvider == matchLine.NameProvider && mv.MatchLine.IdMatch == matchLine.IdMatch) == null)
				{
					MatchesLineVM.Add(matchLineVM);
				}
			}
			SportsChoosed.ForEach(delegate(SportChoose sp)
			{
				sp.TotalSelected = 0;
			});
			foreach (SportChoose item in SportsChoosed)
			{
				if (tuple.Item3.TryGetValue(item.Sport.IdSport, out var value))
				{
					item.TotalSelected = value;
				}
			}
		}
		UnblockUI();
	}

	private async Task DownloadAtData()
	{
		BlockUI();
		MatchesLineVM.Clear();
		(List<MatchLine>, List<MatchLineStat>, Dictionary<int, int>) tuple = await WatsonDataServerLineClient.GetMatchesAtData(ProvidersChooses.Single((ProviderChoose pr) => pr.IsSelected).Provider.Name, SportsChoosed.Single((SportChoose sp) => sp.IsSelected).Sport.IdSport, DateStartPoint);
		if (tuple.Item1 != null)
		{
			foreach (MatchLine matchLine in tuple.Item1)
			{
				MatchLineVM matchLineVM = new MatchLineVM
				{
					MatchLine = matchLine
				};
				matchLineVM.IsFavorite = FavoriteMatches.SingleOrDefault((MatchLineVM m) => m.MatchLine.IsEquals(matchLineVM.MatchLine)) != null;
				if (MatchesLineVM.SingleOrDefault((MatchLineVM mv) => mv.MatchLine.NameProvider == matchLine.NameProvider && mv.MatchLine.IdMatch == matchLine.IdMatch) == null)
				{
					MatchesLineVM.Add(matchLineVM);
				}
				MatchLineStat matchLineStat = tuple.Item2.SingleOrDefault((MatchLineStat ml) => ml.NameProvider == matchLine.NameProvider && ml.IdMatch == matchLine.IdMatch);
				if (matchLineStat == null)
				{
					continue;
				}
				matchLineVM.Score = matchLineStat.Score;
				CouponStat couponStat = matchLineStat.CouponsStats.SingleOrDefault((CouponStat cs) => cs.CouponLine.IssueType == IssueType.VictoryTeam_1);
				CouponStat couponStat2 = matchLineStat.CouponsStats.SingleOrDefault((CouponStat cs) => cs.CouponLine.IssueType == IssueType.VictoryTeam_2);
				if (couponStat == null || couponStat2 == null)
				{
					continue;
				}
				CouponStat couponStat3 = ((couponStat.CouponLine.PayoutRatio < couponStat2.CouponLine.PayoutRatio) ? couponStat : couponStat2);
				matchLineVM.MaxDeviationPercent = couponStat3.MaxDeviationCoef;
				matchLineVM.Volume = couponStat3.CouponLine.HistoryCoefs.Count;
				if (couponStat3.CouponLine.HistoryCoefs.Count > 1)
				{
					double payoutRatio = couponStat3.CouponLine.HistoryCoefs.Last((CouponLine.CoefFrame f) => f.PayoutRatio > 1.0).PayoutRatio;
					matchLineVM.DeviationPercent = Math.Round((payoutRatio / couponStat3.CouponLine.HistoryCoefs.First().PayoutRatio - 1.0) * 100.0);
				}
			}
			SportsChoosed.ForEach(delegate(SportChoose sp)
			{
				sp.TotalSelected = 0;
			});
			foreach (SportChoose item in SportsChoosed)
			{
				if (tuple.Item3.TryGetValue(item.Sport.IdSport, out var value))
				{
					item.TotalSelected = value;
				}
			}
		}
		UnblockUI();
	}

	private async Task DownloadFavorites(List<MatchDto> matchesDto)
	{
		BlockUI();
		MatchesLineVM.Clear();
		(List<MatchLine>, List<MatchLineStat>) tuple = await WatsonDataServerLineClient.GetMatches(matchesDto);
		if (tuple.Item1 != null)
		{
			foreach (MatchLine matchLine in tuple.Item1)
			{
				MatchLineVM matchLineVM = new MatchLineVM
				{
					MatchLine = matchLine
				};
				matchLineVM.IsFavorite = true;
				AddFavoriteMatch(matchLineVM);
				MatchLineStat matchLineStat = tuple.Item2.SingleOrDefault((MatchLineStat ml) => ml.NameProvider == matchLine.NameProvider && ml.IdMatch == matchLine.IdMatch);
				if (matchLineStat == null)
				{
					continue;
				}
				matchLineVM.Score = matchLineStat.Score;
				CouponStat couponStat = matchLineStat.CouponsStats.SingleOrDefault((CouponStat cs) => cs.CouponLine.IssueType == IssueType.VictoryTeam_1);
				CouponStat couponStat2 = matchLineStat.CouponsStats.SingleOrDefault((CouponStat cs) => cs.CouponLine.IssueType == IssueType.VictoryTeam_2);
				if (couponStat != null && couponStat2 != null)
				{
					CouponStat couponStat3 = ((couponStat.CouponLine.PayoutRatio < couponStat2.CouponLine.PayoutRatio) ? couponStat : couponStat2);
					matchLineVM.MaxDeviationPercent = couponStat3.MaxDeviationCoef;
					matchLineVM.Volume = couponStat3.CouponLine.HistoryCoefs.Count;
					if (couponStat3.CouponLine.HistoryCoefs.Count > 1)
					{
						matchLineVM.DeviationPercent = Math.Round((couponStat3.CouponLine.PayoutRatio / couponStat3.CouponLine.HistoryCoefs.First().PayoutRatio - 1.0) * 100.0);
					}
				}
			}
		}
		UnblockUI();
	}

	private async Task UpdateDataMatchLine()
	{
		if (MatchLineVMSelected == null)
		{
			new WindowInfo("Не выбран матч");
			return;
		}
		BlockUI();
		await WatsonDataServerLineClient.GetMatch(MatchLineVMSelected.MatchLine.NameProvider, MatchLineVMSelected.MatchLine.IdSport, MatchLineVMSelected.MatchLine.IdMatch, MatchLineVMSelected.MatchLine.StartTime.Date);
		UnblockUI();
	}

	private void AddFavoriteMatch(MatchLineVM matchLineVM)
	{
		FavoriteMatches.Add(matchLineVM);
		FavoriteTotal = FavoriteMatches.Count;
	}

	private void RemoveFavoriteMatch(MatchLineVM matchLineVM)
	{
		try
		{
			FavoriteMatches.Remove(matchLineVM);
		}
		catch
		{
		}
		FavoriteTotal = FavoriteMatches.Count;
	}

	private string ConvertLabelName(MatchLine matchLine, CouponLine couponLine)
	{
		if (couponLine.IssueType == IssueType.VictoryTeam_1)
		{
			return matchLine.Team1;
		}
		if (couponLine.IssueType == IssueType.VictoryTeam_2)
		{
			return matchLine.Team2;
		}
		return IssueTypeBL.ConvertUserSrtIssueTypeCoupon(couponLine.IssueType.ToString(), couponLine.IssueValue);
	}

	private SKColor ConvertColorCoupon(BookmakerLibrary.Coupon coupon)
	{
		if (coupon.IssueType == IssueType.VictoryTeam_1)
		{
			return SKColors.Blue;
		}
		if (coupon.IssueType == IssueType.Draw)
		{
			return SKColors.Orange;
		}
		if (coupon.IssueType == IssueType.VictoryTeam_2)
		{
			return SKColors.IndianRed;
		}
		if (coupon.IssueType == IssueType.TotalOver)
		{
			return SKColors.Aqua;
		}
		if (coupon.IssueType == IssueType.TotalUnder)
		{
			return SKColors.IndianRed;
		}
		return SKColors.Black;
	}

	public void Load()
	{
		if (File.Exists(DCSETTINGSPATH))
		{
			try
			{
				List<MatchDto> matchesDto = JsonConvert.DeserializeObject<List<MatchDto>>(CommonStaticFunction.DecompressString(File.ReadAllText(DCSETTINGSPATH)));
				DownloadFavorites(matchesDto);
			}
			catch (Exception)
			{
			}
		}
	}

	public void Save()
	{
		List<MatchDto> list = new List<MatchDto>();
		foreach (MatchLine item in FavoriteMatches.Select((MatchLineVM f) => f.MatchLine))
		{
			list.Add(new MatchDto
			{
				NameProvider = item.NameProvider,
				IdSport = item.IdSport,
				IdMatch = item.IdMatch,
				Date = item.StartTime.Date
			});
		}
		string text = JsonConvert.SerializeObject(list);
		File.WriteAllText(Environment.CurrentDirectory + "\\DCSettings.gb", CommonStaticFunction.CompressString(text));
	}

	public void OnSearchTextChanged()
	{
		if (SearchText.Trim().Length < 2)
		{
			ViewModelMainWindow.ShowDialog("Для поиска должно быть не менее 2-х символов.");
		}
		else
		{
			CVSMatchesProviderRefresh();
		}
	}
}
