using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary.CUPIS;

public class CupisClient : INotifyPropertyChanged
{
	public string cupisLogin;

	private bool isAvability = true;

	public int totalRequests = 0;

	public int totalCheck = 0;

	public DateTime lastTimeRequest = DateTime.Now;

	private string lastRequest;

	private int totalLogs = 0;

	private bool isWarningSubscribeGroup = false;

	public long IdChat { get; set; } = 0L;

	public string UserName { get; set; }

	public string FirstName { get; set; }

	public string LastName { get; set; }

	public string CupisLogin
	{
		get
		{
			return cupisLogin;
		}
		set
		{
			cupisLogin = value;
			if (cupisLogin != null)
			{
				cupisLogin = cupisLogin.Replace("(", "").Replace(")", "").Replace("-", "")
					.Replace(" ", "");
			}
			if (!string.IsNullOrEmpty(value) && value.Length == 11 && (value.Remove(2) == "89" || value.Remove(2) == "79"))
			{
				cupisLogin = "+7" + value.Remove(0, 1);
			}
		}
	}

	public string CupisPassword { get; set; }

	public bool IsAvability
	{
		get
		{
			return isAvability;
		}
		set
		{
			isAvability = value;
			RaisePropertyChanged("IsAvability");
		}
	}

	public int TotalRequests
	{
		get
		{
			return totalRequests;
		}
		set
		{
			totalRequests = value;
			RaisePropertyChanged("TotalRequests");
		}
	}

	public int TotalCheck
	{
		get
		{
			return totalCheck;
		}
		set
		{
			totalCheck = value;
			RaisePropertyChanged("TotalCheck");
		}
	}

	public List<CupisResponse> CupisResponses { get; set; } = new List<CupisResponse>();

	public List<string> TasksStr { get; set; } = new List<string>();

	public DateTime LastTimeRequest
	{
		get
		{
			return lastTimeRequest;
		}
		set
		{
			lastTimeRequest = value;
			RaisePropertyChanged("LastTimeRequest");
		}
	}

	public DateTime LastSuccessCheck { get; set; }

	public string LastRequest
	{
		get
		{
			return lastRequest;
		}
		set
		{
			lastRequest = value;
			RaisePropertyChanged("LastRequest");
		}
	}

	public int TotalLogs
	{
		get
		{
			return totalLogs;
		}
		set
		{
			totalLogs = value;
			RaisePropertyChanged("TotalLogs");
		}
	}

	public bool IsWarningSubscribeGroup
	{
		get
		{
			return isWarningSubscribeGroup;
		}
		set
		{
			isWarningSubscribeGroup = value;
			RaisePropertyChanged("IsWarningSubscribeGroup");
		}
	}

	public ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

	public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
