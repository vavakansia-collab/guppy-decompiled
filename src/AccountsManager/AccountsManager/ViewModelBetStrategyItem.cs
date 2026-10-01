using System.ComponentModel;
using System.Runtime.CompilerServices;
using AccountsLibrary;

namespace AccountsManager;

public class ViewModelBetStrategyItem : INotifyPropertyChanged
{
	public BetStrategy BetStrategy { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public ViewModelBetStrategyItem(BetStrategy initBetStrategy)
	{
		BetStrategy = initBetStrategy;
	}
}
