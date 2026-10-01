using System;

namespace AccountsLibrary;

public class StatusWorking
{
	public Guid GUID { get; set; } = Guid.NewGuid();

	public int Index { get; set; }

	public string Name { get; set; }
}
