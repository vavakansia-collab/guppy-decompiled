using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class StatusAccount : INotifyPropertyChanged
{
	private string name;

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

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public override string ToString()
	{
		return Name;
	}
}
