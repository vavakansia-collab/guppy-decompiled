using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class ParameterAccount : INotifyPropertyChanged
{
	private string nameParameter;

	private string valueParameter;

	private bool isNecessary = false;

	private bool isChecked = false;

	public string NameParameter
	{
		get
		{
			return nameParameter;
		}
		set
		{
			nameParameter = value;
			RaisePropertyChanged("NameParameter");
		}
	}

	public string ValueParameter
	{
		get
		{
			return valueParameter;
		}
		set
		{
			valueParameter = value;
			RaisePropertyChanged("ValueParameter");
		}
	}

	public bool IsNecessary
	{
		get
		{
			return isNecessary;
		}
		set
		{
			isNecessary = value;
			RaisePropertyChanged("IsNecessary");
		}
	}

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

	public override string ToString()
	{
		return NameParameter;
	}
}
