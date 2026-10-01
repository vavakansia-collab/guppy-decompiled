using System.Collections.Generic;

namespace BookmakerLibrary;

public class Championat
{
	public int Id { get; set; }

	public string Name { get; set; }

	public int IdSport { get; set; }

	public string NameSport { get; set; }

	public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}
