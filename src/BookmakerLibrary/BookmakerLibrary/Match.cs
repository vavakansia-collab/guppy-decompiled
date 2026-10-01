using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ProtoBuf;

namespace BookmakerLibrary;

public class Match
{
	private int idSport;

	private string team1;

	private string team2;

	public string NameProvider { get; set; }

	[ProtoIgnore]
	public string Name => NameProvider + ". " + ((!NameChamp.Contains(Sport.FullName) && !NameChamp.Contains(Sport.ShortName)) ? (Sport.FullName + ". ") : "") + NameChamp + " " + Team1 + " - " + Team2;

	public List<Event> Events { get; set; } = new List<Event>();

	public List<Coupon> Coupons { get; set; } = new List<Coupon>();

	public int IdSport
	{
		get
		{
			return idSport;
		}
		set
		{
			idSport = value;
			Sport = SportBL.GetSportByID(idSport);
		}
	}

	[IgnoreDataMember]
	[ProtoIgnore]
	public Sport Sport { get; private set; }

	public long IdMatch { get; set; }

	public int IdChamp { get; set; }

	public string NameChamp { get; set; } = "";

	public int PeriodDurationMinutes { get; set; } = 0;

	public string Team1
	{
		get
		{
			return team1;
		}
		set
		{
			if (value == null)
			{
				return;
			}
			team1 = value.Trim();
			if (string.IsNullOrEmpty(team1))
			{
				return;
			}
			if (IdSport == 10)
			{
				if (NameProvider == "_1Win" || NameProvider == "_1xbet" || NameProvider == "Betcity" || NameProvider == "Parimatch" || NameProvider == "LeonRU" || NameProvider == "Tennisi" || NameProvider == "ZenitWin")
				{
					string text = team1;
					if (text.Contains("("))
					{
						text = text.Remove(text.IndexOf("(")).Trim();
					}
					string[] array = text.Split(' ');
					if (array.Length == 2)
					{
						Team1FirstName = array[0];
						Team1LastName = array[1];
					}
				}
				else if (NameProvider == "BetBoom" || NameProvider == "BaltbetRU" || NameProvider == "FonbetRU" || NameProvider == "LigaStavokRU" || NameProvider == "Marathon" || NameProvider == "OlimpBet" || NameProvider == "Winline")
				{
					string text2 = team1;
					if (text2.Contains("("))
					{
						text2 = text2.Remove(text2.IndexOf("(")).Trim();
					}
					string[] array2 = text2.Split(' ');
					if (array2.Length == 2)
					{
						Team1FirstName = array2[1].Trim();
						Team1LastName = array2[0].Trim();
					}
				}
			}
			else
			{
				if (IdSport != 4)
				{
					return;
				}
				if (NameProvider == "_1Win" || NameProvider == "_1xbet" || NameProvider == "Betcity" || NameProvider == "Parimatch" || NameProvider == "LeonRU" || NameProvider == "ZenitWin")
				{
					string text3 = team1.Replace(",", "");
					if (text3.Contains("("))
					{
						text3 = text3.Remove(text3.IndexOf("(")).Trim();
					}
					string[] array3 = text3.Split(' ');
					if (array3.Length == 2)
					{
						Team1FirstName = array3[0];
						Team1LastName = array3[1];
					}
				}
				else if (NameProvider == "BaltbetRU" || NameProvider == "FonbetRU" || NameProvider == "LigaStavokRU" || NameProvider == "Marathon" || NameProvider == "OlimpBet" || NameProvider == "Tennisi" || NameProvider == "Winline")
				{
					string text4 = team1.Replace(",", "");
					if (text4.Contains("("))
					{
						text4 = text4.Remove(text4.IndexOf("(")).Trim();
					}
					string[] array4 = text4.Split(' ');
					if (array4.Length == 2)
					{
						Team1FirstName = array4[1].Trim();
						Team1LastName = array4[0].Trim();
					}
				}
			}
		}
	}

	public string Team1FirstName { get; set; }

	public string Team1LastName { get; set; }

