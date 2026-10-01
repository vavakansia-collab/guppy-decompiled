using System;
using System.ComponentModel;

namespace AccountsLibrary;

public class PaymentSystem : INotifyPropertyChanged
{
	private string imgSource;

	private string login = "";

	private string password = "";

	private string comment;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public string ImgSource
	{
		get
		{
			return imgSource;
		}
		set
		{
			imgSource = value;
			RaisePropertyChanged("ImgSource");
		}
	}

	public int IdPaymentSystem { get; set; } = 0;

	public DateTime DateTimeCreate { get; set; } = DateTime.Now;

	public string Login
	{
		get
		{
			return login;
		}
		set
		{
			if (login == null)
			{
				login = value;
			}
			RaisePropertyChanged("Login");
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
