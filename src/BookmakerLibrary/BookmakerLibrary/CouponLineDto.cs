using System.Collections.Generic;

namespace BookmakerLibrary;

public class CouponLineDto
{
	public bool Availability = true;

	public int Period;

	public IssueType IssueType;

	public double IssueValue;

	public double Coef;

	public int GameTennis = 0;

	public Dictionary<string, string> Parameters = new Dictionary<string, string>();
}
