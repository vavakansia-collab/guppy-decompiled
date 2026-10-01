using System.Collections.Generic;

namespace BookmakerLibrary;

public class MatchDeltaMessage
{
	public string IdMatch { get; set; }

	public bool IsNew { get; set; }

	public Dictionary<string, object> Changes { get; set; } = new Dictionary<string, object>();
}
