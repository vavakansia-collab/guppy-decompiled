using System.ComponentModel;
using System.Runtime.CompilerServices;
using AccountsLibrary;

namespace AccountsManager;

public class OwnerItem : INotifyPropertyChanged
{
	private bool isVisible;

	public OwnerAccount Owner { get; set; }

	public bool IsVisible
	{
		get
		{
			return isVisible;
		}
		set
		{
			isVisible = value;
			ViewModelMainWindow.AMSettings.VisibilityOwners[Owner.GUID.ToString()] = isVisible;
			RaisePropertyChanged("IsVisible");
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
