using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AccountsLibrary;

public class Player : UserContact, INotifyPropertyChanged
{
	private bool isChoose = true;

	private bool sendEmailBet = true;

	private bool sendBetInfo = false;

	private string folderOfFotos;

	private Passport passport;

	private StatusPlayer statusPlayer;

	public bool IsChoose
	{
		get
		{
			return isChoose;
		}
		set
		{
			isChoose = value;
			RaisePropertyChanged("IsChoose");
		}
	}

	public bool SendEmailBet
	{
		get
		{
			return sendEmailBet;
		}
		set
		{
			sendEmailBet = value;
			RaisePropertyChanged("SendEmailBet");
		}
	}

	public bool SendBetInfo
	{
		get
		{
			return sendBetInfo;
		}
		set
		{
			sendBetInfo = value;
			RaisePropertyChanged("SendBetInfo");
		}
	}

	public string FolderOfFotos
	{
		get
		{
			return folderOfFotos;
		}
		set
		{
			folderOfFotos = value;
			RaisePropertyChanged("FolderOfFotos");
		}
	}

	public Passport Passport
	{
		get
		{
			if (passport == null)
			{
				passport = new Passport();
			}
			return passport;
		}
		set
		{
			passport = value;
			RaisePropertyChanged("Passport");
		}
	}

	public ObservableCollection<Photo> Photos { get; set; } = new ObservableCollection<Photo>();

	public bool IsSaved { get; set; } = false;

	public StatusPlayer StatusPlayer
	{
		get
		{
			return statusPlayer;
		}
		set
		{
			statusPlayer = value;
			RaisePropertyChanged("StatusPlayer");
		}
	}
}