	public string Team2
	{
		get
		{
			return team2;
		}
		set
		{
			if (value == null)
			{
				return;
			}
			team2 = value.Trim();
			if (string.IsNullOrEmpty(team2))
			{
				return;
			}
			if (IdSport == 10)
			{
				if (NameProvider == "_1Win" || NameProvider == "_1xbet" || NameProvider == "Betcity" || NameProvider == "Parimatch" || NameProvider == "LeonRU" || NameProvider == "Tennisi" || NameProvider == "ZenitWin")
				{
					string text = team2;
					if (text.Contains("("))
					{
						text = text.Remove(text.IndexOf("(")).Trim();
					}
					string[] array = text.Split(' ');
					if (array.Length == 2)
					{
						Team2FirstName = array[0];
						Team2LastName = array[1];
					}
				}
				else if (NameProvider == "BetBoom" || NameProvider == "BaltbetRU" || NameProvider == "FonbetRU" || NameProvider == "LigaStavokRU" || NameProvider == "Marathon" || NameProvider == "OlimpBet" || NameProvider == "Winline")
				{
					string text2 = team2;
					if (text2.Contains("("))
					{
						text2 = text2.Remove(text2.IndexOf("(")).Trim();
					}
					string[] array2 = text2.Split(' ');
					if (array2.Length == 2)
					{
						Team2FirstName = array2[1].Trim();
						Team2LastName = array2[0].Trim();
					}
				}
			}
			else
			{
				if (IdSport != 4)
				{
					return;
				}
				if (NameProvider == "_1Win" || NameProvider == "_1xbet" || NameProvider == "BetBoom" || NameProvider == "Betcity" || NameProvider == "Parimatch" || NameProvider == "LeonRU" || NameProvider == "ZenitWin")
				{
					string text3 = team2.Replace(",", "");
					if (text3.Contains("("))
					{
						text3 = text3.Remove(text3.IndexOf("(")).Trim();
					}
					string[] array3 = text3.Split(' ');
					if (array3.Length == 2)
					{
						Team2FirstName = array3[0];
						Team2LastName = array3[1];
					}
				}
				else if (NameProvider == "BaltbetRU" || NameProvider == "FonbetRU" || NameProvider == "LigaStavokRU" || NameProvider == "Marathon" || NameProvider == "OlimpBet" || NameProvider == "Tennisi" || NameProvider == "Winline")
				{
					string text4 = team2.Replace(",", "");
					if (text4.Contains("("))
					{
						text4 = text4.Remove(text4.IndexOf("(")).Trim();
					}
					string[] array4 = text4.Split(' ');
					if (array4.Length == 2)
					{
						Team2FirstName = array4[1].Trim();
						Team2LastName = array4[0].Trim();
					}
				}
			}
		}
	}

	public string Team2FirstName { get; set; }

	public string Team2LastName { get; set; }

	public Score Score { get; set; } = new Score();

	public int Period { get; set; } = 1;

	public TimeSpan ElapsedTime { get; set; }

	public DateTime StartTime { get; set; } = DateTime.MinValue;

	public DateTime FinishTime { get; set; } = DateTime.MinValue;

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public DateTime LastTimeUpdate { get; set; } = DateTime.Now;

	public TypesMatch TypeMatch { get; set; } = TypesMatch.Regular;

	public bool IsLine { get; set; } = false;

	public string Url { get; set; }

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

	public event OnUpdateMatchHandler OnUpdateMatchHandler;

	public event OnAddMessageHandler AddMessageHandler;

	public Match()
	{
		Score.OnUpdateScore += Score_OnUpdateScore;
		Score.OnErrorUpdateScoreHandler += OnErrorUpdateScore;
	}

	public Match(string initNameProvider)
	{
		NameProvider = initNameProvider;
		Score.OnErrorUpdateScoreHandler += OnErrorUpdateScore;
		Score.OnUpdateScore += Score_OnUpdateScore;
	}

