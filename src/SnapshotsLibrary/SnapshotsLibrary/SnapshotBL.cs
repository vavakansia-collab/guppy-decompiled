using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibrary;

namespace SnapshotsLibrary;

public static class SnapshotBL
{
	public const bool IS_LOG = true;

	public static Snapshot MergeSnapshots(IEnumerable<Snapshot> snapshots)
	{
		if (snapshots == null)
		{
			return null;
		}
		Log($"Start merging {snapshots.Count()} snapshots");
		DateTime now = DateTime.Now;
		Snapshot snapshot = new Snapshot();
		snapshot.StartTime = DateTime.MaxValue;
		Log("Appending data...");
		foreach (Snapshot snapshot2 in snapshots)
		{
			if (snapshot2.StartTime < snapshot.StartTime)
			{
				snapshot.StartTime = snapshot2.StartTime;
			}
			foreach (Bookmaker currentBookmaker in snapshot2.Bookmakers)
			{
				Bookmaker bookmaker = snapshot.Bookmakers.FirstOrDefault((Bookmaker x) => x.Name == currentBookmaker.Name);
				if (bookmaker == null)
				{
					bookmaker = new Bookmaker();
					bookmaker.Name = currentBookmaker.Name;
					snapshot.Bookmakers.Add(bookmaker);
				}
				foreach (Match currentMatch in currentBookmaker.Matches)
				{
					Match match = bookmaker.Matches.FirstOrDefault((Match x) => x.Id == currentMatch.Id);
					if (match == null)
					{
						match = new Match();
						match.TimeCreate = currentMatch.TimeCreate;
						match.FinishTime = currentMatch.FinishTime;
						match.Id = currentMatch.Id;
						match.Championship = currentMatch.Championship;
						match.IdSport = currentMatch.IdSport;
						match.Team1 = currentMatch.Team1;
						match.Team2 = currentMatch.Team2;
						match.Score = currentMatch.Score;
						bookmaker.Matches.Add(match);
					}
					match.ScoreHistory.AddRange(currentMatch.ScoreHistory);
					foreach (Coupon currentCoupon in currentMatch.Coupons)
					{
						Coupon coupon = match.Coupons.FirstOrDefault((Coupon x) => x.IssueType == currentCoupon.IssueType && x.IssueValue == currentCoupon.IssueValue && x.Period == currentCoupon.Period && x.GameTennis == currentCoupon.GameTennis);
						if (coupon == null)
						{
							coupon = new Coupon();
							coupon.IssueType = currentCoupon.IssueType;
							coupon.IssueValue = currentCoupon.IssueValue;
							coupon.Period = currentCoupon.Period;
							match.Coupons.Add(coupon);
						}
						coupon.CoefficientHistory.AddRange(currentCoupon.CoefficientHistory);
					}
				}
			}
		}
		Log("Sorting timeline...");
		foreach (Bookmaker bookmaker2 in snapshot.Bookmakers)
		{
			foreach (Match match2 in bookmaker2.Matches)
			{
				match2.ScoreHistory.OrderBy((Match.ScoreFrame x) => x.Timestamp);
				foreach (Coupon coupon2 in match2.Coupons)
				{
					coupon2.CoefficientHistory.OrderBy((Coupon.CoefficientFrame x) => x.Timestamp);
				}
			}
		}
		int num = snapshot.Bookmakers.SelectMany((Bookmaker x) => x.Matches.SelectMany((Match y) => y.ScoreHistory)).Count();
		int num2 = snapshot.Bookmakers.SelectMany((Bookmaker x) => x.Matches.SelectMany((Match y) => y.Coupons.SelectMany((Coupon z) => z.CoefficientHistory))).Count();
		Log(string.Format("Snapshots merged in {0} s [{1}|{2}]", (DateTime.Now - now).TotalSeconds.ToString("0.00"), num, num2));
		return snapshot;
	}

	public static void Log(string msg)
	{
		Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " [SnapshotProvider] " + msg);
	}

	public static (bool, double) IsExistCouponToDataPoint(Match match, Coupon coupon, DateTime dataPoint)
	{
		Coupon.CoefficientFrame coefficientFrame = coupon.CoefficientHistory.SingleOrDefault((Coupon.CoefficientFrame ch) => ch.Timestamp == dataPoint);
		if (coefficientFrame != null)
		{
			return (true, coefficientFrame.Value);
		}
		coefficientFrame = coupon.CoefficientHistory.OrderBy((Coupon.CoefficientFrame ch) => ch.Timestamp).LastOrDefault((Coupon.CoefficientFrame ch) => ch.Timestamp < dataPoint);
		if (coefficientFrame == null)
		{
			return (false, -1.0);
		}
		return (true, coefficientFrame.Value);
	}

	public static Match GetMatch(Snapshot snapshot, string _provider, long idMatch)
	{
		Match result = null;
		Bookmaker bookmaker = snapshot.Bookmakers.SingleOrDefault((Bookmaker pr) => pr.Name == _provider);
		if (bookmaker != null)
		{
			result = bookmaker.Matches.SingleOrDefault((Match m) => m.Id == idMatch);
		}
		return result;
	}
}
