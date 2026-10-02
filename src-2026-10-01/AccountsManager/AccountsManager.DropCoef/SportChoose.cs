using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace AccountsManager.DropCoef;

public class SportChoose : INotifyPropertyChanged
{
	private bool isSelected;

	private int totalSelected;

	public Sport Sport { get; set; }

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

	public int TotalSelected
	{
		get
		{
			return totalSelected;
		}
		set
		{
			totalSelected = value;
			RaisePropertyChanged("TotalSelected");
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
