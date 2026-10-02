using System.Collections.Generic;
using System.Globalization;

namespace AccountsLibrary;

public static class BookmakerNameConverter
{
	private static readonly CultureInfo RuCultureInfo = new CultureInfo("ru-RU");

	private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo
	{
		NumberDecimalSeparator = ".",
		NumberGroupSeparator = "\u00a0"
	};

	private static readonly Dictionary<string, string> MapBookmakerNames = new Dictionary<string, string>
	{
		{ "БК «Лига Ставок»", "LigaStavokRU" },
		{ "БК «МЕЛБЕТ»", "MelbetRU" },
		{ "БК «Winline»", "Winline" },
		{ "БК «1хСтавка»", "1xСтавка" },
		{ "БК «Tennisi.bet»", "TennisiBet" },
		{ "БК «БЕТСИТИ»", "BetcityRU" },
		{ "БК «FON.BET»", "FonbetRU" },
		{ "БК «Балтбет»", "BaltbetRU" },
		{ "БК «Олимп»", "OlimpBet" },
		{ "БК «OLIMPBET»", "OlimpBet" },
		{ "БК «Леон»", "LeonRU" },
		{ "БК «BetBoom»", "BetBoom" },
		{ "БК «Марафон»", "MarathonbetRU" },
		{ "БК «Зенит»", "ZenitWin" },
		{ "БК «BETTERY»", "BetteryRU" },
		{ "БК «Pari»", "ParibetRU" }
	};

	public static string ToNameProvider(string nameBC)
	{
		string bookmaker = "";
		try
		{
			MapBookmakerNames.TryGetValue(nameBC, out bookmaker);
		}
		catch
		{
		}
		return bookmaker;
	}
}
