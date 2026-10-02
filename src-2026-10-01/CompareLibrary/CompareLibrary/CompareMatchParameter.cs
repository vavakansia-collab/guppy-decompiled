using System;
using System.ComponentModel;

namespace CompareLibrary;

public class CompareMatchParameter : INotifyPropertyChanged
{
	public DateTime DateTimeCreate = DateTime.Now;

	private int innerId;

	private bool isOverturn;

	public string NameMatch { get; set; }

	public string NameProvider { get; set; }

	public long IdMatch { get; set; }

	public int InnerId
	{
		get
		{
			return innerId;
		}
		set
		{
			innerId = value;
			RaisePropertyChanged("InnerId");
		}
	}

	public bool IsOverturn
	{
		get
		{
			return isOverturn;
		}
		set
		{
			isOverturn = value;
			RaisePropertyChanged("IsOverturn");
		}
	}

	public string Comment { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public CompareMatchParameter()
	{
	}

	public CompareMatchParameter(string initNameProvider, string initMatchName, long initIdMatch)
	{
		NameProvider = initNameProvider;
		NameMatch = initMatchName;
		IdMatch = initIdMatch;
	}
}
