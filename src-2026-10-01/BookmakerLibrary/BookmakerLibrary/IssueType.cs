using System;
using System.ComponentModel;

namespace BookmakerLibrary;

[Serializable]
public enum IssueType
{
	[Description("Не определен тип")]
	_empty,
	[Description("ТБ")]
	TotalOver,
	[Description("ТМ")]
	TotalUnder,
	[Description("ИТБ1")]
	IndividualTotalOverTeam_1,
	[Description("ИТМ1")]
	IndividualTotalUnderTeam_1,
	[Description("ИТБ2")]
	IndividualTotalOverTeam_2,
	[Description("ИТМ2")]
	IndividualTotalUnderTeam_2,
	[Description("Фора 1")]
	OddsTeam_1,
	[Description("Фора 2")]
	OddsTeam_2,
	[Description("Очко П1")]
	PointP1,
	[Description("Очко П2")]
	PointP2,
	[Description("Гонка П1")]
	RaceP1,
	[Description("Гонка П2")]
	RaceP2,
	[Description("Четное")]
	Even,
	[Description("Нечетное")]
	Odd,
	[Description("Гейм П1")]
	GetTennisGameTeam1,
	[Description("Гейм П2")]
	GetTennisGameTeam2,
	[Description("Гейм П1 40:0")]
	TennisGameP1_40_0,
	[Description("Гейм П1 40:15")]
	TennisGameP1_40_15,
	[Description("Гейм П1 40:30")]
	TennisGameP1_40_30,
	[Description("Гейм П1 40:40")]
	TennisGameP1_40_40,
	[Description("Гейм П2 40:0")]
	TennisGameP2_40_0,
	[Description("Гейм П2 40:15")]
	TennisGameP2_40_15,
	[Description("Гейм П2 40:30")]
	TennisGameP2_40_30,
	[Description("Гейм П2 40:40")]
	TennisGameP2_40_40,
	[Description("Гейм 15:15 да")]
	TennisGame_15_15_Yes,
	[Description("Гейм 15:15 нет")]
	TennisGame_15_15_No,
	[Description("Гейм 30:30 да")]
	TennisGame_30_30_Yes,
	[Description("Гейм 30:30 нет")]
	TennisGame_30_30_No,
	[Description("Гейм 40:40 да")]
	TennisGame_40_40_Yes,
	[Description("Гейм 40:40 нет")]
	TennisGame_40_40_No,
	[Description("П1")]
	VictoryTeam_1,
	[Description("Ничья")]
	Draw,
	[Description("П2")]
	VictoryTeam_2,
	[Description("1Х")]
	Type1X,
	[Description("12")]
	Type12,
	[Description("Х2")]
	TypeX2
}
