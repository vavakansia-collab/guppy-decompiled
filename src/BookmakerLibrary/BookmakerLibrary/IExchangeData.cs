namespace BookmakerLibrary;

public interface IExchangeData
{
	double TimeUpload { get; set; }

	double TimeDownload { get; set; }

	void Setup(Provider provider);

	bool Start();

	void UploadData();

	void LoadProviderSettings();
}
