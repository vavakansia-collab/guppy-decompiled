using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class Coupon
{
	public bool Availability { get; set; } = true;

	public DateTime LastTimeUpdate { get; set; } = DateTime.Now;

	public int Period { get; set; }

	public int GameTennis { get; set; } = 0;

	public IssueType IssueType { get; set; } = IssueType._empty;

	public double IssueValue { get; set; } = 0.0;

	public double PayoutRatio { get; set; } = 0.0;

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

	public string Comment { get; set; } = "";

	public Coupon Clone()
	{
		Coupon coupon = (Coupon)MemberwiseClone();
		coupon.IssueType = IssueType;
		return coupon;
	}

	public override string ToString()
	{
		return (Availability ? "" : "(н)") + " " + ((Period > 0) ? ("Период " + Period + ". ") : "") + IssueTypeBL.ConvertUserSrtIssueTypeCoupon(IssueType.ToString(), IssueValue, GameTennis) + ". коэф." + PayoutRatio.ToString().Replace(",", ".") + ", обн." + LastTimeUpdate.ToLongTimeString();
	}

	public string ToShortString()
	{
		return (Availability ? "" : "(н)") + " " + ((Period > 0) ? ("Период " + Period + ". ") : "") + IssueTypeBL.ConvertUserSrtIssueTypeCoupon(IssueType.ToString(), IssueValue, GameTennis) + ". коэф." + PayoutRatio.ToString().Replace(",", ".");
	}

	public void Overturn()
	{
		IssueType = IssueTypeBL.GetOverturnTypeCoupon(IssueType);
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

	public bool IsEquals(Coupon coupon, bool withoutCoef = true)
	{
		bool result = false;
		if (IssueType == coupon.IssueType && IssueValue == coupon.IssueValue && Period == coupon.Period && GameTennis == coupon.GameTennis && (withoutCoef || PayoutRatio == coupon.PayoutRatio))
		{
			result = true;
		}
		return result;
	}

	public static CouponLine GetCopyCoupon(Coupon coupon, bool isOverturn)
	{
		CouponLine couponLine = new CouponLine
		{
			LastTimeUpdate = coupon.LastTimeUpdate,
			Period = coupon.Period,
			GameTennis = coupon.GameTennis,
			IssueType = coupon.IssueType,
			IssueValue = coupon.IssueValue,
			PayoutRatio = coupon.PayoutRatio,
			Comment = coupon.Comment
		};
		foreach (KeyValuePair<string, string> parameter in coupon.Parameters)
		{
			couponLine.Parameters.Add(parameter.Key, parameter.Value);
		}
		if (isOverturn)
		{
			couponLine.Overturn();
		}
		return couponLine;
	}
}
