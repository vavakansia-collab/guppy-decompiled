using System.Collections.Generic;

namespace BookmakerLibrary;

public static class BookmakersBL
{
	public static List<Bookmaker> Bookmakers;

	static BookmakersBL()
	{
		Bookmakers = new List<Bookmaker>();
		Bookmakers.Add(new Bookmaker("_1Win", "_1Win", "", "1wazke.top"));
		Bookmakers.Add(new Bookmaker("_1xbet", "_1xbet", "", "1xbet.com"));
		Bookmakers.Add(new Bookmaker("_1xbit", "_1xbit", "", "1xbit.com"));
		Bookmakers.Add(new Bookmaker("Betwinner", "_1xbet", "", "betwinner-4021277.top"));
		Bookmakers.Add(new Bookmaker("Melbet", "_1xbet", "", "melbet-89993.top"));
		Bookmakers.Add(new Bookmaker("MelbetRU", "BetBoom", "БК «МЕЛБЕТ»", "melbet.ru"));
		Bookmakers.Add(new Bookmaker("BetBoom", "BetBoom", "БК «BetBoom»", "betboom.ru"));
		Bookmakers.Add(new Bookmaker("BaltbetRU", "BaltbetRU", "БК «Балтбет»", "baltbet.ru"));
		Bookmakers.Add(new Bookmaker("Betcity", "Betcity", "", "betcityru.com"));
		Bookmakers.Add(new Bookmaker("BetcityRU", "Betcity", "БК «БЕТСИТИ»", "betcity.ru"));
		Bookmakers.Add(new Bookmaker("ParibetRU", "FonbetRU", "БК «PARI»", "pari.ru"));
		Bookmakers.Add(new Bookmaker("TennisiBet", "Tennisi", "БК «TENNISI.bet»", "tennisi.bet"));
		Bookmakers.Add(new Bookmaker("Marathon", "Marathon", "", "marathonbet.com"));
		Bookmakers.Add(new Bookmaker("MarathonRU", "MarathonRU", "БК «Марафон»", "marathonbet.ru"));
		Bookmakers.Add(new Bookmaker("BetteryRU", "FonbetRU", "БК «BETTERY»", "bettery.ru"));
		Bookmakers.Add(new Bookmaker("FonbetRU", "FonbetRU", "БК «FON.BET»", "fon.bet"));
		Bookmakers.Add(new Bookmaker("LigaStavokRU", "LigaStavokRU", "БК «Лига Ставок»", "ligastavok.ru"));
		Bookmakers.Add(new Bookmaker("Leon", "LeonRU", "", "8332.info"));
		Bookmakers.Add(new Bookmaker("LeonRU", "LeonRU", "БК «Леон»", "leon.ru"));
		Bookmakers.Add(new Bookmaker("OlimpBet", "OlimpBet", "БК «OLIMPBET»", "olimp.bet"));
		Bookmakers.Add(new Bookmaker("Winline", "Winline", "БК «Winline»", "winline.ru"));
		Bookmakers.Add(new Bookmaker("ZenitWin", "ZenitWin", "БК «ZENIT»", "zenit.win"));
	}
}
