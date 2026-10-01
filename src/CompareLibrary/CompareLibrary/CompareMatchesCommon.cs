using System.Collections.Generic;
using System.Linq;

namespace CompareLibrary;

public static class CompareMatchesCommon
{
	public static object thislock { get; set; } = new object();

	public static CompareMatch GetCompareMatch(List<CompareMatch> compareMatches, int innerId)
	{
		lock (thislock)
		{
			return compareMatches.SingleOrDefault((CompareMatch c) => c.InnerId == innerId);
		}
	}

	public static CompareMatchParameter GetCMPMatch(List<CompareMatch> compareMatches, string nameProvider, long idMatch)
	{
		lock (thislock)
		{
			foreach (CompareMatch compareMatch in compareMatches)
			{
				lock (compareMatch.thisLock)
				{
					foreach (CompareMatchParameter item in compareMatch.MatchesCMP)
					{
						if (item.NameProvider == nameProvider && item.IdMatch == idMatch)
						{
							return item;
						}
					}
				}
			}
			return null;
		}
	}

	public static CompareMatchParameter GetCMPMatch(CompareMatch compareMatch, string nameProvider, long idMatch)
	{
		lock (thislock)
		{
			lock (compareMatch.thisLock)
			{
				foreach (CompareMatchParameter item in compareMatch.MatchesCMP)
				{
					if (item.NameProvider == nameProvider && item.IdMatch == idMatch)
					{
						return item;
					}
				}
			}
			return null;
		}
	}

	public static List<CompareMatch> MergeCompareMatches(List<List<CompareMatch>> listCompareMatches)
	{
		List<CompareMatch> list = new List<CompareMatch>();
		lock (thislock)
		{
			foreach (List<CompareMatch> listCompareMatch in listCompareMatches)
			{
				foreach (CompareMatch compareMatch in listCompareMatch)
				{
					CompareMatch compareMatch2 = list.SingleOrDefault((CompareMatch cm) => cm.InnerId == compareMatch.InnerId);
					if (compareMatch2 == null)
					{
						list.Add(compareMatch);
						continue;
					}
					lock (compareMatch.thisLock)
					{
						foreach (CompareMatchParameter cmp in compareMatch.MatchesCMP)
						{
							lock (compareMatch2.thisLock)
							{
								if (compareMatch2.MatchesCMP.SingleOrDefault((CompareMatchParameter c) => c.NameProvider == cmp.NameProvider) == null)
								{
									compareMatch2.MatchesCMP.Add(cmp);
								}
							}
						}
					}
				}
			}
			return list;
		}
	}
}
