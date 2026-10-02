using System.ComponentModel;
using System.Runtime.CompilerServices;
using AccountsLibrary;
using CommonLibrary;

namespace AccountsManager;

public class ViewModelAuth : INotifyPropertyChanged
{
	public static ViewModelAuth instance;

	private bool isConnectServer;

	private string authResponce;

	private bool isAuthorized;

	private RelayCommand authCommand;

	public static AMSettings AMSettings { get; set; }

	public bool IsConnectServer
	{
		get
		{
			return isConnectServer;
		}
		set
		{
			isConnectServer = value;
			RaisePropertyChanged("IsConnectServer");
		}
	}

	public string AuthResponce
	{
		get
		{
			return authResponce;
		}
		set
		{
			authResponce = value;
			RaisePropertyChanged("AuthResponce");
		}
	}

	public bool IsAuthorized
	{
		get
		{
			return isAuthorized;
		}
		set
		{
			isAuthorized = value;
			RaisePropertyChanged("IsAuthorized");
		}
	}

	public RelayCommand AuthCommand => authCommand ?? (authCommand = new RelayCommand(delegate
	{
		Auth();
	}));

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public ViewModelAuth()
	{
		Settings.ViewModelAuth = this;
		instance = this;
	}

	private async void Auth()
	{
		if (string.IsNullOrEmpty(AMSettings.LastLogin))
		{
			AuthResponce = "Не указан логин";
			return;
		}
		AuthResponce = "";
		(OwnerAccount, string) obj = await AccountsCenterClient.Auth(AMSettings.LastLogin, AMSettings.LastPassword);
		OwnerAccount ownerAccounts = obj.Item1;
		string responce = obj.Item2;
		AuthResponce = responce;
		if (ownerAccounts != null)
		{
			Settings.AMSettings.OwnerAccounts = ownerAccounts;
			WindowAccountsManager.instance.Show();
			Settings.AuthDialogWindow?.Close();
			IsAuthorized = true;
			ViewModelMainWindow.instance.Run();
		}
	}

	public void OnServerConnected()
	{
		IsConnectServer = true;
	}
}
