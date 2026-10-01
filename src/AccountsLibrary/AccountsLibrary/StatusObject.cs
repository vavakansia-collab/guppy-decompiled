using System.ComponentModel;

namespace AccountsLibrary;

public class StatusObject : INotifyPropertyChanged
{
	private string name = null;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
			RaisePropertyChanged("name");
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
