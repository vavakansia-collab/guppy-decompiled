using System.Collections.Generic;
using BookmakerLibrary;

namespace StrategyLibrary;

public class SnapshotStrategis
{
	public long Number { get; set; }

	public List<Poslegol> Poslegols { get; set; } = new List<Poslegol>();

	public List<Value> Values { get; set; } = new List<Value>();

	public List<ValuePlus> ValuesPlus { get; set; } = new List<ValuePlus>();

	public List<ValueScore> ValuesScore { get; set; } = new List<ValueScore>();

	public List<Fork> Forks { get; set; } = new List<Fork>();

	public double tsUpdate { get; set; }

	public bool ExistPoslegol(Coupon coupon, Match match)
	{
		foreach (Poslegol poslegol in Poslegols)
		{
			if (poslegol.Match1NameProvider == match.NameProvider && poslegol.Coupon.GameTennis == coupon.GameTennis && poslegol.Coupon.IssueType == coupon.IssueType && poslegol.Coupon.IssueValue == coupon.IssueValue && poslegol.Coupon.Period == coupon.Period)
			{
				return true;
			}
		}
		return false;
	}
}
