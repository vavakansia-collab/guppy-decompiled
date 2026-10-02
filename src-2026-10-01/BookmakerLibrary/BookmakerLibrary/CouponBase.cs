using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class CouponBase
{
	public bool Availability = true;

	public DateTime LastTimeUpdate = DateTime.Now;

	public int Period;

	public int GameTennis = 0;

	public IssueType IssueType = IssueType._empty;

	public double IssueValue;

	public double Coef;

	public Dictionary<string, string> Parameters = new Dictionary<string, string>();

	public string Comment = "";

	public override string ToString()
	{
		return ((Period > 0) ? ("Период " + Period + ". ") : "") + IssueTypeBL.ConvertUserSrtIssueTypeCoupon(IssueType.ToString(), IssueValue, GameTennis) + ". коэф." + Coef.ToString().Replace(",", ".") + ", обн." + LastTimeUpdate.ToLongTimeString();
	}

	public string ToShortString()
	{
		return ((Period > 0) ? ("Период " + Period + ". ") : "") + IssueTypeBL.ConvertUserSrtIssueTypeCoupon(IssueType.ToString(), IssueValue, GameTennis) + ". коэф." + Coef.ToString().Replace(",", ".");
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

	public bool IsEquals(CouponBase coupon, bool withoutCoef = true)
	{
		bool result = false;
		if (IssueType == coupon.IssueType && IssueValue == coupon.IssueValue && Period == coupon.Period && GameTennis == coupon.GameTennis && (withoutCoef || Coef == coupon.Coef))
		{
			result = true;
		}
		return result;
	}

	public static CouponLine GetCopyCoupon(CouponLine couponLine, bool isOverturn)
	{
		CouponLine couponLine2 = new CouponLine
		{
			LastTimeUpdate = couponLine.LastTimeUpdate,
			Period = couponLine.Period,
			GameTennis = couponLine.GameTennis,
			IssueType = couponLine.IssueType,
			IssueValue = couponLine.IssueValue,
			PayoutRatio = couponLine.PayoutRatio,
			Comment = couponLine.Comment
		};
		foreach (KeyValuePair<string, string> parameter in couponLine.Parameters)
		{
			couponLine2.Parameters.Add(parameter.Key, parameter.Value);
		}
		if (isOverturn)
		{
			couponLine2.Overturn();
		}
		return couponLine2;
	}
}
