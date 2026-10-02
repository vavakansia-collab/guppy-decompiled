using System.Collections.Generic;
using AccountsLibrary;

namespace AccountsManager;

internal class StrategisDTO
{
	public List<PoslegolSettings> PoslegolsSettings { get; set; } = new List<PoslegolSettings>();

	public List<ValueSettings> ValuesSettings { get; set; } = new List<ValueSettings>();

	public List<ForkSettings> ForksSettings { get; set; } = new List<ForkSettings>();

	public List<ValuesPlusSettings> ValuesPlusSettings { get; set; } = new List<ValuesPlusSettings>();

	public List<ValueScoreSettings> ValuesScoreSettings { get; set; } = new List<ValueScoreSettings>();
}
