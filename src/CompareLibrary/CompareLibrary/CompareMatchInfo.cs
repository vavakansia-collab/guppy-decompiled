using System;
using System.Collections.Generic;

namespace CompareLibrary;

public class CompareMatchInfo
{
	public DateTime DateTimeCreate = DateTime.Now;

	public bool Error;

	private double timeUpdate;

	public int InnerIdCompareMatch;

	public Dictionary<string, string> MatchesInfo = new Dictionary<string, string>();

	public string Message = "";

	public string ErrorInfo = "";

	public double TimeUpdate
	{
		get
		{
			return timeUpdate;
		}
		set
		{
			timeUpdate = Math.Round(value, 4);
		}
	}
}
