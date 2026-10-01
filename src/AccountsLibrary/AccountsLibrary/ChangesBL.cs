using System;
using System.Threading;

namespace AccountsLibrary;

public static class ChangesBL
{
	public static Action OnSendChanges;

	private static object lockChange = new object();

	public static Changes SessionChanges = new Changes();

	public static event Action OnSendChangedStarted;

	public static event Action OnSendChangedComplite;

	public static event OnAccountSettingChanged OnChangesReceived;

	public static event OnRemoveAccountSettingChanged OnRemoveChangesReceived;

	public static void Start()
	{
		AccountsSettingsBL.OnAccountSettingChanged += AccountsSettingsBL_OnAccountSettingChanged;
		Thread thread = new Thread((ThreadStart)delegate
		{
			while (true)
			{
				Thread.Sleep(5000);
				SendChanges();
			}
		});
		thread.Name = "ChangesBL";
		thread.Start();
	}

	public static void AccountsSettingsBL_OnAccountSettingChanged(AccountSettings accountSettings)
	{
		if (accountSettings != null && !accountSettings.IsChanged)
		{
			accountSettings.IsChanged = true;
			SessionChanges.ChangedAccounts.Add(accountSettings);
		}
	}

	public static void RemoveAccount(AccountSettings accountSettings)
	{
		SessionChanges.RemovedAccounts.Add(accountSettings.GUID);
	}

	public static void ChangesReceived(Changes changes)
	{
		foreach (AccountSettings changedAccount in changes.ChangedAccounts)
		{
			AccountsSettingsBL.AddAccountSettings(changedAccount, isReceived: true);
			ChangesBL.OnChangesReceived?.Invoke(changedAccount);
		}
		foreach (Guid removedAccount in changes.RemovedAccounts)
		{
			AccountsSettingsBL.RemoveAccountSettings(removedAccount.ToString());
			ChangesBL.OnRemoveChangesReceived?.Invoke(removedAccount.ToString());
		}
	}

	public static void ClearChanges()
	{
		foreach (AccountSettings changedAccount in SessionChanges.ChangedAccounts)
		{
			changedAccount.IsChanged = false;
		}
		SessionChanges.ChangedAccounts.Clear();
		SessionChanges.RemovedAccounts.Clear();
	}

	public static void SendChanges()
	{
		lock (lockChange)
		{
			if (SessionChanges.ChangedAccounts.Count > 0 || SessionChanges.RemovedAccounts.Count > 0)
			{
				ChangesBL.OnSendChangedStarted?.Invoke();
				if (OnSendChanges != null)
				{
					OnSendChanges();
				}
				ChangesBL.OnSendChangedComplite?.Invoke();
			}
		}
	}
}