	public Match Clone()
	{
		Match match = (Match)MemberwiseClone();
		match.Score = new Score
		{
			StrScore = Score.StrScore,
			LastTimeUpdate = Score.LastTimeUpdate
		};
		match.TypeMatch = TypeMatch;
		match.Coupons = Coupons.Select((Coupon c) => c.Clone()).ToList();
		return match;
	}

	public override string ToString()
	{
		return Name + " " + Score.StrScore;
	}

	private void OnErrorUpdateScore(string text)
	{
		this.OnUpdateMatchHandler?.Invoke(this);
		this.AddMessageHandler?.Invoke(Name + " " + text);
	}

	private void Score_OnUpdateScore(string periodsScore)
	{
		Period = periodsScore.Trim('(', ')').Split(',').Length;
		this.OnUpdateMatchHandler?.Invoke(this);
	}

	public string GetParameter(string nameParameter)
	{
		string result = null;
		foreach (KeyValuePair<string, string> parameter in Parameters)
		{
			if (parameter.Key == nameParameter)
			{
				result = parameter.Value;
			}
		}
		return result;
	}

	public void SetParameter(string nameParameter, string valueParameter)
	{
		Parameters.Remove(nameParameter);
		Parameters.Add(nameParameter, valueParameter);
	}

	public void RemoveData()
	{
		Coupons.Clear();
		foreach (PointScore item in Score.PointsScore)
		{
			item.PointsSubScore.Clear();
		}
		Score.PointsScore.Clear();
	}

	public string GetStrCouponsMatch()
	{
		string strCoupons = "";
		(from c in Coupons
			orderby c.Period, c.GameTennis, c.IssueType.ToString()
			select c).ToList().ForEach(delegate(Coupon coupon)
		{
			strCoupons = strCoupons + ((!coupon.Availability) ? "!!! НЕДОСТУПНЫЙ" : "") + Environment.NewLine + "Обн." + coupon.LastTimeUpdate.ToLongTimeString() + ((coupon.Period > 0) ? (" Период " + coupon.Period + " ") : " Весь матч ") + IssueTypeBL.ConvertUserSrtIssueTypeCoupon(coupon.IssueType.ToString(), coupon.IssueValue, coupon.GameTennis) + ", коэф." + coupon.PayoutRatio;
			if (coupon.Parameters.Count > 0)
			{
				strCoupons += ". Параметры купона: ";
				foreach (KeyValuePair<string, string> parameter in coupon.Parameters)
				{
					strCoupons = strCoupons + parameter.Key + " - " + parameter.Value + "; ";
				}
			}
			if (!string.IsNullOrEmpty(coupon.Comment))
			{
				strCoupons = strCoupons + Environment.NewLine + "Описание(фильтры): " + coupon.Comment;
			}
		});
		return strCoupons;
	}

	public string GetInfoMatch(bool getHeader = true, bool getCoupons = true, bool getPoints = true, double timeSpan = 0.0)
	{
		string strInfo = "";
		if (getHeader)
		{
			strInfo = strInfo + Environment.NewLine + "--------------------------" + Environment.NewLine;
			strInfo = strInfo + Environment.NewLine + "История матча БК " + NameProvider;
			strInfo = strInfo + Environment.NewLine + "Матч " + Name;
			strInfo = strInfo + Environment.NewLine + "Счёт: " + Score.StrScore;
			strInfo = strInfo + Environment.NewLine + "Создан: " + TimeCreate.ToLongTimeString() + ", обновлён в: " + LastTimeUpdate.ToLongTimeString() + " (" + Math.Round(DateTime.Now.Subtract(LastTimeUpdate).TotalSeconds - timeSpan, 2) + " сек. назад)";
			strInfo = strInfo + Environment.NewLine + "id:" + IdMatch + ", " + ((IdChamp != 0) ? (", idChamp = " + IdChamp) : "") + " " + Team1 + " - " + Team2 + " " + Score.StrScore + " период-" + Period;
			strInfo = strInfo + Environment.NewLine + "TypeMatch: " + TypeMatch;
			strInfo += ((Parameters.Count == 0) ? "" : (Environment.NewLine + "Параметры матча:" + Environment.NewLine));
			Parameters.ToList().ForEach(delegate(KeyValuePair<string, string> param)
			{
				strInfo = strInfo + "параметр: " + param.Key + " - " + param.Value + ", ";
			});
			strInfo = strInfo.TrimEnd(',');
		}
		if (getCoupons)
		{
			strInfo = strInfo + Environment.NewLine + "Всего купонов: " + Coupons.ToList().Count + Environment.NewLine + GetStrCouponsMatch();
		}
		if (getPoints)
		{
			strInfo = strInfo + Environment.NewLine + GetTimeLineScore();
		}
		strInfo = strInfo + Environment.NewLine + "..........................";
		return strInfo;
	}

