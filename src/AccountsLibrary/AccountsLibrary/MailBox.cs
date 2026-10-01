using System;
using System.ComponentModel;

namespace AccountsLibrary;

public class MailBox : INotifyPropertyChanged
{
	private string name;

	private string password = "";

	private string comment = "";

	public Guid GUID { get; set; } = Guid.NewGuid();

	public DateTime DateTimeCreate { get; set; } = DateTime.Now;

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

	public string Password
	{
		get
		{
			return password;
		}
		set
		{
			password = value;
			RaisePropertyChanged("Password");
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

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
