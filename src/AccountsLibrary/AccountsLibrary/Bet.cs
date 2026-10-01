using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BookmakerLibrary;

namespace AccountsLibrary;

public class Bet
{
	private double sum = 0.0;

	private StatusesBet status = StatusesBet.Create;

	private TypesCalculationBet typeCalculationBet = TypesCalculationBet.NotCalculation;

	[IgnoreDataMember]
	public OnProcessBetDelegate OnProcessBet;

	[IgnoreDataMember]
	public OnProcessBetDelegate OnCompliteBet;

	[IgnoreDataMember]
	public OnProcessBetDelegate OnChangeTypeCalculationBet;

	private static Random random { get; set; } = new Random();

	public Guid GUID { get; set; } = Guid.NewGuid();

	public TypeBet TypeBet { get; set; }

	public double BalanceBefore { get; set; } = 0.0;

	public Coupon Coupon { get; set; }

	public string NameProvider { get; set; }

	public long IdMatch { get; set; }

	public string NameMatch { get; set; }

	public double MaxSum { get; set; } = 0.0;

	public string MaxBetResponce { get; set; }

	public string Responce { get; set; }

	public DateTime TimeBegin { get; set; } = DateTime.Now;

	public DateTime TimeEnd { get; set; }

	public double TimeResponce { get; set; } = 0.0;

	public double Sum
	{
		get
		{
			return sum;
		}
		set
		{
			sum = value;
			if (sum > 0.0 && Coupon != null && Coupon.PayoutRatio > 0.0)
			{
				Profit = Math.Floor(Coupon.PayoutRatio * sum);
			}
		}
	}

	public double Profit { get; private set; }

	public bool Success { get; set; } = false;

	public TypeChangePayoutRatio TypeChangePayoutRatio { get; set; } = TypeChangePayoutRatio.All;

	public TypesLine TypeLine { get; set; } = TypesLine.Live;

	public string Description { get; set; }

	public bool ApplyChangeCouponValue { get; set; } = false;

	public StatusesBet Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
			if (status == StatusesBet.InProcess)
			{
				OnProcessBet?.Invoke();
			}
			else if (status == StatusesBet.Cancel || status == StatusesBet.Complite)
			{
				OnCompliteBet?.Invoke();
			}
		}
	}

	public TypesCalculationBet TypeCalculationBet
	{
		get
		{
			return typeCalculationBet;
		}
		set
		{
			typeCalculationBet = value;
			OnChangeTypeCalculationBet?.Invoke();
		}
	}

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

	public Bet()
	{
	}

	public Bet(string initNameProvider, long initIdMatch, TypeBet initTypeBet, TypesLine initTypesLine = TypesLine.Live)
	{
		NameProvider = initNameProvider;
		IdMatch = initIdMatch;
		TypeBet = initTypeBet;
		TypeLine = initTypesLine;
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
}
