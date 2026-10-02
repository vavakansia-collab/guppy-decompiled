using System;
using System.ComponentModel;
using CommonLibrary;

namespace AccountsLibrary;

public class SimCard : INotifyPropertyChanged
{
	private string number = "";

	private UserContact owner;

	private DateTime dateTimeCreateSystem = DateTime.Now;

	private DateTime dateTimeCreate;

	private string comment = "";

	private SimOperator simOperator;

	private Country country;

	private int ii;

	private StatusReg statusReg;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public string Number
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
			RaisePropertyChanged("Number");
		}
	}

	public UserContact Owner
	{
		get
		{
			return owner;
		}
		set
		{
			owner = value;
			RaisePropertyChanged("Owner");
		}
	}

	public DateTime DateTimeCreateSystem
	{
		get
		{
			return dateTimeCreateSystem;
		}
		set
		{
			dateTimeCreateSystem = value;
			RaisePropertyChanged("DateTimeCreateSystem");
		}
	}

	public DateTime DateTimeCreate
	{
		get
		{
			return dateTimeCreate;
		}
		set
		{
			dateTimeCreate = value;
			RaisePropertyChanged("DateTimeCreate");
		}
	}

	public string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			comment = value;
			RaisePropertyChanged("Comment");
		}
	}

	public SimOperator SimOperator
	{
		get
		{
			return simOperator;
		}
		set
		{
			simOperator = value;
			RaisePropertyChanged("SimOperator");
		}
	}

	public Country Country
	{
		get
		{
			return country;
		}
		set
		{
			country = value;
			RaisePropertyChanged("Country");
		}
	}

	public int II
	{
		get
		{
			return ii;
		}
		set
		{
			ii = value;
			RaisePropertyChanged("II");
		}
	}

	public StatusReg StatusReg
	{
		get
		{
			return statusReg;
		}
		set
		{
			statusReg = value;
			RaisePropertyChanged("StatusReg");
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
