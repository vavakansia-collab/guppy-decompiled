using System.Collections.ObjectModel;
using System.Linq;
using CommonLibrary;

namespace AccountsLibrary;

public static class CountriesBL
{
	public static ObservableCollection<Country> Countries;

	static CountriesBL()
	{
		Countries = new ObservableCollection<Country>();
	}

	public static void AddCountry(string name, string shortName, string nameEng, int numCode = 0)
	{
		Countries.Add(new Country
		{
			Name = name,
			ShortName = shortName,
			NameEng = nameEng,
			NumCode = numCode
		});
	}

	public static void CheckExistCountry()
	{
		if (Countries.SingleOrDefault((Country c) => c.Name == "не определено") == null)
		{
			AddCountry("не определено", "", "");
		}
		if (Countries.SingleOrDefault((Country c) => c.NameEng == "USA") == null)
		{
			AddCountry("Соединенные штаты америки", "США", "USA", 840);
		}
		if (Countries.Where((Country c) => c.ShortName == "Россия").Count() > 1)
		{
			Countries.Remove(Countries.Single((Country c) => c.ShortName == "Россия"));
		}
		if (Countries.Where((Country c) => c.ShortName == "Украина").Count() > 1)
		{
			Countries.Remove(Countries.Single((Country c) => c.ShortName == "Украина"));
		}
		if (Countries.Where((Country c) => c.ShortName == "США").Count() > 1)
		{
			Countries.Remove(Countries.Single((Country c) => c.ShortName == "США"));
		}
		if (Countries.SingleOrDefault((Country c) => c.NameEng == "RU") == null)
		{
			AddCountry("Россия", "Россия", "RU", 643);
		}
		if (Countries.SingleOrDefault((Country c) => c.NameEng == "UA") == null)
		{
			AddCountry("Украина", "Украина", "UA", 804);
		}
		if (Countries.SingleOrDefault((Country c) => c.NameEng == "USA") == null)
		{
			AddCountry("Соединенные штаты америки", "США", "USA", 840);
		}
		if (Countries.Single((Country c) => c.NameEng == "RU").NumCode == 0)
		{
			Countries.Single((Country c) => c.NameEng == "RU").NumCode = 643;
		}
		if (Countries.Single((Country c) => c.NameEng == "UA").NumCode == 0)
		{
			Countries.Single((Country c) => c.NameEng == "UA").NumCode = 804;
		}
		if (Countries.Single((Country c) => c.NameEng == "USA").NumCode == 0)
		{
			Countries.Single((Country c) => c.NameEng == "USA").NumCode = 840;
		}
		if (Countries.SingleOrDefault((Country c) => c.NameEng == "KAZ") == null)
		{
			AddCountry("Казахстан", "Казахстан", "KAZ", 398);
		}
		if (Countries.SingleOrDefault((Country c) => c.NameEng == "BLR") == null)
		{
			AddCountry("Беларусь", "Беларусь", "BLR", 112);
		}
		Countries.OrderBy((Country c) => c.NameEng);
	}
}
