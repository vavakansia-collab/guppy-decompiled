using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using BookmakerLibrary;
using CommonLibrary;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AccountsManager.DropCoef.ViewModels;

public class VMMatchLine : INotifyPropertyChanged
{
	private string windowName;

	private MatchLineVM matchLineVMSelected;

	private RelayCommand copyUrlCommand;

	private RelayCommand hyperlinkMatchUrlCommand;

	private ISeries[] _series;

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

	public MatchLineVM MatchLineVMSelected
	{
		get
		{
			return matchLineVMSelected;
		}
		set
		{
			matchLineVMSelected = value;
			RaisePropertyChanged("MatchLineVMSelected");
		}
	}

	public RelayCommand CopyUrlCommand => copyUrlCommand ?? (copyUrlCommand = new RelayCommand(delegate
	{
		Clipboard.SetText(MatchLineVMSelected.MatchLine.Url);
	}));

	public RelayCommand HyperlinkMatchUrlCommand => hyperlinkMatchUrlCommand ?? (hyperlinkMatchUrlCommand = new RelayCommand(delegate
	{
		if (MatchLineVMSelected != null && !string.IsNullOrEmpty(MatchLineVMSelected.MatchLine.Url))
		{
			Process.Start(MatchLineVMSelected.MatchLine.Url);
		}
	}));

	public SolidColorPaint TextPaint { get; set; } = new SolidColorPaint(SKColors.Orange);

	public Axis[] XAxes { get; set; }

	public Axis[] YAxes { get; set; }

	public ISeries[] Series
	{
		get
		{
			return _series;
		}
		set
		{
			_series = value;
			RaisePropertyChanged("Series");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public VMMatchLine(MatchLineVM initMatchLineVMSelected)
	{
		MatchLineVMSelected = initMatchLineVMSelected;
		WindowName = MatchLineVMSelected.MatchLine.Name;
		XAxes = new Axis[1]
		{
			new Axis
			{
				Name = "Время",
				Labeler = (double value) => new DateTime((long)Math.Abs(value)).ToString("HH:mm:ss") + Environment.NewLine + new DateTime((long)Math.Abs(value)).ToShortDateString()
			}
		};
		YAxes = new Axis[1]
		{
			new Axis
			{
				Name = "Коэффициент",
				MinLimit = 1.0
			}
		};
	}

	public void UpdateChars()
	{
		WindowName = MatchLineVMSelected.MatchLine.Name;
		List<CouponLine> list = MatchLineVMSelected.MatchLine.CouponsLine.Where((CouponLine c) => c.IssueType == IssueType.VictoryTeam_1 || c.IssueType == IssueType.VictoryTeam_2 || c.IssueType == IssueType.Draw).ToList();
		Series = new ISeries[list.Count];
		for (int num = 0; num < list.Count; num++)
		{
			CouponLine couponLine = list[num];
			ObservableCollection<DateTimePoint> observableCollection = new ObservableCollection<DateTimePoint>();
			List<CouponLine.CoefFrame> list2 = couponLine.HistoryCoefs.Where((CouponLine.CoefFrame h) => h.PayoutRatio > 1.0).ToList();
			foreach (CouponLine.CoefFrame item in list2)
			{
				observableCollection.Add(new DateTimePoint(item.Timestamp, item.PayoutRatio));
			}
			if (list2.Count > 0)
			{
				observableCollection.Add(new DateTimePoint(DateTime.Now, list2.Last().PayoutRatio));
			}
			Series[num] = new LineSeries<DateTimePoint>
			{
				Name = ConvertLabelName(MatchLineVMSelected.MatchLine, couponLine),
				Mapping = (DateTimePoint point, int index) => new Coordinate(point.DateTime.Ticks, point.Value.Value),
				Values = observableCollection,
				Stroke = new SolidColorPaint(ConvertColorCoupon(couponLine), 2f),
				GeometrySize = 8.0,
				GeometryFill = new SolidColorPaint(ConvertColorCoupon(couponLine)),
				GeometryStroke = new SolidColorPaint(ConvertColorCoupon(couponLine)),
				Fill = null
			};
		}
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

	private SKColor ConvertColorCoupon(Coupon coupon)
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
}
