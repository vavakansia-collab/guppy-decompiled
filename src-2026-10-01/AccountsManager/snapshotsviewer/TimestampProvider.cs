using System.Collections.Generic;
using BookmakerLibrary;

namespace SnapshotsViewer;

public class TimestampProvider
{
	public string NameProvider { get; set; }

	public List<Coupon> Coupons { get; set; } = new List<Coupon>();

	public string Score { get; set; }
}
