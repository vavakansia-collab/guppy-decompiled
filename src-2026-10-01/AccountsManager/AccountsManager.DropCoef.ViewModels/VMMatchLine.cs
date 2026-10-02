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
		List<CouponLine> couponLines = MatchLineVMSelected.MatchLine.CouponsLine.Where((CouponLine c) => c.IssueType == IssueType.VictoryTeam_1 || c.IssueType == IssueType.VictoryTeam_2 || c.IssueType == IssueType.Draw).ToList();
		Series = new ISeries[couponLines.Count];
		for (int i = 0; i < couponLines.Count; i++)
		{
			CouponLine couponLine = couponLines[i];
			ObservableCollection<DateTimePoint> coefPoints = new ObservableCollection<DateTimePoint>();
			List<CouponLine.CoefFrame> frames = couponLine.HistoryCoefs.Where((CouponLine.CoefFrame h) => h.PayoutRatio > 1.0).ToList();
			foreach (CouponLine.CoefFrame historyKoef in frames)
			{
				coefPoints.Add(new DateTimePoint(historyKoef.Timestamp, historyKoef.PayoutRatio));
			}
			if (frames.Count > 0)
			{
				coefPoints.Add(new DateTimePoint(DateTime.Now, frames.Last().PayoutRatio));
			}
			Series[i] = new LineSeries<DateTimePoint>
			{
				Name = ConvertLabelName(MatchLineVMSelected.MatchLine, couponLine),
				Mapping = (DateTimePoint point, int index) => new Coordinate(point.DateTime.Ticks, point.Value.Value),
				Values = coefPoints,
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
