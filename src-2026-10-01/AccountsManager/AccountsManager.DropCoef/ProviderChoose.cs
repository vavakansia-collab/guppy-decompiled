using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace AccountsManager.DropCoef;

public class ProviderChoose : INotifyPropertyChanged
{
	private bool isSelected;

	public Provider Provider { get; set; }

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			RaisePropertyChanged("IsSelected");
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
