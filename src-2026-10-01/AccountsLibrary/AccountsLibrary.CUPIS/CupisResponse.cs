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
		string history = "";
		int total = 0;
		foreach (CupisBookmakerStatistic statistic in Statistic)
		{
			total += (int)statistic.SaldoTotal;
		}
		string total_type = "Проигрыш";
		if (total < 0)
		{
			total_type = "Выигрыш";
		}
		history += "Выполнено с помощью телеграм-бота @gbCupisBKBot";
		history += Environment.NewLine;
		history = history + Timestamp.ToUniversalTime().AddHours(3.0).ToString("dd.MM.yyyy HH:mm:ss") + " МСК";
		history += Environment.NewLine;
		history = history + "Результат по всем БК: " + total_type + " " + total.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
		history += Environment.NewLine;
		if (Statistic.Count == 0)
		{
			history = history + Environment.NewLine + "История транзакций аккаунта пуста\r\n";
		}
		string contactSellAccouts = "https://t.me/alexSmit_777";
		foreach (CupisBookmakerStatistic statistic2 in Statistic)
		{
			history += "......................................................";
			history += Environment.NewLine;
			history += statistic2.BookmakerCompany;
			string existenceTime = "";
			int days = (int)(DateTime.Now - statistic2.firstTransaction).TotalDays;
			if (days <= 30)
			{
				existenceTime = days.ToString();
				existenceTime = ((!existenceTime.EndsWith("11") && !existenceTime.EndsWith("12") && !existenceTime.EndsWith("13") && !existenceTime.EndsWith("14")) ? (existenceTime.EndsWith("1") ? (existenceTime + " день") : ((!existenceTime.EndsWith("2") && !existenceTime.EndsWith("3") && !existenceTime.EndsWith("4")) ? (existenceTime + " дней") : (existenceTime + " дня"))) : (existenceTime + " дней"));
			}
			if (days > 30)
			{
				existenceTime = (days / 30).ToString();
				existenceTime = ((!existenceTime.EndsWith("11") && !existenceTime.EndsWith("12")) ? (existenceTime.EndsWith("1") ? (existenceTime + " месяц") : ((!existenceTime.EndsWith("2") && !existenceTime.EndsWith("3") && !existenceTime.EndsWith("4")) ? (existenceTime + " месяцев") : (existenceTime + " месяца"))) : (existenceTime + " месяцев"));
			}
			if (days >= 365)
			{
				int years = days / 365;
				if (years > 1)
				{
					existenceTime = "более двух лет";
				}
				else
				{
					existenceTime = years + " год";
					int months = (days - 365) / 30;
					if (months > 0)
					{
						existenceTime = existenceTime + " и " + months;
						existenceTime = ((!existenceTime.EndsWith("11") && !existenceTime.EndsWith("12")) ? (existenceTime.EndsWith("1") ? (existenceTime + " месяц") : ((!existenceTime.EndsWith("2") && !existenceTime.EndsWith("3") && !existenceTime.EndsWith("4")) ? (existenceTime + " месяцев") : (existenceTime + " месяца"))) : (existenceTime + " месяцев"));
					}
				}
			}
			history = history + " (аккаунту " + existenceTime + ")";
			if (statistic2.SaldoTotal != 0.0)
			{
				history = history + Environment.NewLine + "Пополнения: " + statistic2.InputTotal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
				history = history + Environment.NewLine + "Выводы: " + statistic2.OutputTotal.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
				history = history + Environment.NewLine + ((statistic2.SaldoTotal > 0.0) ? "Проигрыш: " : "Выигрыш: ".ToUpper()) + Math.Abs(statistic2.SaldoTotal).ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
				if (statistic2.SaldoPeriod != 0.0)
				{
					history = history + Environment.NewLine + "      Активность за последние 3 месяца:";
					history = history + Environment.NewLine + "      Пополнения: " + statistic2.InputPeriod.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
					history = history + Environment.NewLine + "      Выводы: " + statistic2.OutputPeriod.ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
					history = history + Environment.NewLine + ((statistic2.SaldoPeriod > 0.0) ? "      Проигрыш: " : "      Выигрыш: ".ToUpper()) + Math.Abs(statistic2.SaldoPeriod).ToString("N0", CultureInfo.GetCultureInfo("ru-RU"));
					history += Environment.NewLine;
				}
				else
				{
					history = history + Environment.NewLine + "      Активность за последние 3 месяца отсутствует";
					history += Environment.NewLine;
				}
				history = history + Environment.NewLine + "Последнее пополнение: " + statistic2.lastTransaction.ToString("dd.MM.yyyy") + Environment.NewLine;
				if ((statistic2.BookmakerCompany == "БК «Лига Ставок»" && statistic2.InputPeriod > 50000.0 && statistic2.InputPeriod > 50000.0) || (statistic2.BookmakerCompany == "БК «OLIMPBET»" && statistic2.InputTotal > 30000.0 && statistic2.InputPeriod > 30000.0) || (statistic2.BookmakerCompany == "БК «ZENIT»" && statistic2.InputTotal > 50000.0 && statistic2.InputPeriod > 50000.0) || (statistic2.BookmakerCompany == "БК «БАЛТБЕТ»" && statistic2.InputTotal > 20000.0 && statistic2.InputPeriod > 20000.0) || (statistic2.BookmakerCompany == "БК «Леон»" && statistic2.InputTotal > 50000.0 && statistic2.InputPeriod > 50000.0) || (statistic2.BookmakerCompany == "БК «Winline»" && statistic2.InputTotal > 300000.0 && statistic2.InputPeriod > 300000.0) || ((statistic2.BookmakerCompany == "БК «PARI»" || statistic2.BookmakerCompany == "БК «FONBET»" || statistic2.BookmakerCompany == "БК «BETTERY»") && statistic2.InputTotal > 300000.0 && statistic2.InputPeriod > 200000.0))
				{
					contactSellAccouts = "https://t.me/guppy_bet";
				}
			}
			else
			{
				history = history + Environment.NewLine + "Оборот отсутствует";
				history += Environment.NewLine;
			}
		}
		history += Environment.NewLine;
		if (!string.IsNullOrEmpty(StatusIdentification))
		{
			history = history + Environment.NewLine + "Статус ЦУПИС: " + StatusIdentification;
		}
		history += Environment.NewLine;
		if (contactSellAccouts.Contains("guppy_bet"))
		{
			history = history + Environment.NewLine + "Вы можете сдать в аренду/продать свои аккаунты безопасно, напрямую, без посредников. Подробности узнавайте у " + contactSellAccouts;
		}
		return history;
	}
}
