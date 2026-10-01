using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace AccountsLibrary;

public class IssueTypeChoose : INotifyPropertyChanged
{
	private bool isChecked = true;

	public IssueType IssueType { get; set; }

	public bool IsChecked
	{
		get
		{
			return isChecked;
		}
		set
		{
			isChecked = value;
			RaisePropertyChanged("IsChecked");
		}
	}

	public event Action OnUpdate;

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		this.OnUpdate?.Invoke();
	}
}
