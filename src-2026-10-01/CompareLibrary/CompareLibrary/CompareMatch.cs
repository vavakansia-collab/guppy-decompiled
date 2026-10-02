using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace CompareLibrary;

public class CompareMatch
{
	[IgnoreDataMember]
	public object thisLock = new object();

	public DateTime TimeCreate { get; set; } = DateTime.Now;

	public int InnerId { get; set; }

	public int IDSport { get; set; }

	public List<CompareMatchParameter> MatchesCMP { get; set; } = new List<CompareMatchParameter>();

	public string Comment { get; set; } = "";

	public CompareMatch()
	{
	}

	public CompareMatch(int _idSport)
	{
		IDSport = _idSport;
		InnerId = GenerateUniqueId();
	}

	public int GenerateUniqueId()
	{
		string compositeKey = Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks;
		using MD5 hash = MD5.Create();
		byte[] inputBytes = Encoding.UTF8.GetBytes(compositeKey);
		return BitConverter.ToInt32(hash.ComputeHash(inputBytes), 0) & 0x7FFFFFFF;
	}

	public int _GenerateUniqueId()
	{
		return BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0) & 0x7FFFFFFF;
	}

	public void AddCMP(CompareMatchParameter compareMatchParameter)
	{
		lock (thisLock)
		{
			if (MatchesCMP.SingleOrDefault((CompareMatchParameter cmp) => cmp.NameProvider == compareMatchParameter.NameProvider) == null)
			{
				MatchesCMP.Add(compareMatchParameter);
			}
		}
	}

	public void RemoveCMP(string nameProvider, long idMatch)
	{
		lock (thisLock)
		{
			CompareMatchParameter compareMatchParameter = MatchesCMP.SingleOrDefault((CompareMatchParameter cmp) => cmp.NameProvider == nameProvider && cmp.IdMatch == idMatch);
			if (compareMatchParameter != null)
			{
				MatchesCMP.Remove(compareMatchParameter);
			}
		}
	}

	public void RemoveCMP(CompareMatchParameter cmp)
	{
		lock (thisLock)
		{
			CompareMatchParameter compareMatchParameter = MatchesCMP.SingleOrDefault((CompareMatchParameter c) => c.NameProvider == cmp.NameProvider && c.IdMatch == cmp.IdMatch);
			if (compareMatchParameter != null)
			{
				MatchesCMP.Remove(compareMatchParameter);
			}
		}
	}

	public CompareMatch Copy()
	{
		lock (thisLock)
		{
			CompareMatch newCompareMatch = new CompareMatch();
			newCompareMatch.TimeCreate = TimeCreate;
			newCompareMatch.IDSport = IDSport;
			newCompareMatch.InnerId = InnerId;
			newCompareMatch.Comment = Comment;
			foreach (CompareMatchParameter cmp in MatchesCMP)
			{
				CompareMatchParameter newCMP = new CompareMatchParameter();
				newCMP.DateTimeCreate = cmp.DateTimeCreate;
				newCMP.NameProvider = cmp.NameProvider;
				newCMP.IdMatch = cmp.IdMatch;
				newCMP.InnerId = cmp.InnerId;
				newCMP.IsOverturn = cmp.IsOverturn;
				newCMP.Comment = cmp.Comment;
				newCompareMatch.AddCMP(newCMP);
			}
			return newCompareMatch;
		}
	}

	public override string ToString()
	{
		return string.Empty;
	}
}
