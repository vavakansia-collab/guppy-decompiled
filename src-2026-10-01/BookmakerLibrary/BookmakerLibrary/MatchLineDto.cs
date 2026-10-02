using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class MatchLineDto
{
	public string NameProvider;

	public DateTime TimeUpload = DateTime.MinValue;

	public List<MatchLine> AddMatches = new List<MatchLine>();

	public List<(long, CouponLineDto, List<CouponLine.CoefFrame>)> ChangesCouponFrame = new List<(long, CouponLineDto, List<CouponLine.CoefFrame>)>();

	public MatchLineDto(string initNameProvider)
	{
		NameProvider = initNameProvider;
	}
}
