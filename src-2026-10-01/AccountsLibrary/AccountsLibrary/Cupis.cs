using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class Cupis : INotifyPropertyChanged
{
	public delegate void OnUpdateHandler();

	public delegate void OnTextFieldChangedHandler(string oldValue, string newValue);

	private string login;

	public string Login
	{
		get
		{
			return login;
		}
		set
		{
			string oldValue = login;
			if (value != null)
			{
				value = value.Replace(" ", "").Replace("-", "").Replace("(", "")
					.Replace(")", "");
			}
			login = value;
			if (!string.IsNullOrEmpty(login) && login.Length == 11 && (login.Remove(2) == "89" || login.Remove(2) == "79"))
			{
				login = "+7" + value.Remove(0, 1);
			}
			this.OnUpdate?.Invoke();
			this.OnCupisLoginChanged?.Invoke(oldValue, login);
			RaisePropertyChanged("Login");
		}
	}

	public event OnUpdateHandler OnUpdate;

	public event OnTextFieldChangedHandler OnCupisLoginChanged;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
