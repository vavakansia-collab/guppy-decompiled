using System;
using System.Collections.Generic;
using BookmakerLibrary;

namespace SnapshotsLibrary;

public class Coupon
{
	public class CoefficientFrame
	{
		public DateTime Timestamp { get; set; }

		public double Value { get; set; }

		public override string ToString()
		{
			return $"{Timestamp} {Value}";
		}
	}

	public bool Availability { get; set; }

	public IssueType IssueType { get; set; }

	public double IssueValue { get; set; }

	public int Period { get; set; }

	public int GameTennis { get; set; }

	public List<CoefficientFrame> CoefficientHistory { get; set; } = new List<CoefficientFrame>();

	public override string ToString()
	{
		return $"{IssueType}";
	}
}
