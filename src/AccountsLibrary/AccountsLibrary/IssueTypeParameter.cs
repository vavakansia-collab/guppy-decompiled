using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace AccountsLibrary;

public class IssueTypeParameter : INotifyPropertyChanged
{
	private bool points0 = true;

	private bool points1 = true;

	private bool points2 = true;

	private bool points2Over = true;

	public IssueType IssueType { get; set; }

	public bool Points0
	{
		get
		{
			return points0;
		}
		set
		{
			points0 = value;
			RaisePropertyChanged("Points0");
		}
	}

	public bool Points1
	{
		get
		{
			return points1;
		}
		set
		{
			points1 = value;
			RaisePropertyChanged("Points1");
		}
	}

	public bool Points2
	{
		get
		{
			return points2;
		}
		set
		{
			points2 = value;
			RaisePropertyChanged("Points2");
		}
	}

	public bool Points2Over
	{
		get
		{
			return points2Over;
		}
		set
		{
			points2Over = value;
			RaisePropertyChanged("Points2Over");
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
