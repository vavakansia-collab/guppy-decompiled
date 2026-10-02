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
					foreach (CompareMatchParameter cmp in compareMatch.MatchesCMP)
					{
						if (cmp.NameProvider == nameProvider && cmp.IdMatch == idMatch)
						{
							return cmp;
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
				foreach (CompareMatchParameter cmp in compareMatch.MatchesCMP)
				{
					if (cmp.NameProvider == nameProvider && cmp.IdMatch == idMatch)
					{
						return cmp;
					}
				}
			}
			return null;
		}
	}

	public static List<CompareMatch> MergeCompareMatches(List<List<CompareMatch>> listCompareMatches)
	{
		List<CompareMatch> listCm = new List<CompareMatch>();
		lock (thislock)
		{
			foreach (List<CompareMatch> listCompareMatch in listCompareMatches)
			{
				foreach (CompareMatch compareMatch in listCompareMatch)
				{
					CompareMatch existCompareMatch = listCm.SingleOrDefault((CompareMatch cm) => cm.InnerId == compareMatch.InnerId);
					if (existCompareMatch == null)
					{
						listCm.Add(compareMatch);
						continue;
					}
					lock (compareMatch.thisLock)
					{
						foreach (CompareMatchParameter cmp in compareMatch.MatchesCMP)
						{
							lock (existCompareMatch.thisLock)
							{
								if (existCompareMatch.MatchesCMP.SingleOrDefault((CompareMatchParameter c) => c.NameProvider == cmp.NameProvider) == null)
								{
									existCompareMatch.MatchesCMP.Add(cmp);
								}
							}
						}
					}
				}
			}
			return listCm;
		}
	}
}
