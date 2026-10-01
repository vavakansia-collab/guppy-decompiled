using System.Collections.ObjectModel;

namespace AccountsLibrary.CUPIS;

public class CupisDatabase
{
	public ObservableCollection<CupisClient> Clients { get; set; } = new ObservableCollection<CupisClient>();
}
