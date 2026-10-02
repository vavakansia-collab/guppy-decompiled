using System.Collections.ObjectModel;

namespace AccountsLibrary;

public static class CardBankBL
{
	public static ObservableCollection<CardBank> Cards { get; set; } = new ObservableCollection<CardBank>();
}
