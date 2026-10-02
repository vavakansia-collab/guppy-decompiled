using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookmakerLibrary;

namespace AccountsManager.DropCoef;

public class MatchLineVM : INotifyPropertyChanged
{
	private bool isFavorite;

	private double deviationPercent;

	private double maxDeviationPercent;

	private double volume;

	private string score = "-";

	public MatchLine MatchLine { get; set; }

	public bool IsFavorite
	{
		get
		{
			return isFavorite;
		}
		set
		{
			isFavorite = value;
			RaisePropertyChanged("IsFavorite");
		}
	}

	public double DeviationPercent
	{
		get
		{
			return deviationPercent;
		}
		set
		{
			deviationPercent = value;
			RaisePropertyChanged("DeviationPercent");
		}
	}

	public double MaxDeviationPercent
	{
		get
		{
			return maxDeviationPercent;
		}
		set
		{
			maxDeviationPercent = value;
			RaisePropertyChanged("MaxDeviationPercent");
		}
	}

	public double Volume
	{
		get
		{
			return volume;
		}
		set
		{
			volume = value;
			RaisePropertyChanged("Volume");
		}
	}

	public string Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
			RaisePropertyChanged("Score");
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
}
