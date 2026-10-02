using System.ComponentModel;

namespace AccountsLibrary;

public class PhotoType : INotifyPropertyChanged
{
	private string sysName;

	private string displayName;

	public string SysName
	{
		get
		{
			return sysName;
		}
		set
		{
			sysName = value;
			RaisePropertyChanged("SysName");
		}
	}

	public string DisplayName
	{
		get
		{
			return displayName;
		}
		set
		{
			displayName = value;
			RaisePropertyChanged("DisplayName");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
