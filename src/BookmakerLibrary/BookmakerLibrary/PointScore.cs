using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class PointScore
{
	public bool showMessageScoreDown = false;

	public bool IsAutoPasteScore = false;

	public static PointSubScore pointSubScoreDefault;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public DateTime TimeServerLoad { get; set; } = DateTime.MinValue;

	public int Period { get; set; } = 0;

	public int ValuePointTeam1 { get; set; } = 0;

	public int ValuePointTeam2 { get; set; } = 0;

	public int NumberGetTeam { get; set; } = 0;

	public bool IsDefault { get; set; } = false;

	public bool IsScoreDown { get; set; } = false;

	public List<PointSubScore> PointsSubScore { get; set; } = new List<PointSubScore>();

	public PointScore()
	{
		pointSubScoreDefault = new PointSubScore
		{
			IsDefault = true
		};
	}

	public PointSubScore GetLastPointSubScore(int deviation = 0)
	{
		PointSubScore result = pointSubScoreDefault;
		int num = PointsSubScore.Count;
		foreach (PointSubScore item in PointsSubScore)
		{
			num--;
			if (num == deviation)
			{
				result = item;
			}
		}
		return result;
	}

	public PointSubScore GetPointSubScore(int numPoint)
	{
		PointSubScore result = pointSubScoreDefault;
		foreach (PointSubScore item in PointsSubScore)
		{
			if (SportBL.GetIndexSubScoreTennis(item.ValueSubScoreTeam1) + SportBL.GetIndexSubScoreTennis(item.ValueSubScoreTeam2) == numPoint)
			{
				result = item;
			}
		}
		return result;
	}

	public void Overturn()
	{
		int valuePointTeam = ValuePointTeam1;
		int valuePointTeam2 = ValuePointTeam2;
		int valuePointTeam3 = valuePointTeam;
		ValuePointTeam1 = valuePointTeam2;
		ValuePointTeam2 = valuePointTeam3;
		if (NumberGetTeam == 1)
		{
			NumberGetTeam = 2;
		}
		else if (NumberGetTeam == 2)
		{
			NumberGetTeam = 1;
		}
		foreach (PointSubScore item in PointsSubScore)
		{
			item.Overturn();
		}
	}

	public override string ToString()
	{
		return string.Format("Период:{0} Счёт: {1}:{2} обн. в {3}", Period, ValuePointTeam1, ValuePointTeam2, TimeCreate.ToString("HH:mm.ss"));
	}

	public string ToShortString()
	{
		return $"({ValuePointTeam1}:{ValuePointTeam2})";
	}
}
