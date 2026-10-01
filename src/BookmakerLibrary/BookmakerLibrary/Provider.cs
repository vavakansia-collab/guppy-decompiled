using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class Provider
{
	public object lockProvider = new object();

	public string Name { get; set; }

	public int NumberUpdate { get; set; } = 0;

	public List<Match> Matches { get; set; } = new List<Match>();

	public List<MatchLine> MatchesLine { get; set; } = new List<MatchLine>();

	public List<MatchLine> ArhiveMatches { get; set; } = new List<MatchLine>();

	public double IntervalUpdate { get; set; } = 5000.0;

	public DateTime LastTimeUpdate { get; set; } = DateTime.MinValue;

	public Provider()
	{
	}

	public Provider(string initName)
	{
		Name = initName;
	}

	public bool CheckTimeUpdate()
	{
		return DateTime.Now.Subtract(LastTimeUpdate).TotalMilliseconds < IntervalUpdate || Name.Contains("Scout");
	}

	public override string ToString()
	{
		return Name;
	}
}
