using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using BookmakerLibrary;

namespace AccountsLibrary;

public class ParametersSport : INotifyPropertyChanged
{
	private int idSport;

	[IgnoreDataMember]
	private Sport sport;

	private bool isChecked = true;

	public int IdSport
	{
		get
		{
			return idSport;
		}
		set
		{
			idSport = value;
			sport = SportBL.GetSportByID(idSport);
			RaisePropertyChanged("Sport");
		}
	}

	[IgnoreDataMember]
	public Sport Sport => sport;

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

	public void RaisePropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		this.OnUpdate?.Invoke();
	}

	public override string ToString()
	{
		return SportBL.GetSportByID(IdSport).FullName + " " + IsChecked;
	}
}
