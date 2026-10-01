using System;
using System.Collections.Generic;
using BookmakerLibrary;

namespace SnapshotsViewer;

public class TimeLine
{
	public DateTime Timestamp { get; set; }

	public int Period { get; set; }

	public List<TimestampProvider> TimestampProviders { get; set; } = new List<TimestampProvider>();

	public TimeLine()
	{
		ProvidersCommon.ProvidersNames.ForEach(delegate(string pr)
		{
			TimestampProviders.Add(new TimestampProvider
			{
				NameProvider = pr
			});
		});
	}
}
