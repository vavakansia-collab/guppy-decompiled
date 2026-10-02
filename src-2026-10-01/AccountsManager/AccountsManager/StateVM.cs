using System.ComponentModel;
using System.Runtime.CompilerServices;
using AccountsLibrary;

namespace AccountsManager;

public class StateVM : INotifyPropertyChanged
{
	private bool isReaded;

	public StateAccount State { get; set; }

	public bool IsReaded
	{
		get
		{
			return isReaded;
		}
		set
		{
			isReaded = value;
			RaisePropertyChanged("IsReaded");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
