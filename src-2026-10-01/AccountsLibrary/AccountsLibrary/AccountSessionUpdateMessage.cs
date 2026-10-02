using System;
using System.Collections.Generic;

namespace AccountsLibrary;

public class AccountSessionUpdateMessage
{
	public Guid GUIDAccount { get; set; }

	public double Balance { get; set; }

	public bool IsRunning { get; set; }

	public string StatusService { get; set; }

	public List<BetStrategy> Bets { get; set; } = new List<BetStrategy>();

	public List<string> Logs { get; set; } = new List<string>();
}
