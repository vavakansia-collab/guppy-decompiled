using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AccountsLibrary;

public class CardBank : INotifyPropertyChanged
{
	private long number;

	public EnumBanks Bank { get; set; }

	public long Number
	{
		get
		{
			return number;
		}
		set
		{
			if (value.ToString().Replace(" ", "").Length <= 16)
			{
				number = value;
			}
			RaisePropertyChanged("Number");
		}
	}

	public string Period { get; set; }

	public string CVV { get; set; }

	public string Description { get; set; } = "";

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
