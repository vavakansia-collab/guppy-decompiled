using System;
using System.Collections.Generic;
using System.Globalization;

namespace AccountsLibrary.CUPIS;

public class CupisResponse
{
	public string CupisLogin;

	public string Error { get; set; } = "";

	public string StatusIdentification { get; set; } = "";

	public int PagesCount { get; set; }

	public List<CupisBookmakerStatistic> Statistic { get; set; } = new List<CupisBookmakerStatistic>();

	public DateTime Timestamp { get; set; }

	public string CHECK_METHOD { get; set; }

	public CupisResponse ReturnError(string msg)
	{
		Error = msg;
		return this;
	}

	public override string ToString()
	{
		if (!string.IsNullOrEmpty(Error))
		{
			return Error;
		}
		string text = "";
		int num = 0;
		foreach (CupisBookmakerStatistic item in Statistic)
		{
			num += (int)item.SaldoTotal;
		}
		string text2 = "Проигрыш";
		if (num < 0)
		{
			text2 = "Выигрыш";
		}
		text += "Выполнено с помощью телеграм-бота @gbCupisBKBot";
		text += Environment.NewLine;
		text = text + Timestamp.ToUniversalTime().AddHours(3.0).ToString("dd.MM.yyyy HH:mm:ss") + " МСК";
		text += Environment.NewLine;
		text = text + "Результат по всем БК: " + text2 + " " + num.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		text += Environment.NewLine;
		if (Statistic.Count == 0)
		{
			text = text + Environment.NewLine + "История транзакций аккаунта пуста\r\n";
		}
		string text3 = "https://t.me/alexSmit_777";
		foreach (CupisBookmakerStatistic item2 in Statistic)
		{
			text += "......................................................";
			text += Environment.NewLine;
			text += item2.BookmakerCompany;
			string text4 = "";
			int num2 = (int)(DateTime.Now - item2.firstTransaction).TotalDays;
			if (num2 <= 30)
			{
				text4 = num2.ToString();
				text4 = ((!text4.EndsWith("11") && !text4.EndsWith("12") && !text4.EndsWith("13") && !text4.EndsWith("14")) ? (text4.EndsWith("1") ? (text4 + " день") : ((!text4.EndsWith("2") && !text4.EndsWith("3") && !text4.EndsWith("4")) ? (text4 + " дней") : (text4 + " дня"))) : (text4 + " дней"));
			}
			if (num2 > 30)
			{
				text4 = (num2 / 30).ToString();
				text4 = ((!text4.EndsWith("11") && !text4.EndsWith("12")) ? (text4.EndsWith("1") ? (text4 + " месяц") : ((!text4.EndsWith("2") && !text4.EndsWith("3") && !text4.EndsWith("4")) ? (text4 + " месяцев") : (text4 + " месяца"))) : (text4 + " месяцев"));
			}
			if (num2 >= 365)
			{
				int num3 = num2 / 365;
				if (num3 > 1)
				{
					text4 = "более двух лет";
				}
				else
				{
					text4 = num3 + " год";
					int num4 = (num2 - 365) / 30;
					if (num4 > 0)
					{
						text4 = text4 + " и " + num4;
						text4 = ((!text4.EndsWith("11") && !text4.EndsWith("12")) ? (text4.EndsWith("1") ? (text4 + " месяц") : ((!text4.EndsWith("2") && !text4.EndsWith("3") && !text4.EndsWith("4")) ? (text4 + " месяцев") : (text4 + " месяца"))) : (text4 + " месяцев"));
					}
				}
			}
			text = text + " (аккаунту " + text4 + ")";
			if (item2.SaldoTotal != 0.0)
			{
				text = text + Environment.NewLine + "Пополнения: " + item2.InputTotal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
				text = text + Environment.NewLine + "Выводы: " + item2.OutputTotal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
				text = text + Environment.NewLine + ((item2.SaldoTotal > 0.0) ? "Проигрыш: " : "Выигрыш: ".ToUpper()) + Math.Abs(item2.SaldoTotal).ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
				if (item2.SaldoPeriod != 0.0)
				{
					text = text + Environment.NewLine + "      Активность за последние 3 месяца:";
					text = text + Environment.NewLine + "      Пополнения: " + item2.InputPeriod.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
					text = text + Environment.NewLine + "      Выводы: " + item2.OutputPeriod.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
					text = text + Environment.NewLine + ((item2.SaldoPeriod > 0.0) ? "      Проигрыш: " : "      Выигрыш: ".ToUpper()) + Math.Abs(item2.SaldoPeriod).ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
					text += Environment.NewLine;
				}
				else
				{
					text = text + Environment.NewLine + "      Активность за последние 3 месяца отсутствует";
					text += Environment.NewLine;
				}
				text = text + Environment.NewLine + "Последнее пополнение: " + item2.lastTransaction.ToString("dd.MM.yyyy") + Environment.NewLine;
				if ((item2.BookmakerCompany == "БК «Лига Ставок»" && item2.InputPeriod > 50000.0 && item2.InputPeriod > 50000.0) || (item2.BookmakerCompany == "БК «OLIMPBET»" && item2.InputTotal > 30000.0 && item2.InputPeriod > 30000.0) || (item2.BookmakerCompany == "БК «ZENIT»" && item2.InputTotal > 50000.0 && item2.InputPeriod > 50000.0) || (item2.BookmakerCompany == "БК «БАЛТБЕТ»" && item2.InputTotal > 20000.0 && item2.InputPeriod > 20000.0) || (item2.BookmakerCompany == "БК «Леон»" && item2.InputTotal > 50000.0 && item2.InputPeriod > 50000.0) || (item2.BookmakerCompany == "БК «Winline»" && item2.InputTotal > 300000.0 && item2.InputPeriod > 300000.0) || ((item2.BookmakerCompany == "БК «PARI»" || item2.BookmakerCompany == "БК «FONBET»" || item2.BookmakerCompany == "БК «BETTERY»") && item2.InputTotal > 300000.0 && item2.InputPeriod > 200000.0))
				{
					text3 = "https://t.me/guppy_bet";
				}
			}
			else
			{
				text = text + Environment.NewLine + "Оборот отсутствует";
				text += Environment.NewLine;
			}
		}
		text += Environment.NewLine;
		if (!string.IsNullOrEmpty(StatusIdentification))
		{
			text = text + Environment.NewLine + "Статус ЦУПИС: " + StatusIdentification;
		}
		text += Environment.NewLine;
		if (text3.Contains("guppy_bet"))
		{
			text = text + Environment.NewLine + "Вы можете сдать в аренду/продать свои аккаунты безопасно, напрямую, без посредников. Подробности узнавайте у " + text3;
		}
		return text;
	}
}
