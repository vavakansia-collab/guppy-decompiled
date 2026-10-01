using System.Collections.Generic;

namespace BookmakerLibrary;

public class Event
{
	public int Id { get; set; }

	public string Name { get; set; }

	public int Period { get; set; }

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}