	public int GetNumberTennisGamePeriodByMatch(int NumberGameMatch)
	{
		int num = 0;
		for (int i = 1; i <= Period; i++)
		{
			string scorePeriod = Score.GetScorePeriod(i);
			num += Convert.ToInt32(scorePeriod.Split(':')[0]) + Convert.ToInt32(scorePeriod.Split(':')[1]);
		}
		string scorePeriod2 = Score.GetScorePeriod(Period);
		int num2 = Convert.ToInt32(scorePeriod2.Split(':')[0]);
		int num3 = Convert.ToInt32(scorePeriod2.Split(':')[1]);
		int num4 = num2 + num3;
		if (NumberGameMatch == num)
		{
			return num4;
		}
		if (NumberGameMatch == num + 1)
		{
			return num4 + 1;
		}
		if (NumberGameMatch == num + 2)
		{
			return num4 + 2;
		}
		if (NumberGameMatch == num + 3)
		{
			return num4 + 3;
		}
		return 0;
	}

	public string GetTimeLineScore(int countTimeLinePoints = 0)
	{
		string text = "";
		int num = 0;
		foreach (PointScore item in Score.PointsScore)
		{
			num++;
			if (countTimeLinePoints > 0 && num > countTimeLinePoints)
			{
				continue;
			}
			text = text + "Время создания: " + item.TimeCreate.ToString() + ", загрузка на сервер: " + item.TimeServerLoad.ToString() + " период:" + item.Period + " " + ((item.NumberGetTeam == 1) ? "*" : " ") + item.ValuePointTeam1 + ":" + item.ValuePointTeam2 + ((item.NumberGetTeam == 2) ? "*" : "") + " isScoreDown:" + (item.IsScoreDown ? "Да" : "Нет") + " isDefault:" + (item.IsDefault ? "Да" : "Нет") + " IsAutoPasteScore:" + (item.IsAutoPasteScore ? "Да" : "Нет");
			if (Sport.IdSport == 4)
			{
				foreach (PointSubScore item2 in item.PointsSubScore)
				{
					num++;
					text = text + Environment.NewLine + "    Подсчёт гейма. Обн " + item2.TimeCreate.ToLongTimeString() + " (" + item2.ValueSubScoreTeam1 + ((Score.Server == 1) ? "*" : "") + ":" + item2.ValueSubScoreTeam2 + ((Score.Server == 2) ? "*" : "") + ") номер очка:" + item2.NumberPointGame + " взял:" + item2.NumberGetTeam + " IsScoreDown:" + (item2.IsScoreDown ? "Да" : "Нет") + " IsDefault:" + (item2.IsDefault ? "Да" : "Нет");
				}
			}
			text += Environment.NewLine;
		}
		return text;
	}

	public string GetStrParameters()
	{
		string text = "Параметры матча: ";
		foreach (KeyValuePair<string, string> parameter in Parameters)
		{
			text = text + parameter.Key + ":" + parameter.Value + ",";
		}
		if (text != "")
		{
			text = text.Remove(text.Length - 1);
		}
		return text;
	}

	public void Overturn()
	{
		string text = Team1;
		Team1 = Team2;
		Team2 = text;
		if (!string.IsNullOrEmpty(Team1FirstName))
		{
			text = Team1FirstName;
			Team1 = Team2FirstName;
			Team2 = text;
		}
		if (!string.IsNullOrEmpty(Team1LastName))
		{
			text = Team1LastName;
			Team1 = Team2LastName;
			Team2 = text;
		}
		Score.Overturn(this);
		foreach (Coupon coupon in Coupons)
		{
			coupon.Overturn();
		}
	}

