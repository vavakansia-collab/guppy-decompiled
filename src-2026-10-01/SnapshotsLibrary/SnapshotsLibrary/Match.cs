using System;
using System.Collections.Generic;
using BookmakerLibrary;

namespace SnapshotsLibrary;

public class Match
{
	public class ScoreFrame
	{
		public DateTime Timestamp { get; set; }

		public string Value { get; set; }
	}

	public long Id { get; set; }

	public DateTime TimeCreate { get; set; }

	public DateTime FinishTime { get; set; }

	public int IdSport { get; set; }

	public string Championship { get; set; }

	public string Team1 { get; set; }

	public string Team2 { get; set; }

	public List<Coupon> Coupons { get; set; } = new List<Coupon>();

	public List<ScoreFrame> ScoreHistory { get; set; } = new List<ScoreFrame>();

	public string Score { get; set; }

	public override string ToString()
	{
		return SportBL.GetSportByID(IdSport).FullName + ". " + Team1 + " vs " + Team2;
	}
}
