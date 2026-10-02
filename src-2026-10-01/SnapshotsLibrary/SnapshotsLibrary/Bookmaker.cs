using System.Collections.Generic;

namespace SnapshotsLibrary;

public class Bookmaker
{
	public string Name { get; set; }

	public List<Match> Matches { get; set; } = new List<Match>();

	public override string ToString()
	{
		return $"{Name} [{Matches.Count}]";
	}
}
