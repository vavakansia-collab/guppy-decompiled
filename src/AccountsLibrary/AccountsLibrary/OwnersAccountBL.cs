using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AccountsLibrary;

public static class OwnersAccountBL
{
	private static object LockOwners = new object();

	public static ObservableCollection<OwnerAccount> OwnersAccount { get; set; } = new ObservableCollection<OwnerAccount>();

	public static OwnerAccount GetOwnerAccountByGuid(string strGuid)
	{
		return OwnersAccount.SingleOrDefault((OwnerAccount ow) => ow.GUID.ToString() == strGuid);
	}

	public static void UpdateOwners(List<OwnerAccount> owners)
	{
		lock (LockOwners)
		{
			owners.ForEach(delegate(OwnerAccount ow)
			{
				OwnersAccount.Remove(GetOwnerAccountByGuid(ow.GUID.ToString()));
			});
			owners.ForEach(delegate(OwnerAccount ow)
			{
				OwnersAccount.Add(ow);
			});
		}
	}
}
