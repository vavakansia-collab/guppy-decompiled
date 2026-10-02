using System;
using System.Collections.Generic;
using CommonLibrary;

namespace SnapshotsLibrary;

public class Snapshot
{
	public DateTime StartTime { get; set; } = CommonStaticFunction.GetMoscowTime();

	public string FileName { get; set; }

	public List<Bookmaker> Bookmakers { get; set; } = new List<Bookmaker>();
}
