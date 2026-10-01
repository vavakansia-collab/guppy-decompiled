using System.Collections.Generic;

namespace BookmakerLibrary;

public class RandomMatchParameters
{
	public string NameProvider;

	public List<int> SportsIds = new List<int>();

	public List<IssueType> IssueTypes = new List<IssueType>();

	public double MinCoef;

	public double MaxCoef;
}
