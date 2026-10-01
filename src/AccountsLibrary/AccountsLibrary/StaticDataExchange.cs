using System.Collections.ObjectModel;
using CommonLibrary;

namespace AccountsLibrary;

public class StaticDataExchange
{
	public ObservableCollection<StatusAccount> StatusesAccount { get; set; }

	public ObservableCollection<Proxy> Proxies { get; set; }

	public ObservableCollection<Country> Countries { get; set; }

	public ObservableCollection<Currency> Currencies { get; set; }

	public ObservableCollection<OwnerAccount> OwnersAccounts { get; set; }

	public ObservableCollection<CardBank> CardsBank { get; set; }
}
