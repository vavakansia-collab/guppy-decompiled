using System.Collections.Generic;

namespace SnapshotsLibrary;

public class MatchLineStat
{
	public string NameProvider;

	public long IdMatch;

	public string Score = "-";

	public List<CouponStat> CouponsStats = new List<CouponStat>();

	public MatchLineStat(string initNameProvider, long initIdMatch)
	{
		NameProvider = initNameProvider;
		IdMatch = initIdMatch;
	}
}
