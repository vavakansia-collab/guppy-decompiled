using System.Collections.ObjectModel;
using System.Linq;
using CommonLibrary;

namespace AccountsLibrary;

public static class ProxyBL
{
	public static ObservableCollection<Proxy> ListProxy { get; set; } = new ObservableCollection<Proxy>();

	public static void CheckExistProxy()
	{
		if (ListProxy.SingleOrDefault((Proxy pr) => pr.IsEmpty) == null)
		{
			ListProxy.Add(new Proxy
			{
				IsEmpty = true,
				NameProxy = "--без прокси--"
			});
		}
	}
}
