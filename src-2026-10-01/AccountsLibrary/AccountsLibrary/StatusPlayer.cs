using System;
using System.ComponentModel;

namespace AccountsLibrary;

public class StatusPlayer : INotifyPropertyChanged
{
	private string name;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
			RaisePropertyChanged("Name");
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
