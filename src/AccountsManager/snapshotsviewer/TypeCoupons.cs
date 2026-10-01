using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace SnapshotsViewer;

public class TypeCoupons : INotifyPropertyChanged
{
	private bool isChoose = true;

	public IssueType IssueType { get; set; }

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
