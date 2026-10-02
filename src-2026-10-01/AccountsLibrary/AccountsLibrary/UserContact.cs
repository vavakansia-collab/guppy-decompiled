using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class UserContact : INotifyPropertyChanged
{
	private string name;

	private string password;

	private string telegram = null;

	private string comment = null;

	private bool isBlocked = false;

	public Guid GUID { get; set; } = Guid.NewGuid();

	public DateTime TimeCreated { get; set; } = DateTime.Now;

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

	public string Telegram
	{
		get
		{
			return telegram;
		}
		set
		{
			telegram = value;
			RaisePropertyChanged("Telegram");
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

	public bool IsBlocked
	{
		get
		{
			return isBlocked;
		}
		set
		{
			isBlocked = value;
			RaisePropertyChanged("IsBlocked");
		}
	}

	public Dictionary<string, bool> AccessRights { get; set; } = new Dictionary<string, bool>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public void SetAccessRight(AccessUser accessUser, bool isAllowed)
	{
		AccessRights[accessUser.ToString()] = isAllowed;
	}

	public bool HasAccess(AccessUser accessUser)
	{
		bool isAllowed;
		return AccessRights.TryGetValue(accessUser.ToString(), out isAllowed) && isAllowed;
	}
}
