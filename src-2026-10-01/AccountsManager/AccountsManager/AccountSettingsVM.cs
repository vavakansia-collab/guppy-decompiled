using System.ComponentModel;
using System.Runtime.CompilerServices;
using AccountsLibrary;

namespace AccountsManager;

public class AccountSettingsVM : INotifyPropertyChanged
{
	public AccountSettings AccountSettings { get; }

	public StateVM StateVM { get; set; } = new StateVM();

	public event PropertyChangedEventHandler PropertyChanged;

	public AccountSettingsVM(AccountSettings initAccountSettings)
	{
		AccountSettings = initAccountSettings;
		StateVM.State = new StateAccount(AccountSettings.GUID);
	}

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
