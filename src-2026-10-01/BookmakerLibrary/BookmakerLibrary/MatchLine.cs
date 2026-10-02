using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ProtoBuf;

namespace BookmakerLibrary;

public class MatchLine : INotifyPropertyChanged
{
	private int idSport;

	[IgnoreDataMember]
	private Sport sport = null;

	private DateTime lastTimeUpdate = DateTime.Now;

	public string NameProvider { get; set; }

	[ProtoIgnore]
	[IgnoreDataMember]
	public string Name => NameProvider + ". " + ((!NameChamp.Contains(Sport.FullName) && !NameChamp.Contains(Sport.ShortName)) ? (Sport.FullName + ". ") : "") + NameChamp + " " + Team1 + " - " + Team2;

	public List<CouponLine> CouponsLine { get; set; } = new List<CouponLine>();

	public int IdSport
	{
		get
		{
			return idSport;
		}
		set
		{
			idSport = value;
			sport = SportBL.GetSportByID(idSport);
		}
	}

	[IgnoreDataMember]
	public Sport Sport => sport;

	public long IdMatch { get; set; }

	public int IdChamp { get; set; }

	public string NameChamp { get; set; } = "";

	public string Team1 { get; set; }

	public string Team2 { get; set; }

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public DateTime StartTime { get; set; } = DateTime.MinValue;

	public DateTime FinishTime { get; set; } = DateTime.MinValue;

	public DateTime LastTimeUpdate
	{
		get
		{
			return lastTimeUpdate;
		}
		set
		{
			lastTimeUpdate = value;
			RaisePropertyChanged("LastTimeUpdate");
		}
	}

	public string Url { get; set; }

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public MatchLine(string initNameProvider)
	{
		NameProvider = initNameProvider;
	}

	public bool IsEquals(MatchLine _match)
	{
		return NameProvider == _match.NameProvider && IdMatch == _match.IdMatch;
	}

	public override string ToString()
	{
		return StartTime.ToString() + " " + Name;
	}

	public string GetParameter(string nameParameter)
	{
		string result = null;
		foreach (KeyValuePair<string, string> parameter in Parameters)
		{
			if (parameter.Key == nameParameter)
			{
				result = parameter.Value;
			}
		}
		return result;
	}

	public void SetParameter(string nameParameter, string valueParameter)
	{
		Parameters.Remove(nameParameter);
		Parameters.Add(nameParameter, valueParameter);
	}

	public void Overturn()
	{
		string team = Team1;
		Team1 = Team2;
		Team2 = team;
		foreach (CouponLine item in CouponsLine)
		{
			item.Overturn();
		}
	}

	public bool IsChanged(MatchLine newMatchLine)
	{
		if (CouponsLine.Count != newMatchLine.CouponsLine.Count)
		{
			return true;
		}
		foreach (CouponLine couponCompare in newMatchLine.CouponsLine)
		{
			Coupon coupon = CouponsLine.SingleOrDefault((CouponLine c) => c.IsEquals(couponCompare));
			if (coupon == null || coupon.PayoutRatio != couponCompare.PayoutRatio)
			{
				return true;
			}
		}
		foreach (CouponLine coupon2 in CouponsLine)
		{
			Coupon coupon3 = newMatchLine.CouponsLine.SingleOrDefault((CouponLine c) => c.IsEquals(coupon2));
			if (coupon3 == null)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateCoupons(MatchLine newMatchLine)
	{
		foreach (CouponLine couponCompare in newMatchLine.CouponsLine)
		{
			CouponLine couponLine = CouponsLine.SingleOrDefault((CouponLine c) => c.IsEquals(couponCompare));
			if (couponLine == null)
			{
				couponLine = Coupon.GetCopyCoupon(couponCompare, isOverturn: false);
				CouponsLine.Add(couponLine);
			}
			else
			{
				if (couponLine.PayoutRatio == couponCompare.PayoutRatio)
				{
					continue;
				}
				foreach (CouponLine.CoefFrame historyCoef in couponCompare.HistoryCoefs)
				{
					if (couponLine.HistoryCoefs.Last().Timestamp < historyCoef.Timestamp)
					{
						CouponLine.CoefFrame coefFrame = new CouponLine.CoefFrame();
						coefFrame.Timestamp = historyCoef.Timestamp;
						coefFrame.PayoutRatio = historyCoef.PayoutRatio;
						couponLine.HistoryCoefs.Add(coefFrame);
					}
				}
			}
		}
		foreach (CouponLine couponLine2 in CouponsLine)
		{
			CouponLine couponLine3 = newMatchLine.CouponsLine.SingleOrDefault((CouponLine c) => c.IsEquals(couponLine2));
			if (couponLine3 == null)
			{
				couponLine2.PayoutRatio = -1.0;
				CouponLine.CoefFrame coefFrame2 = new CouponLine.CoefFrame();
				coefFrame2.Timestamp = DateTime.Now;
				coefFrame2.PayoutRatio = -1.0;
				couponLine2.HistoryCoefs.Add(coefFrame2);
			}
		}
	}
}
