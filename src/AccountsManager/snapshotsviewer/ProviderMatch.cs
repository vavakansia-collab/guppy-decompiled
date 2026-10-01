using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnapshotsViewer;

public class ProviderMatch : INotifyPropertyChanged
{
	private bool isChoose = true;

	public string Name { get; set; }

	public bool IsChoose
	{
		get
		{
			return isChoose;
		}
		set
		{
			isChoose = value;
			RaisePropertyChanged("IsChoose");
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
