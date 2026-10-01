using BookmakerLibrary;

namespace SnapshotsLibrary;

public class CouponStat
{
	public string NameProvider { get; set; }

	public long IdMatch { get; set; }

	public CouponLine CouponLine { get; set; }

	public double InitialCoef { get; set; }

	public double MinCoef { get; set; }

	public double MaxCoef { get; set; }

	public double DeviationCoef { get; set; }

	public double MaxDeviationCoef { get; set; }

	public CouponStat(string initNameProvider, long initIdMatch, CouponLine initCoupon)
	{
		NameProvider = initNameProvider;
		IdMatch = initIdMatch;
		CouponLine = initCoupon;
	}
}
