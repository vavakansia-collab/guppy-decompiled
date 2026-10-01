using System;

namespace CommonLibrary;

public class Country
{
	public Guid GUID { get; set; } = Guid.NewGuid();

	public string Name { get; set; }

	public string ShortName { get; set; }

	public string NameEng { get; set; }

	public int NumCode { get; set; }
}
