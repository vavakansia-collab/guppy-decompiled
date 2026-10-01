using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ProtoBuf;

namespace BookmakerLibrary;

public class Score : INotifyPropertyChanged
{
	[ProtoIgnore]
	[IgnoreDataMember]
	public const string patternScore = "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))";

	[ProtoIgnore]
	[IgnoreDataMember]
	public const string patternScoreTennis = "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)";

	[ProtoIgnore]
	[IgnoreDataMember]
	public System.Text.RegularExpressions.Match matchRegex;

	[ProtoIgnore]
	[IgnoreDataMember]
	public System.Text.RegularExpressions.Match matchRegexTennis;

	private string strScore = "0:0(0:0)";

	private int server = 0;

	public DateTime LastTimeUpdate { get; set; } = DateTime.MinValue;

	public string StrScore
	{
		get
		{
			return strScore;
		}
		set
		{
			LastTimeUpdate = DateTime.Now;
			if (value.Trim() == "0:0")
			{
				value = "0:0(0:0)";
			}
			matchRegex = Regex.Match(value, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
			matchRegexTennis = Regex.Match(value, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
			if (matchRegex.Success || matchRegexTennis.Success)
			{
				string value2 = matchRegex.Groups["fullScore"].Value;
				string text = matchRegex.Groups["periodsScore"].Value.Replace("00", "0");
				if (value2.Contains(":") && text.Split(',').Length == text.Split(':').Length - 1)
				{
					if (matchRegexTennis.Success)
					{
						string text2 = matchRegexTennis.Groups["subScore"].Value.Replace("ADV", "A").Replace("Adv", "A").Replace("AD", "A")
							.Replace("50", "A")
							.Replace("00", "0");
						strScore = value2 + text + text2;
					}
					else
					{
						strScore = value2 + text;
					}
					RaisePropertyChanged("StrScore");
					this.OnUpdateScore?.Invoke(text);
				}
				else
				{
					this.OnErrorUpdateScoreHandler?.Invoke("Не смогли обновить счёт матча " + value);
				}
			}
			else
			{
				this.OnErrorUpdateScoreHandler?.Invoke("Не смогли обновить счёт матча " + value);
			}
		}
	}

	public bool IsScoreUpdated { get; set; } = true;

	public int Server
	{
		get
		{
			return server;
		}
		set
		{
			if (value == 0 || value == 1 || value == 2)
			{
				server = value;
			}
		}
	}

	public virtual List<PointScore> PointsScore { get; set; } = new List<PointScore>();

	private static PointScore PointScoreDefault { get; set; }

	public event OnUpdateScoreHandler OnUpdateScore;

	public event OnUpdateScoreHandler OnErrorUpdateScoreHandler;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public Score()
	{
		PointScoreDefault = new PointScore
		{
			IsDefault = true,
			Period = 1
		};
	}

	public override string ToString()
	{
		return StrScore;
	}

	public PointScore GetLastPointScore(int period, int deviation = 0)
	{
		PointScore result = PointScoreDefault;
		int num = -1;
		for (int num2 = PointsScore.Count; num2 > 0; num2--)
		{
			PointScore pointScore = PointsScore[num2 - 1];
			if (pointScore.Period == period)
			{
				num++;
				if (num == deviation)
				{
					result = pointScore;
					break;
				}
			}
		}
		return result;
	}

	public PointScore GetPoint(int period, int numPoint)
	{
		PointScore pointScore = null;
		foreach (PointScore item in PointsScore)
		{
			if (item.Period == period && item.ValuePointTeam1 + item.ValuePointTeam2 == numPoint)
			{
				pointScore = item;
				break;
			}
		}
		PointScore pointScore2 = null;
		for (int num = PointsScore.Count; num > 0; num--)
		{
			PointScore pointScore3 = PointsScore[num - 1];
			if (pointScore3.Period == period)
			{
				pointScore2 = pointScore3;
				break;
			}
		}
		if (pointScore == null || pointScore2 == null || pointScore.ValuePointTeam1 + pointScore.ValuePointTeam2 > pointScore2.ValuePointTeam1 + pointScore2.ValuePointTeam2)
		{
			pointScore = PointScoreDefault;
		}
		return pointScore;
	}

	public void Overturn(Match match)
	{
		if (StrScore == "0:0(0:0)" || StrScore == "0:0(0:0)(0:0)")
		{
			return;
		}
		foreach (PointScore item in PointsScore)
		{
			item.Overturn();
		}
		if (server == 1)
		{
			server = 2;
		}
		else if (server == 2)
		{
			server = 1;
		}
		string overturnScore = GetOverturnScore(match);
		if (StrScore != overturnScore)
		{
			StrScore = overturnScore;
		}
	}

	public static string GetOverturnScore(Match match)
	{
		string text = "";
		for (int i = 0; i <= match.Period; i++)
		{
			string scorePeriod = match.Score.GetScorePeriod(i);
			text = ((i != 0) ? (text + GetOverturnScore(scorePeriod) + ",") : (GetOverturnScore(scorePeriod) + "("));
		}
		text = text.TrimEnd(',');
		text += ")";
		string strSubScore = match.Score.GetStrSubScore();
		if (!string.IsNullOrEmpty(strSubScore))
		{
			text = text + "(" + GetOverturnScore(strSubScore) + ")";
		}
		return text;
	}

	private static string GetOverturnScore(string scorePeriod)
	{
		string result = "";
		string[] array = scorePeriod.Split(':');
		if (array.Length < 2)
		{
			return result;
		}
		string text = array[0];
		string text2 = array[1];
		return text2 + ":" + text;
	}

	public static string GetStrScoreByPointsScore(Match match)
	{
		PointScore lastPointScore = match.Score.GetLastPointScore(0);
		string text = lastPointScore.ValuePointTeam1.ToString();
		string text2 = lastPointScore.ValuePointTeam2.ToString();
		string text3 = text + ":" + text2 + "(";
		if (match.Score.PointsScore.Count > 0)
		{
			for (int i = 1; i <= match.Score.PointsScore.Select((PointScore ps) => ps.Period).Max(); i++)
			{
				lastPointScore = match.Score.GetLastPointScore(i);
				string text4 = lastPointScore.ValuePointTeam1.ToString();
				string text5 = lastPointScore.ValuePointTeam2.ToString();
				text3 = text3 + text4 + ":" + text5 + ",";
			}
		}
		else
		{
			text3 += "0:0";
		}
		text3 = text3.TrimEnd(',');
		text3 += ")";
		if (match.IdSport == 4)
		{
			PointSubScore lastPointSubScore = lastPointScore.GetLastPointSubScore();
			text3 = text3 + "(" + lastPointSubScore.ValueSubScoreTeam1 + ":" + lastPointSubScore.ValueSubScoreTeam2 + ")";
		}
		return text3;
	}

	public static int GetTotalPointsDifference(Match match1, Match match2)
	{
		int num = 0;
		int period = 0;
		if (!match1.Sport.IsFullScoreParallelPeriod)
		{
			period = match1.Period;
		}
		PointScore lastPointScore = match1.Score.GetLastPointScore(period);
		int valuePointTeam = lastPointScore.ValuePointTeam1;
		int valuePointTeam2 = lastPointScore.ValuePointTeam2;
		int num2 = valuePointTeam + valuePointTeam2;
		PointScore lastPointScore2 = match2.Score.GetLastPointScore(period);
		int valuePointTeam3 = lastPointScore2.ValuePointTeam1;
		int valuePointTeam4 = lastPointScore2.ValuePointTeam2;
		int num3 = valuePointTeam3 + valuePointTeam4;
		if (valuePointTeam > valuePointTeam3 || valuePointTeam2 > valuePointTeam4)
		{
			return -1;
		}
		if (match1.Sport.IdSport != 4)
		{
			int num4 = valuePointTeam3 - valuePointTeam;
			int num5 = valuePointTeam4 - valuePointTeam2;
			num = Math.Abs(num5 - num4);
			if (match1.Period != match2.Period)
			{
				period = Math.Min(match1.Period, match2.Period);
				lastPointScore = match1.Score.GetLastPointScore(period);
				valuePointTeam = lastPointScore.ValuePointTeam1;
				valuePointTeam2 = lastPointScore.ValuePointTeam2;
				num2 = valuePointTeam + valuePointTeam2;
				lastPointScore2 = match2.Score.GetLastPointScore(period);
				valuePointTeam3 = lastPointScore2.ValuePointTeam1;
				valuePointTeam4 = lastPointScore2.ValuePointTeam2;
				num3 = valuePointTeam3 + valuePointTeam4;
				num += Math.Abs(num3 - num2);
			}
			if (num2 > num3)
			{
				num *= -1;
			}
		}
		else
		{
			int indexSubScoreTennis = SportBL.GetIndexSubScoreTennis(lastPointScore.GetLastPointSubScore().ValueSubScoreTeam1);
			int indexSubScoreTennis2 = SportBL.GetIndexSubScoreTennis(lastPointScore.GetLastPointSubScore().ValueSubScoreTeam2);
			int indexSubScoreTennis3 = SportBL.GetIndexSubScoreTennis(lastPointScore2.GetLastPointSubScore().ValueSubScoreTeam1);
			int indexSubScoreTennis4 = SportBL.GetIndexSubScoreTennis(lastPointScore2.GetLastPointSubScore().ValueSubScoreTeam2);
			if (num2 == num3)
			{
				num = indexSubScoreTennis3 + indexSubScoreTennis4 - (indexSubScoreTennis + indexSubScoreTennis2);
				if (indexSubScoreTennis >= 3 && indexSubScoreTennis2 >= 3 && indexSubScoreTennis3 >= 3 && indexSubScoreTennis4 >= 3 && lastPointScore.GetLastPointSubScore().TimeCreate < lastPointScore2.GetLastPointSubScore().TimeCreate)
				{
					num = Math.Abs(num);
				}
			}
			else if (num3 == 0 && num2 != num3)
			{
				num = 0;
			}
			else if (num3 - num2 != 1)
			{
				num = ((num3 - num2 <= 1) ? (num3 - num2) : 4);
			}
			else
			{
				int num6 = ((Math.Max(indexSubScoreTennis, indexSubScoreTennis2) == 4 || (Math.Max(indexSubScoreTennis, indexSubScoreTennis2) == 3 && Math.Min(indexSubScoreTennis, indexSubScoreTennis2) < 3) || (indexSubScoreTennis == 3 && indexSubScoreTennis2 == 3 && match1.Team1.Contains("/"))) ? 1 : (4 - Math.Max(indexSubScoreTennis, indexSubScoreTennis2)));
				num = indexSubScoreTennis3 + indexSubScoreTennis4 + num6;
			}
		}
		return num;
	}

	public string GetScorePeriod(int period)
	{
		System.Text.RegularExpressions.Match match = Regex.Match(StrScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
		if (match.Success)
		{
			if (period == 0)
			{
				return match.Groups["fullScore"].Value.Trim('(', ')');
			}
			string text = match.Groups["periodsScore"].Value.Trim('(', ')');
			int num = 0;
			string[] array = text.Split(',');
			foreach (string result in array)
			{
				num++;
				if (period == num)
				{
					return result;
				}
			}
		}
		return "";
	}

	public static int GetCurrentPeriod(string score)
	{
		int result = -1;
		System.Text.RegularExpressions.Match match = Regex.Match(score, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
		if (match.Success)
		{
			string text = match.Groups["periodsScore"].Value.Trim('(', ')');
			result = text.Split(',').Count();
		}
		return result;
	}

	public string GetStrSubScore()
	{
		System.Text.RegularExpressions.Match match = Regex.Match(StrScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
		if (match.Success)
		{
			return match.Groups["subScore"].Value.Trim('(', ')');
		}
		return null;
	}

	public int GetScorePeriodTeam(int period, int numTeam)
	{
		int result = -1;
		string scorePeriod = GetScorePeriod(period);
		if (string.IsNullOrEmpty(scorePeriod))
		{
			return result;
		}
		switch (numTeam)
		{
		case 1:
			int.TryParse(scorePeriod.Split(':')[0], out result);
			break;
		case 2:
			int.TryParse(scorePeriod.Split(':')[1], out result);
			break;
		}
		return result;
	}

	public static string GetScorePeriod(string score, int period)
	{
		System.Text.RegularExpressions.Match match = Regex.Match(score, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
		if (match.Success)
		{
			if (period == 0)
			{
				return match.Groups["fullScore"].Value.Trim('(', ')');
			}
			string text = match.Groups["periodsScore"].Value.Trim('(', ')');
			int num = 0;
			string[] array = text.Split(',');
			foreach (string result in array)
			{
				num++;
				if (period == num)
				{
					return result;
				}
			}
		}
		return "0:0";
	}

	public static string GetStrSubScore(string score)
	{
		System.Text.RegularExpressions.Match match = Regex.Match(score, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
		if (match.Success)
		{
			return match.Groups["subScore"].Value.Trim('(', ')');
		}
		return "0:0";
	}

	public static string AddPointPeriod(string score, int numTeam, int idSport)
	{
		string text = score;
		System.Text.RegularExpressions.Match match = Regex.Match(score, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
		if (!match.Success)
		{
			return null;
		}
		string value = match.Groups["fullScore"].Value;
		string text2 = match.Groups["periodsScore"].Value.Replace("00", "0");
		if (idSport != 4)
		{
			int currentPeriod = GetCurrentPeriod(score);
			string scorePeriod = GetScorePeriod(score, currentPeriod);
			int num = int.Parse(scorePeriod.Split(':')[0]);
			int num2 = int.Parse(scorePeriod.Split(':')[1]);
			switch (numTeam)
			{
			case 1:
				num++;
				break;
			case 2:
				num2++;
				break;
			}
			string text3 = "";
			int num3 = 0;
			string[] array = text2.Trim('(', ')').Split(',');
			foreach (string text4 in array)
			{
				num3++;
				text3 = ((currentPeriod != num3) ? (text3 + text4 + ",") : (text3 + $"{num}:{num2},"));
			}
			text3 = "(" + text3.TrimEnd(',') + ")";
			text = value + text3;
		}
		else
		{
			System.Text.RegularExpressions.Match match2 = Regex.Match(score, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
			if (!match2.Success)
			{
				return null;
			}
			string text5 = match2.Groups["subScore"].Value.Replace("ADV", "A").Replace("Adv", "A").Replace("AD", "A")
				.Replace("50", "A")
				.Replace("00", "0");
			int currentPeriod2 = GetCurrentPeriod(score);
			string scorePeriod2 = GetScorePeriod(score, currentPeriod2);
			int num4 = int.Parse(scorePeriod2.Split(':')[0]);
			int num5 = int.Parse(scorePeriod2.Split(':')[1]);
			int num6 = num4 + num5 + 1;
			string strSubScore = GetStrSubScore(score);
			string text6 = strSubScore.Split(':')[0];
			string text7 = strSubScore.Split(':')[1];
			int indexSubScoreTennis = SportBL.GetIndexSubScoreTennis(text6);
			int indexSubScoreTennis2 = SportBL.GetIndexSubScoreTennis(text7);
			switch (numTeam)
			{
			case 1:
				switch (indexSubScoreTennis)
				{
				case 0:
					text6 = "15";
					break;
				case 1:
					text6 = "30";
					break;
				case 2:
					text6 = "40";
					break;
				case 3:
					if (indexSubScoreTennis == indexSubScoreTennis2)
					{
						text6 = "A";
					}
					else if (indexSubScoreTennis > indexSubScoreTennis2)
					{
						num4++;
						text6 = "0";
						text7 = "0";
					}
					else
					{
						text6 = "40";
						text7 = "40";
					}
					break;
				case 4:
					num4++;
					text6 = "0";
					text7 = "0";
					break;
				}
				break;
			case 2:
				if (indexSubScoreTennis2 < 3)
				{
					if (indexSubScoreTennis2 == 0)
					{
						text7 = "15";
					}
					else if (indexSubScoreTennis == 1)
					{
						text7 = "30";
					}
					else if (indexSubScoreTennis2 == 2)
					{
						text7 = "40";
					}
					break;
				}
				switch (indexSubScoreTennis2)
				{
				case 3:
					if (indexSubScoreTennis == indexSubScoreTennis2)
					{
						text7 = "A";
					}
					else if (indexSubScoreTennis < indexSubScoreTennis2)
					{
						num5++;
						text6 = "0";
						text7 = "0";
					}
					else
					{
						text6 = "40";
						text7 = "40";
					}
					break;
				case 4:
					num5++;
					text6 = "0";
					text7 = "0";
					break;
				}
				break;
			}
			text5 = "(" + text6 + ":" + text7 + ")";
			string text8 = text2;
			if (scorePeriod2 != $"{num4}:{num5}")
			{
				text8 = "";
				int num7 = 0;
				string[] array2 = text2.Trim('(', ')').Split(',');
				foreach (string text9 in array2)
				{
					num7++;
					text8 = ((currentPeriod2 != num7) ? (text8 + text9 + ",") : (text8 + $"{num4}:{num5},"));
				}
				text8 = "(" + text8.TrimEnd(',') + ")";
			}
			text = value + text8 + text5;
		}
		return text;
	}

	public static bool CheckConsistentScore(Match match, int period)
	{
		if (!match.Sport.IsConsistentScore)
		{
			return true;
		}
		if (match.IdSport != 4)
		{
			for (int i = 0; i < 2; i++)
			{
				PointScore lastPointScore = match.Score.GetLastPointScore(period, i);
				PointScore lastPointScore2 = match.Score.GetLastPointScore(period, i + 1);
				if (lastPointScore.IsScoreDown || lastPointScore2.IsScoreDown)
				{
					return false;
				}
				int valuePointTeam = lastPointScore.ValuePointTeam1;
				int valuePointTeam2 = lastPointScore2.ValuePointTeam1;
				int valuePointTeam3 = lastPointScore.ValuePointTeam2;
				int valuePointTeam4 = lastPointScore2.ValuePointTeam2;
				if (valuePointTeam - valuePointTeam2 > 1 || valuePointTeam3 - valuePointTeam4 > 1)
				{
					return false;
				}
			}
		}
		else
		{
			PointScore lastPointScore3 = match.Score.GetLastPointScore(period);
			if (lastPointScore3.PointsSubScore.Count >= 2)
			{
				PointSubScore pointSubScore = lastPointScore3.PointsSubScore[lastPointScore3.PointsSubScore.Count - 1];
				PointSubScore pointSubScore2 = lastPointScore3.PointsSubScore[lastPointScore3.PointsSubScore.Count - 2];
				if (pointSubScore.IsDefault || pointSubScore2.IsDefault || pointSubScore.IsScoreDown || pointSubScore2.IsScoreDown || pointSubScore.NumberGetTeam == 0 || pointSubScore2.NumberGetTeam == 0)
				{
					return false;
				}
			}
			else
			{
				lastPointScore3 = match.Score.GetLastPointScore(period, 1);
				if (lastPointScore3.PointsSubScore.Count >= 2)
				{
					PointSubScore pointSubScore3 = lastPointScore3.PointsSubScore[lastPointScore3.PointsSubScore.Count - 1];
					PointSubScore pointSubScore4 = lastPointScore3.PointsSubScore[lastPointScore3.PointsSubScore.Count - 2];
					if (pointSubScore3.IsDefault || pointSubScore4.IsDefault || pointSubScore3.IsScoreDown || pointSubScore4.IsScoreDown || pointSubScore3.NumberGetTeam == 0 || pointSubScore4.NumberGetTeam == 0 || Math.Max(SportBL.GetIndexSubScoreTennis(pointSubScore3.ValueSubScoreTeam1), SportBL.GetIndexSubScoreTennis(pointSubScore3.ValueSubScoreTeam2)) < 3)
					{
						return false;
					}
				}
			}
		}
		return true;
	}
}
