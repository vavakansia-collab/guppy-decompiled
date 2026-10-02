using System;

namespace BookmakerLibrary;

public class MatchFinish
{
	public string NameProvider;

	public int IdSport;

	public long IdMatch;

	public string Score;

	public DateTime FinishTime;

	public DateTime StartData;

	public override string ToString()
	{
		return $"{NameProvider} {SportBL.GetSportByID(IdSport).ShortName} {IdMatch} {Score}";
	}
}
