using System.ComponentModel;

namespace AccountsLibrary;

public class Photo : INotifyPropertyChanged
{
	private string pathFile;

	private PhotoType typeOfPhoto;

	private string comment = "";

	public string PathFile
	{
		get
		{
			return pathFile;
		}
		set
		{
			pathFile = value;
			RaisePropertyChanged("PathFile");
		}
	}

	public PhotoType TypeOfPhoto
	{
		get
		{
			return typeOfPhoto;
		}
		set
		{
			typeOfPhoto = value;
			RaisePropertyChanged("TypeOfPhoto");
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

	public event PropertyChangedEventHandler PropertyChanged;

	public void RaisePropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
