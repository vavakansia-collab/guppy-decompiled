using System;
using System.Collections.Generic;

namespace AccountsLibrary;

public class Changes
{
	public List<AccountSettings> ChangedAccounts = new List<AccountSettings>();

	public List<Guid> RemovedAccounts = new List<Guid>();
}
