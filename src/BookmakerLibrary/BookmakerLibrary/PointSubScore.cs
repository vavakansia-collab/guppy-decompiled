using System;

namespace BookmakerLibrary;

public class PointSubScore
{
	private string valueSubScoreTeam1 = "0";

	private string valueSubScoreTeam2 = "0";

	public Guid GUID { get; set; } = Guid.NewGuid();

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public string ValueSubScoreTeam1
	{
		get
		{
			return valueSubScoreTeam1;
		}
		set
		{
			if (value == "00")
			{
				value = "0";
			}
			valueSubScoreTeam1 = value;
		}
	}

	public string ValueSubScoreTeam2
	{
		get
		{
			return valueSubScoreTeam2;
		}
		set
		{
			if (value == "00")
			{
				value = "0";
			}
			valueSubScoreTeam2 = value;
		}
	}

	public int NumberPointGame { get; set; } = 0;

	public int NumberGetTeam { get; set; } = 0;

	public bool IsDefault { get; set; } = false;

	public bool IsScoreDown { get; set; } = false;

	public void Overturn()
	{
		string text = ValueSubScoreTeam1;
		string text2 = ValueSubScoreTeam2;
		string text3 = text;
		ValueSubScoreTeam1 = text2;
		ValueSubScoreTeam2 = text3;
		if (NumberGetTeam == 1)
		{
			NumberGetTeam = 2;
		}
		else if (NumberGetTeam == 2)
		{
			NumberGetTeam = 1;
		}
	}

	public override string ToString()
	{
		return ValueSubScoreTeam1 + ":" + ValueSubScoreTeam2 + " обн. в " + TimeCreate.ToString("HH:mm.ss");
	}
}
