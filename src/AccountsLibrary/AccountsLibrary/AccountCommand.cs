using System;

namespace AccountsLibrary;

public class AccountCommand
{
	public DateTime DateTimeCreate = DateTime.Now;

	public string OwnerName;

	public Guid GuidAccount = Guid.Empty;

	public AccountCommandEnum Command;
}
