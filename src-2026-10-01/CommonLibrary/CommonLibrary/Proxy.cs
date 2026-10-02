using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ProtoBuf;

namespace CommonLibrary;

public class Proxy : INotifyPropertyChanged
{
	public Guid GUID = Guid.NewGuid();

	private string nameProxy = "";

	public bool isActive = true;

	private string host = "";

	private int port;

	private string userName = "";

	private string password = "";

	private Country country;

	private string city;

	public bool IsEmpty { get; set; }

	public string NameProxy
	{
		get
		{
			return nameProxy;
		}
		set
		{
			if (!IsEmpty || !(value != "---без прокси---"))
			{
				nameProxy = value;
				RaisePropertyChanged("NameProxy");
			}
		}
	}

	public string Host
	{
		get
		{
			return host;
		}
		set
		{
			if (!IsEmpty)
			{
				host = value;
				ReFillWebProxy();
				RaisePropertyChanged("Host");
			}
		}
	}

	public int Port
	{
		get
		{
			return port;
		}
		set
		{
			if (!IsEmpty)
			{
				port = value;
				ReFillWebProxy();
				RaisePropertyChanged("Port");
			}
		}
	}

	public string UserName
	{
		get
		{
			return userName;
		}
		set
		{
			if (!IsEmpty)
			{
				userName = value;
				if (string.IsNullOrEmpty(nameProxy))
				{
					NameProxy = value;
				}
				ReFillWebProxy();
				RaisePropertyChanged("UserName");
			}
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
			if (!IsEmpty)
			{
				password = value;
				ReFillWebProxy();
				RaisePropertyChanged("Password");
			}
		}
	}

	[ProtoIgnore]
	[IgnoreDataMember]
	public WebProxy webProxy { get; set; } = new WebProxy();

	public DateTime DateTimeCreate { get; set; } = DateTime.Now;

	public DateTime DateTimeEnd { get; set; }

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

	public string City
	{
		get
		{
			return city;
		}
		set
		{
			city = value;
			RaisePropertyChanged("City");
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

	public Proxy(string initIP, int initPort, string initUserName, string initPassword)
	{
		Host = initIP;
		Port = initPort;
		UserName = initUserName;
		Password = initPassword;
		ReFillWebProxy();
	}

	public Proxy(string strProxy)
	{
		Match findRegex = new Regex("(?<ip>\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}):(?<port>\\d{1,5}):(?<username>\\S*):(?<password>\\w*)").Match(strProxy);
		Host = findRegex.Groups["ip"].Value.Trim();
		Port = Convert.ToInt32(findRegex.Groups["port"].Value.Trim());
		UserName = findRegex.Groups["username"].Value.Trim();
		Password = findRegex.Groups["password"].Value.Trim();
		ReFillWebProxy();
	}

	public Proxy()
	{
	}

	private void ReFillWebProxy()
	{
		if (Host != "" && Port != 0)
		{
			webProxy.Address = new Uri("http://" + Host + ":" + Port);
		}
		if (UserName != "" && Password != "")
		{
			webProxy.Credentials = new NetworkCredential(UserName, Password);
		}
	}

	public override string ToString()
	{
		if (IsEmpty)
		{
			return "---не указан---";
		}
		return Host + ":" + Port + ":" + UserName + ":" + Password;
	}
}
