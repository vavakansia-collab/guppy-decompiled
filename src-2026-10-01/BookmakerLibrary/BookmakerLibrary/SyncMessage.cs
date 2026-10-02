using System;
using System.Collections.Generic;

namespace BookmakerLibrary;

public class SyncMessage
{
	public SyncMode Mode { get; set; }

	public long Sequence { get; set; }

	public DateTime SentAtUtc { get; set; }

	public string NameProvider { get; set; }

	public List<Match> FullData { get; set; }

	public List<MatchDeltaMessage> Deltas { get; set; }
}
