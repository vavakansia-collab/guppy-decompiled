using System;
using System.ComponentModel;

namespace AccountsLibrary;

public class Passport : INotifyPropertyChanged
{
	private string name;

	private string surname;

	private string middleName;

	private string series;

	private string number;

	private DateTime dateOfBirth;

	private DateTime dateOfIssue;

	private string codeSubdivision;

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

	public string Surname
	{
		get
		{
			return surname;
		}
		set
		{
			surname = value;
			RaisePropertyChanged("Surname");
		}
	}

	public string MiddleName
	{
		get
		{
			return middleName;
		}
		set
		{
			middleName = value;
			RaisePropertyChanged("MiddleName");
		}
	}

	public string Series
	{
		get
		{
			return series;
		}
		set
		{
			series = value;
			RaisePropertyChanged("Series");
		}
	}

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

	public DateTime DateOfBirth
	{
		get
		{
			return dateOfBirth;
		}
		set
		{
			dateOfBirth = value;
			RaisePropertyChanged("DateOfBirth");
		}
	}

	public DateTime DateOfIssue
	{
		get
		{
			return dateOfIssue;
		}
		set
		{
			dateOfIssue = value;
			RaisePropertyChanged("DateOfIssue");
		}
	}

	public string CodeSubdivision
	{
		get
		{
			return codeSubdivision;
		}
		set
		{
			codeSubdivision = value;
			RaisePropertyChanged("CodeSubdivision");
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
