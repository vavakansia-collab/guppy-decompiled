using System;

namespace AccountsLibrary;

public class StatusUserChanged
{
	public DateTime DateTimeChange { get; set; } = DateTime.Now;

	public StatusObject StatusUser { get; set; }
}
