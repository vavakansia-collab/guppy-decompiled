using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class CouponLine : Coupon
{
	public class CoefFrame
	{
		public DateTime Timestamp;

		public double PayoutRatio;

		public override string ToString()
		{
			return $"{Timestamp} {PayoutRatio}";
		}
	}

	public List<CoefFrame> HistoryCoefs = new List<CoefFrame>();

	public static CouponLine GetCopyCouponLine(CouponLine couponLine, bool isOverturn)
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
