using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookmakerLibrary;

public class Bookmaker : INotifyPropertyChanged
{
	public string Name { get; set; }

	public string NameProvider { get; set; }

	public string NameCupis { get; set; }

	public string Host { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public Bookmaker(string initName, string initNameProvider, string initNameCupis, string initHost)
	{
		Name = initName;
		NameProvider = initNameProvider;
		NameCupis = initNameCupis;
		Host = initHost;
	}
}
