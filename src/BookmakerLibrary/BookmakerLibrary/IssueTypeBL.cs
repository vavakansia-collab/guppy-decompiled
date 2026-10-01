using System;

namespace BookmakerLibrary;

public static class IssueTypeBL
{
	public static IssueType GetOverturnTypeCoupon(IssueType curIssueType)
	{
		return curIssueType switch
		{
			IssueType.OddsTeam_1 => IssueType.OddsTeam_2, 
			IssueType.OddsTeam_2 => IssueType.OddsTeam_1, 
			IssueType.VictoryTeam_1 => IssueType.VictoryTeam_2, 
			IssueType.VictoryTeam_2 => IssueType.VictoryTeam_1, 
			IssueType.IndividualTotalOverTeam_1 => IssueType.IndividualTotalOverTeam_2, 
			IssueType.IndividualTotalUnderTeam_1 => IssueType.IndividualTotalUnderTeam_2, 
			IssueType.IndividualTotalOverTeam_2 => IssueType.IndividualTotalOverTeam_1, 
			IssueType.IndividualTotalUnderTeam_2 => IssueType.IndividualTotalUnderTeam_1, 
			IssueType.Type1X => IssueType.TypeX2, 
			IssueType.TypeX2 => IssueType.Type1X, 
			IssueType.GetTennisGameTeam1 => IssueType.GetTennisGameTeam2, 
			IssueType.GetTennisGameTeam2 => IssueType.GetTennisGameTeam1, 
			IssueType.TennisGameP1_40_0 => IssueType.TennisGameP2_40_0, 
			IssueType.TennisGameP2_40_0 => IssueType.TennisGameP1_40_0, 
			IssueType.TennisGameP1_40_15 => IssueType.TennisGameP2_40_15, 
			IssueType.TennisGameP2_40_15 => IssueType.TennisGameP1_40_15, 
			IssueType.TennisGameP1_40_30 => IssueType.TennisGameP2_40_30, 
			IssueType.TennisGameP2_40_30 => IssueType.TennisGameP1_40_30, 
			IssueType.TennisGameP1_40_40 => IssueType.TennisGameP2_40_40, 
			IssueType.TennisGameP2_40_40 => IssueType.TennisGameP1_40_40, 
			IssueType.PointP1 => IssueType.PointP2, 
			IssueType.PointP2 => IssueType.PointP1, 
			IssueType.RaceP1 => IssueType.RaceP2, 
			IssueType.RaceP2 => IssueType.RaceP1, 
			_ => curIssueType, 
		};
	}