	public void UpdateFullScore(string score)
	{
		if (score == "0:0")
		{
			score = "0:0(0:0)";
		}
		if (!string.IsNullOrEmpty(score))
		{
			Score.StrScore = score;
		}
		LastTimeUpdate = DateTime.Now;
	}

	public void UpdateScorePeriod(int period, int scoreTeam1, int scoreTeam2)
	{
		LastTimeUpdate = DateTime.Now;
		string text = "";
		System.Text.RegularExpressions.Match match = Regex.Match(Score.StrScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
		if (!match.Success)
		{
			return;
		}
		string text2 = match.Groups["fullScore"].Value.Trim('(', ')');
		string text3 = match.Groups["periodsScore"].Value.Trim('(', ')');
		if (period == 0)
		{
			text2 = scoreTeam1 + ":" + scoreTeam2;
			if (Sport.IsFullScoreParallelPeriod && !text3.Contains(",") && (scoreTeam1 > 0 || scoreTeam2 > 0))
			{
				text3 = scoreTeam1 + ":" + scoreTeam2;
			}
			text = text2 + "(" + text3 + ")";
		}
		else
		{
			text = text2 + "(";
			int num = 0;
			string[] array = text3.Split(',');
			foreach (string text4 in array)
			{
				num++;
				text = ((period != num) ? (text + text4) : (text + scoreTeam1 + ":" + scoreTeam2));
				text += ",";
			}
			if (period > num)
			{
				for (int j = num + 1; j <= period; j++)
				{
					text = ((j != period) ? (text + "0:0") : (text + scoreTeam1 + ":" + scoreTeam2));
					text += ",";
				}
			}
			text = text.TrimEnd(',') + ")";
		}
		if (IdSport == 4)
		{
			System.Text.RegularExpressions.Match match2 = Regex.Match(Score.StrScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
			if (match2.Success)
			{
				string value = match2.Groups["subScore"].Value;
				text += value;
			}
		}
		Score.StrScore = text;
	}

	public void UpdateSubScoreTennis(string subScoreTeam1, string subScoreTeam2)
	{
		LastTimeUpdate = DateTime.Now;
		if (string.IsNullOrEmpty(subScoreTeam1) || string.IsNullOrEmpty(subScoreTeam2))
		{
			return;
		}
		string strScore = "";
		System.Text.RegularExpressions.Match match = Regex.Match(Score.StrScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
		if (match.Success)
		{
			string text = match.Groups["fullScore"].Value.Trim('(', ')');
			string text2 = match.Groups["periodsScore"].Value.Trim('(', ')');
			strScore = text + "(" + text2 + ")(" + subScoreTeam1 + ":" + subScoreTeam2 + ")";
		}
		else
		{
			System.Text.RegularExpressions.Match match2 = Regex.Match(Score.StrScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
			if (match2.Success)
			{
				strScore = Score.StrScore + "(" + subScoreTeam1 + ":" + subScoreTeam2 + ")";
			}
		}
		Score.StrScore = strScore;
	}

	public bool IsEquals(Match _match)
	{
		return NameProvider == _match.NameProvider && IdMatch == _match.IdMatch;
	}

	public void UpdatePointsScore(string newScore, DateTime timeUpdate)
	{
		if (string.IsNullOrEmpty(newScore))
		{
			return;
		}
		try
		{
			System.Text.RegularExpressions.Match match = Regex.Match(newScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
			System.Text.RegularExpressions.Match match2 = Regex.Match(newScore, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))(?<subScore>\\(([^)]+)\\)[^)]*$)");
			string text = "";
			string text2 = "";
			if (match.Success)
			{
				text = match.Groups["fullScore"].Value.Trim('(', ')');
				text2 = match.Groups["periodsScore"].Value.Trim('(', ')');
				UpdatePointScore(0, int.Parse(text.Split(':')[0]), int.Parse(text.Split(':')[1]), newScore, timeUpdate);
				int num = 0;
				string[] array = text2.Split(',');
				foreach (string text3 in array)
				{
					num++;
					UpdatePointScore(num, int.Parse(text3.Split(':')[0]), int.Parse(text3.Split(':')[1]), newScore, timeUpdate);
				}
				if (IdSport == 4 && match2.Success)
				{
					string text4 = match2.Groups["subScore"].Value.Trim('(', ')').Replace("ADV", "A").Replace("Adv", "A")
						.Replace("00", "0");
					string subScoreTeam = text4.Split(':')[0];
					string subScoreTeam2 = text4.Split(':')[1];
					UpdatePointsSubScoreTennis(subScoreTeam, subScoreTeam2);
				}
			}
			Score.StrScore = DecomposedScore(newScore);
			Period = Score.PointsScore.Select((PointScore ps) => ps.Period).Max();
			if (Sport.IsSlowTimeGoal)
			{
				return;
			}
			int num2 = Sport.TotalPointsToCompare + 2;
			int period;
			for (period = 0; period < Period; period++)
			{
				List<PointScore> list = (from ps in Score.PointsScore
					where ps.Period == period
					orderby ps.TimeCreate
					select ps).ToList();
				if (list.Count > num2)
				{
					List<PointScore> psPeriod = new List<PointScore>();
					for (int num3 = 0; num3 < num2; num3++)
					{
						psPeriod.Add(Score.GetLastPointScore(period, num3));
					}
					List<PointScore> psRemove = Score.PointsScore.Where((PointScore ps) => ps.Period == period && !psPeriod.Contains(ps)).ToList();
					Score.PointsScore.RemoveAll((PointScore ps) => psRemove.Contains(ps));
				}
			}
		}
		catch
		{
		}
	}

	private void UpdatePointScore(int period, int scoreTeam1, int scoreTeam2, string score, DateTime timeUpdate, bool isAutoPasteScore = false)
	{
		PointScore lastPointScore = Score.GetLastPointScore(period);
		int valuePointTeam = lastPointScore.ValuePointTeam1;
		int valuePointTeam2 = lastPointScore.ValuePointTeam2;
		if (valuePointTeam != scoreTeam1 || valuePointTeam2 != scoreTeam2 || lastPointScore.IsDefault)
		{
			PointScore pointScore = new PointScore();
			pointScore.TimeCreate = timeUpdate;
			pointScore.TimeServerLoad = DateTime.Now;
			pointScore.Period = period;
			pointScore.ValuePointTeam1 = scoreTeam1;
			pointScore.ValuePointTeam2 = scoreTeam2;
			pointScore.IsAutoPasteScore = isAutoPasteScore;
			if (scoreTeam1 > valuePointTeam && valuePointTeam2 == scoreTeam2)
			{
				pointScore.NumberGetTeam = 1;
			}
			else if (scoreTeam2 > valuePointTeam2 && scoreTeam1 == valuePointTeam)
			{
				pointScore.NumberGetTeam = 2;
			}
			else
			{
				pointScore.NumberGetTeam = 0;
			}
			if (valuePointTeam > scoreTeam1 || valuePointTeam2 > scoreTeam2)
			{
				pointScore.IsScoreDown = true;
			}
			Score.PointsScore.Add(pointScore);
		}
	}

	private void UpdatePointsSubScoreTennis(string subScoreTeam1, string subScoreTeam2)
	{
		if (string.IsNullOrEmpty(subScoreTeam1) || string.IsNullOrEmpty(subScoreTeam2))
		{
			return;
		}
		subScoreTeam1 = subScoreTeam1.Replace("ADV", "A").Replace("Adv", "A").Replace("00", "0");
		subScoreTeam2 = subScoreTeam2.Replace("ADV", "A").Replace("Adv", "A").Replace("00", "0");
		PointScore pointScore = Score.GetLastPointScore(Period);
		string valueSubScoreTeam = pointScore.GetLastPointSubScore().ValueSubScoreTeam1;
		string valueSubScoreTeam2 = pointScore.GetLastPointSubScore().ValueSubScoreTeam2;
		int indexSubScoreTennis = SportBL.GetIndexSubScoreTennis(valueSubScoreTeam);
		int indexSubScoreTennis2 = SportBL.GetIndexSubScoreTennis(valueSubScoreTeam2);
		string text = subScoreTeam1;
		string text2 = subScoreTeam2;
		int indexSubScoreTennis3 = SportBL.GetIndexSubScoreTennis(text);
		int indexSubScoreTennis4 = SportBL.GetIndexSubScoreTennis(text2);
		if (valueSubScoreTeam != text || valueSubScoreTeam2 != text2)
		{
			if (pointScore.IsDefault)
			{
				PointScore pointScore2 = new PointScore();
				pointScore2.Period = 1;
				pointScore = pointScore2;
			}
			PointSubScore pointSubScore = new PointSubScore();
			pointSubScore.ValueSubScoreTeam1 = text;
			pointSubScore.ValueSubScoreTeam2 = text2;
			pointSubScore.NumberPointGame = indexSubScoreTennis3 + indexSubScoreTennis4;
			if (indexSubScoreTennis3 > indexSubScoreTennis && indexSubScoreTennis4 == indexSubScoreTennis2 && indexSubScoreTennis3 - indexSubScoreTennis == 1)
			{
				pointSubScore.NumberGetTeam = 1;
			}
			else if (indexSubScoreTennis4 > indexSubScoreTennis2 && indexSubScoreTennis3 == indexSubScoreTennis && indexSubScoreTennis4 - indexSubScoreTennis2 == 1)
			{
				pointSubScore.NumberGetTeam = 2;
			}
			else if (indexSubScoreTennis4 == 3 && indexSubScoreTennis2 == 4 && indexSubScoreTennis3 == indexSubScoreTennis && indexSubScoreTennis3 == 3)
			{
				pointSubScore.NumberGetTeam = 1;
			}
			else if (SportBL.GetIndexSubScoreTennis(text) == 3 && indexSubScoreTennis == 4 && indexSubScoreTennis4 == indexSubScoreTennis2 && indexSubScoreTennis4 == 3)
			{
				pointSubScore.NumberGetTeam = 2;
			}
			pointScore.PointsSubScore.Add(pointSubScore);
		}
	}

	public string DecomposedScore(string _score)
	{
		if (!Sport.IsFullScoreParallelPeriod)
		{
			return _score;
		}
		string result = _score;
		Score.matchRegex = Regex.Match(_score, "(?<fullScore>^\\d*\\:\\d*)(?<periodsScore>\\((.*?)\\))");
		if (Score.matchRegex.Success)
		{
			string text = Score.matchRegex.Groups["fullScore"].Value.Trim('(', ')');
			string text2 = Score.matchRegex.Groups["periodsScore"].Value.Trim('(', ')');
			int num = int.Parse(text.Split(':')[0]);
			int num2 = int.Parse(text.Split(':')[1]);
			int num3 = num + num2;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			string[] array = text2.Split(',');
			foreach (string text3 in array)
			{
				num7++;
				num4 += int.Parse(text3.Split(':')[0]);
				num5 += int.Parse(text3.Split(':')[1]);
			}
			num6 = num4 + num5;
			if (num3 > num6)
			{
				int num8 = num7 + 1;
				PointScore lastPointScore = Score.GetLastPointScore(num8 - 1);
				if (lastPointScore.IsAutoPasteScore && num8 > Period)
				{
					num8--;
				}
				result = text + "(" + text2 + "," + (num - num4) + ":" + (num2 - num5) + ")";
			}
		}
		return result;
	}

	public void UpdateTime(string timeString, TimeMode timeMode = TimeMode.Elapsed)
	{
		if (TimeSpan.TryParse(timeString, out var result))
		{
			ElapsedTime = ((timeMode == TimeMode.Elapsed) ? result : (TimeSpan.FromMinutes(PeriodDurationMinutes) - result));
		}
	}
}
