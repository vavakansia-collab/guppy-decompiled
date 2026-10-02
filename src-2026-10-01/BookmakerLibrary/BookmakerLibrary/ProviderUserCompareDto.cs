using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class ProviderUserCompareDto
{
	public string Name;

	public DateTime LastTimeUpdate;

	public List<MatchUserCompareDto> MatchesDto = new List<MatchUserCompareDto>();

	public int TotalCoupons;

	public ProviderUserCompareDto()
	{
	}

	public ProviderUserCompareDto(string nameProvider)
	{
		Name = nameProvider;
	}
}