	public static string ConvertUserSrtIssueTypeCoupon(string issueType, double issueValue, int gameTennis = 0)
	{
		if (issueType.ToString() == "VictoryTeam_1")
		{
			return "Победа 1";
		}
		if (issueType.ToString() == "Draw")
		{
			return "Ничья";
		}
		if (issueType.ToString() == "VictoryTeam_2")
		{
			return "Победа 2";
		}
		if (issueType.ToString() == "Type12")
		{
			return "12";
		}
		if (issueType.ToString() == "Type1X")
		{
			return "1Х";
		}
		if (issueType.ToString() == "TypeX2")
		{
			return "Х2";
		}
		if (issueType.ToString() == "OddsTeam_1")
		{
			return "Ф1 " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "OddsTeam_2")
		{
			return "Ф2 " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "TotalOver")
		{
			return "ТБ " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "TotalUnder")
		{
			return "ТM " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "IndividualTotalOverTeam_1")
		{
			return "ИТБ1 " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "IndividualTotalUnderTeam_1")
		{
			return "ИТМ1 " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "IndividualTotalOverTeam_2")
		{
			return "ИТБ2 " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "IndividualTotalUnderTeam_2")
		{
			return "ИТМ2 " + Convert.ToString(issueValue);
		}
		if (issueType.ToString() == "GetTennisGameTeam1")
		{
			return "Гейм " + gameTennis + " Поб1";
		}
		if (issueType.ToString() == "GetTennisGameTeam2")
		{
			return "Гейм " + gameTennis + " Поб2";
		}
		if (issueType.ToString() == "TennisGame_15_15_Yes")
		{
			return "Гейм " + gameTennis + " Будет 15:15 - ДА";
		}
		if (issueType.ToString() == "TennisGame_15_15_No")
		{
			return "Гейм " + gameTennis + " Будет 15:15 - НЕТ";
		}
		if (issueType.ToString() == "TennisGame_30_30_Yes")
		{
			return "Гейм " + gameTennis + " Будет 30:30 - ДА";
		}
		if (issueType.ToString() == "TennisGame_30_30_No")
		{
			return "Гейм " + gameTennis + " Будет 30:30 - НЕТ";
		}
		if (issueType.ToString() == "TennisGame_40_40_Yes")
		{
			return "Гейм " + gameTennis + " Будет 40:40 - ДА";
		}
		if (issueType.ToString() == "TennisGame_40_40_No")
		{
			return "Гейм " + gameTennis + " Будет 40:40 - НЕТ";
		}
		if (issueType.ToString() == "PointP1")
		{
			return ((gameTennis == 0) ? "" : ("Гейм " + gameTennis + " ")) + "Очко " + Convert.ToString(issueValue) + " П1";
		}
		if (issueType.ToString() == "PointP2")
		{
			return ((gameTennis == 0) ? "" : ("Гейм " + gameTennis + " ")) + "Очко " + Convert.ToString(issueValue) + " П2";
		}
		if (issueType.ToString() == "RaceP1")
		{
			return "Гонка до " + Convert.ToString(issueValue) + " очков П1";
		}
		if (issueType.ToString() == "RaceP2")
		{
			return "Гонка до " + Convert.ToString(issueValue) + " очков П2";
		}
		if (issueType.ToString() == "Even")
		{
			return "Чет. ";
		}
		if (issueType.ToString() == "Odd")
		{
			return "Нечет ";
		}
		if (issueType.ToString() == "TennisGameP1_40_0")
		{
			return "Гейм " + gameTennis + " П1 со счётом 40:0";
		}
		if (issueType.ToString() == "TennisGameP1_40_15")
		{
			return "Гейм " + gameTennis + " П1 со счётом 40:15";
		}
		if (issueType.ToString() == "TennisGameP1_40_30")
		{
			return "Гейм " + gameTennis + " П1 со счётом 40:30";
		}
		if (issueType.ToString() == "TennisGameP1_40_40")
		{
			return "Гейм " + gameTennis + " П1 со счётом 40:40";
		}
		if (issueType.ToString() == "TennisGameP2_40_0")
		{
			return "Гейм " + gameTennis + " П2 со счётом 40:0";
		}
		if (issueType.ToString() == "TennisGameP2_40_15")
		{
			return "Гейм " + gameTennis + " П2 со счётом 40:15";
		}
		if (issueType.ToString() == "TennisGameP2_40_30")
		{
			return "Гейм " + gameTennis + " П2 со счётом 40:30";
		}
		if (issueType.ToString() == "TennisGameP2_40_40")
		{
			return "Гейм " + gameTennis + " П2 со счётом 40:40";
		}
		return issueType.ToString();
	}

