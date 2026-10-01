using System.Collections.Generic;

namespace AccountsLibrary;

public class ChatTelegram
{
	public long IdChat = 0L;

	public OwnerAccount User;

	public AccountSettings CurrentAccountSettings = null;

	public List<string> TasksStr = new List<string>();
}
