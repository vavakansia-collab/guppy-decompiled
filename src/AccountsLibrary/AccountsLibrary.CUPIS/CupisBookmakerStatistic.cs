using System;

namespace AccountsLibrary.CUPIS;

public class CupisBookmakerStatistic
{
	public bool isHasAccount;

	public DateTime firstTransaction;

	public DateTime lastTransaction;

	public string BookmakerCompany { get; set; }

	public double InputTotal { get; set; } = 0.0;

	public double OutputTotal { get; set; } = 0.0;

	public double SaldoTotal => InputTotal - OutputTotal;

	public double InputPeriod { get; set; } = 0.0;

	public double OutputPeriod { get; set; } = 0.0;

	public double SaldoPeriod => InputPeriod - OutputPeriod;

	public string CalcualteAccountCost()
	{
		double num = 0.0;
		if (SaldoPeriod >= 50000.0 && SaldoPeriod < 100000.0)
		{
			num = 500.0;
		}
		else if (SaldoPeriod >= 100000.0 && SaldoPeriod < 150000.0)
		{
			num = 1000.0;
		}
		else if (SaldoPeriod >= 150000.0 && SaldoPeriod < 200000.0)
		{
			num = 1500.0;
		}
		else if (SaldoPeriod >= 200000.0 && SaldoPeriod < 250000.0)
		{
			num = 2000.0;
		}
		else if (SaldoPeriod >= 250000.0 && SaldoPeriod < 300000.0)
		{
			num = 2500.0;
		}
		else if (SaldoPeriod >= 300000.0)
		{
			num = 3000.0;
		}
		if (BookmakerCompany == "БК «Лига Ставок»" || BookmakerCompany == "БК «БАЛТБЕТ»")
		{
			if (SaldoPeriod >= 500000.0)
			{
				num = 5000.0;
			}
			if (SaldoPeriod >= 700000.0)
			{
				num = 7000.0;
			}
			if (SaldoPeriod >= 1000000.0)
			{
				num = 10000.0;
			}
		}
		if (BookmakerCompany == "БК «1хСтавка»" || BookmakerCompany == "БК «БЕТСИТИ»" || BookmakerCompany == "БК «Марафон»")
		{
			num = 0.0;
		}
		if (num == 0.0)
		{
			return "-";
		}
		if (BookmakerCompany == "БК «БАЛТБЕТ»")
		{
			num += 1000.0;
		}
		else if (BookmakerCompany == "БК «OLIMPBET»" || BookmakerCompany == "БК «Зенит»")
		{
			num += 500.0;
		}
		else if (BookmakerCompany == "БК «Лига Ставок»")
		{
			num += 1000.0;
		}
		else if (BookmakerCompany == "БК «FON.BET»" || BookmakerCompany == "БК «FONBET»")
		{
			num -= 1000.0;
		}
		else if (BookmakerCompany == "БК «Winline»")
		{
			num -= 1000.0;
		}
		return (num > 0.0) ? (num + "р. в день") : "-";
	}
}