	public static IssueType GetIssueTypeByStr(string strIssueType)
	{
		IssueType result = IssueType._empty;
		switch (strIssueType)
		{
		case "VictoryTeam_1":
			result = IssueType.VictoryTeam_1;
			break;
		case "Draw":
			result = IssueType.Draw;
			break;
		case "VictoryTeam_2":
			result = IssueType.VictoryTeam_2;
			break;
		case "Type1X":
			result = IssueType.Type1X;
			break;
		case "Type12":
			result = IssueType.Type12;
			break;
		case "TypeX2":
			result = IssueType.TypeX2;
			break;
		case "OddsTeam_1":
			result = IssueType.OddsTeam_1;
			break;
		case "OddsTeam_2":
			result = IssueType.OddsTeam_2;
			break;
		case "TotalOver":
			result = IssueType.TotalOver;
			break;
		case "TotalUnder":
			result = IssueType.TotalUnder;
			break;
		case "IndividualTotalOverTeam_1":
			result = IssueType.IndividualTotalOverTeam_1;
			break;
		case "IndividualTotalUnderTeam_1":
			result = IssueType.IndividualTotalUnderTeam_1;
			break;
		case "IndividualTotalOverTeam_2":
			result = IssueType.IndividualTotalOverTeam_2;
			break;
		case "IndividualTotalUnderTeam_2":
			result = IssueType.IndividualTotalUnderTeam_2;
			break;
		case "TennisGameP1_40_0":
			result = IssueType.TennisGameP1_40_0;
			break;
		case "TennisGameP1_40_15":
			result = IssueType.TennisGameP1_40_15;
			break;
		case "TennisGameP1_40_30":
			result = IssueType.TennisGameP1_40_30;
			break;
		case "TennisGameP1_40_40":
			result = IssueType.TennisGameP1_40_40;
			break;
		case "TennisGameP2_40_0":
			result = IssueType.TennisGameP2_40_0;
			break;
		case "TennisGameP2_40_15":
			result = IssueType.TennisGameP2_40_15;
			break;
		case "TennisGameP2_40_30":
			result = IssueType.TennisGameP2_40_30;
			break;
		case "TennisGameP2_40_40":
			result = IssueType.TennisGameP2_40_40;
			break;
		case "GetTennisGameTeam1":
			result = IssueType.GetTennisGameTeam1;
			break;
		case "GetTennisGameTeam2":
			result = IssueType.GetTennisGameTeam2;
			break;
		case "TennisGame_40_40_Yes":
			result = IssueType.TennisGame_40_40_Yes;
			break;
		case "TennisGame_40_40_No":
			result = IssueType.TennisGame_40_40_No;
			break;
		case "TennisGame_15_15_Yes":
			result = IssueType.TennisGame_15_15_Yes;
			break;
		case "TennisGame_15_15_No":
			result = IssueType.TennisGame_15_15_No;
			break;
		case "TennisGame_30_30_Yes":
			result = IssueType.TennisGame_30_30_Yes;
			break;
		case "TennisGame_30_30_No":
			result = IssueType.TennisGame_30_30_No;
			break;
		case "PointP1":
			result = IssueType.PointP1;
			break;
		case "PointP2":
			result = IssueType.PointP2;
			break;
		case "Even":
			result = IssueType.Even;
			break;
		case "Odd":
			result = IssueType.Odd;
			break;
		case "RaceP1":
			result = IssueType.RaceP1;
			break;
		case "RaceP2":
			result = IssueType.RaceP2;
			break;
		}
		return result;
	}

	public static IssueType GetInverseIssueType(int idSport, IssueType issueType)
	{
		if (idSport == 1 || idSport == 2)
		{
			switch (issueType)
			{
			case IssueType.VictoryTeam_1:
				return IssueType.TypeX2;
			case IssueType.VictoryTeam_2:
				return IssueType.Type1X;
			case IssueType.Draw:
				return IssueType.Type12;
			}
		}
		else
		{
			switch (issueType)
			{
			case IssueType.VictoryTeam_1:
				return IssueType.VictoryTeam_2;
			case IssueType.VictoryTeam_2:
				return IssueType.VictoryTeam_1;
			}
		}
		return issueType switch
		{
			IssueType.GetTennisGameTeam1 => IssueType.GetTennisGameTeam2, 
			IssueType.GetTennisGameTeam2 => IssueType.GetTennisGameTeam1, 
			IssueType.IndividualTotalOverTeam_1 => IssueType.IndividualTotalUnderTeam_1, 
			IssueType.IndividualTotalUnderTeam_1 => IssueType.IndividualTotalOverTeam_1, 
			IssueType.IndividualTotalOverTeam_2 => IssueType.IndividualTotalUnderTeam_2, 
			IssueType.IndividualTotalUnderTeam_2 => IssueType.IndividualTotalOverTeam_2, 
			IssueType.Odd => IssueType.Even, 
			IssueType.Even => IssueType.Odd, 
			IssueType.OddsTeam_1 => IssueType.OddsTeam_2, 
			IssueType.OddsTeam_2 => IssueType.OddsTeam_1, 
			IssueType.PointP1 => IssueType.PointP2, 
			IssueType.PointP2 => IssueType.PointP1, 
			IssueType.RaceP1 => IssueType.RaceP2, 
			IssueType.RaceP2 => IssueType.RaceP1, 
			IssueType.TennisGame_15_15_No => IssueType.TennisGame_15_15_Yes, 
			IssueType.TennisGame_15_15_Yes => IssueType.TennisGame_15_15_No, 
			IssueType.TennisGame_30_30_No => IssueType.TennisGame_30_30_Yes, 
			IssueType.TennisGame_30_30_Yes => IssueType.TennisGame_30_30_No, 
			IssueType.TennisGame_40_40_No => IssueType.TennisGame_40_40_Yes, 
			IssueType.TennisGame_40_40_Yes => IssueType.TennisGame_40_40_No, 
			IssueType.TotalOver => IssueType.TotalUnder, 
			IssueType.TotalUnder => IssueType.TotalOver, 
			_ => IssueType._empty, 
		};
	}
}
