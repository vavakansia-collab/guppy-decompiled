using System;

namespace AccountsLibrary;

public class Currency
{
	public Guid Guid { get; set; } = Guid.NewGuid();

	public string Name { get; set; }

	public int NumCode { get; set; }

	public string ShortName { get; set; }

	public string NameEng { get; set; }

	public double Exchange { get; set; }

	public string Symbol { get; set; }
}
