using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BookmakerLibrary;

namespace AccountsLibrary;

public static class AccountsSettingsBL
{
	public static List<string> RegOperators;

	public static object thislock { get; set; }

	public static ObservableCollection<AccountSettings> AccountsSettings { get; set; }

	public static ObservableCollection<StatusAccount> Statuses { get; set; }

	public static event OnAccountSettingChanged OnAccountSettingChanged;

	public static event OnAccountSettingChanged OnUpdateStatusRegistration;

	static AccountsSettingsBL()
	{
		thislock = new object();
		AccountsSettings = new ObservableCollection<AccountSettings>();
		Statuses = new ObservableCollection<StatusAccount>();
		RegOperators = new List<string> { "не указан", "ЦУПИС" };
	}

	public static StatusAccount GetStatusAccount(string status)
	{
		return Statuses.SingleOrDefault((StatusAccount st) => st.Name == status);
	}

	public static void Update(List<AccountSettings> newCollection)
	{
		lock (thislock)
		{
			AccountsSettings.Clear();
			foreach (AccountSettings item in newCollection)
			{
				AddAccountSettings(item);
			}
		}
	}

	public static void ClearAccountsSettings()
	{
		lock (thislock)
		{
			AccountsSettings.Clear();
		}
	}

	public static void AddAccountSettings(AccountSettings accountSettings, bool isReceived = false)
	{
		RemoveAccountSettings(accountSettings.GUID.ToString());
		if (accountSettings.OwnerAccount == null)
		{
			return;
		}
		accountSettings.OwnerAccount = OwnersAccountBL.OwnersAccount.FirstOrDefault((OwnerAccount ow) => ow.GUID == accountSettings.OwnerAccount.GUID);
		if (accountSettings.Currency != null)
		{
			accountSettings.Currency = CurrencyBL.Currencies.FirstOrDefault((Currency st) => st.NumCode == accountSettings.Currency.NumCode);
		}
		SubscribeAccount(accountSettings);
		lock (thislock)
		{
			AccountsSettings.Add(accountSettings);
		}
		if (!isReceived)
		{
			AccountSettings_OnUpdateAccount(accountSettings);
		}
	}

	public static void RemoveAccountSettings(string guid)
	{
		AccountSettings accountSettings = AccountsSettings.SingleOrDefault((AccountSettings ac) => ac.GUID.ToString() == guid);
		if (accountSettings != null)
		{
			RemoveAccountSettings(accountSettings);
		}
	}

	public static void RemoveAccountSettings(AccountSettings accountSettings)
	{
		UnsubscribeAccount(accountSettings);
		AccountsSettings.Remove(AccountsSettings.SingleOrDefault((AccountSettings ac) => ac.GUID == accountSettings.GUID));
	}

	private static void AccountSettings_OnUpdateAccount(AccountSettings accountSettings)
	{
		AccountsSettingsBL.OnAccountSettingChanged?.Invoke(accountSettings);
	}

	public static void SubscribeAccount(AccountSettings accountSettings)
	{
		accountSettings.OnUpdateAccount += AccountSettings_OnUpdateAccount;
		accountSettings.OnUpdateStatusRegistration += AccountSettings_OnUpdateStatusRegistration;
		accountSettings.OnSubscribeSettings();
		foreach (string item in ProvidersCommon.ProvidersNames.Where((string pn) => pn != accountSettings.NameProvider && !pn.EndsWith("Line")))
		{
			foreach (PoslegolSettings poslegolsSetting in accountSettings.PoslegolsSettings)
			{
				CheckExistProvider(accountSettings, poslegolsSetting, item);
			}
			foreach (ValueSettings valuesSetting in accountSettings.ValuesSettings)
			{
				CheckExistProvider(accountSettings, valuesSetting, item);
			}
			foreach (ForkSettings forksSetting in accountSettings.ForksSettings)
			{
				CheckExistProvider(accountSettings, forksSetting, item);
			}
			foreach (ValuesPlusSettings valuesPlusSetting in accountSettings.ValuesPlusSettings)
			{
				CheckExistProvider(accountSettings, valuesPlusSetting, item);
			}
			foreach (ValueScoreSettings valuesScoreSetting in accountSettings.ValuesScoreSettings)
			{
				CheckExistProvider(accountSettings, valuesScoreSetting, item);
			}
		}
		void CheckExistProvider(AccountSettings _accountSettings, StrategySettingsBase settings, string nameProvider)
		{
			if (settings.ParametersCheckBC.SingleOrDefault((ParameterAccount p) => p.NameParameter == nameProvider) == null)
			{
				settings.AddParameterCheckBC(new ParameterAccount
				{
					NameParameter = nameProvider,
					IsChecked = false
				});
				AccountSettings_OnUpdateAccount(accountSettings);
			}
		}
	}

	private static void AccountSettings_OnUpdateStatusRegistration(AccountSettings accountSettings)
	{
		AccountsSettingsBL.OnUpdateStatusRegistration(accountSettings);
	}

	public static void UnsubscribeAccount(AccountSettings accountSettings)
	{
		accountSettings.OnUpdateAccount -= AccountSettings_OnUpdateAccount;
		accountSettings.OnUpdateStatusRegistration -= AccountSettings_OnUpdateStatusRegistration;
		accountSettings.OnUnsubscribeSettings();
	}
}
