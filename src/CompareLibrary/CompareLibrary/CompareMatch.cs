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
		string s = Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks;
		using MD5 mD = MD5.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		return BitConverter.ToInt32(mD.ComputeHash(bytes), 0) & 0x7FFFFFFF;
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
			CompareMatch compareMatch = new CompareMatch();
			compareMatch.TimeCreate = TimeCreate;
			compareMatch.IDSport = IDSport;
			compareMatch.InnerId = InnerId;
			compareMatch.Comment = Comment;
			foreach (CompareMatchParameter item in MatchesCMP)
			{
				CompareMatchParameter compareMatchParameter = new CompareMatchParameter();
				compareMatchParameter.DateTimeCreate = item.DateTimeCreate;
				compareMatchParameter.NameProvider = item.NameProvider;
				compareMatchParameter.IdMatch = item.IdMatch;
				compareMatchParameter.InnerId = item.InnerId;
				compareMatchParameter.IsOverturn = item.IsOverturn;
				compareMatchParameter.Comment = item.Comment;
				compareMatch.AddCMP(compareMatchParameter);
			}
			return compareMatch;
		}
	}

	public override string ToString()
	{
		return string.Empty;
	}
}
