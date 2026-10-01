using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using CommonLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AccountsLibrary;

public static class CurrencyBL
{
	public static ObservableCollection<Currency> Currencies;

	public static void AddCurrency(string name, string shortName, string nameEng, int numCode, double exchange, string symbol)
	{
		Currencies.Add(new Currency
		{
			Name = name,
			ShortName = shortName,
			NameEng = nameEng,
			NumCode = numCode,
			Exchange = exchange,
			Symbol = symbol
		});
	}

	static CurrencyBL()
	{
		Currencies = new ObservableCollection<Currency>();
	}

	public static void UpdateCurrency()
	{
		Currencies.Clear();
		if (Currencies.SingleOrDefault((Currency c) => c.NameEng == "RUB") == null)
		{
			AddCurrency("Российский рубль", "руб.", "RUB", 643, 1.0, "₽");
		}
		if (Currencies.SingleOrDefault((Currency c) => c.NameEng == "USD") == null)
		{
			AddCurrency("Доллар США", "долл.", "USD", 840, 70.92, "$");
		}
		if (Currencies.SingleOrDefault((Currency c) => c.NameEng == "EUR") == null)
		{
			AddCurrency("Евро", "евро", "EUR", 978, 80.02, "€");
		}
		if (Currencies.SingleOrDefault((Currency c) => c.NameEng == "KZT") == null)
		{
			AddCurrency("Тенге", "тенге", "KZT", 398, 0.1818, "₸");
		}
		if (Currencies.SingleOrDefault((Currency c) => c.NameEng == "BYN") == null)
		{
			AddCurrency("Белорусский рубль", "Бел.рубль", "BYN", 933, 46.0, "Br");
		}
		if (Currencies.SingleOrDefault((Currency c) => c.NameEng == "UAH") == null)
		{
			AddCurrency("Украинских гривен", "Укр.гривна", "UAH", 980, 2.73612, "₴");
		}
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.cbr-xml-daily.ru/daily_json.js");
		HttpWebResponse httpWebResponse;
		try
		{
			httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		}
		catch
		{
			return;
		}
		StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
		string value;
		try
		{
			value = streamReader.ReadToEnd();
		}
		catch
		{
			httpWebResponse.Close();
			streamReader.Close();
			return;
		}
		httpWebResponse.Close();
		streamReader.Close();
		JContainer jsonData = (JContainer)JsonConvert.DeserializeObject(value);
		Currencies.ToList().ForEach(delegate(Currency currency)
		{
			JValue jValue = (from p in jsonData.Descendants().OfType<JValue>()
				where p.Value.ToString() == currency.NumCode.ToString()
				select p).FirstOrDefault();
			if (jValue != null)
			{
				JObject jObject = (JObject)jValue.Parent.Parent;
				double num = Convert.ToDouble(jObject["Value"].ToString().Replace(".", ","));
				double num2 = Convert.ToDouble(jObject["Nominal"].ToString().Replace(".", ","));
				currency.Exchange = num / num2;
			}
		});
		Console.WriteLine(CommonStaticFunction.GetDateTimeStamp() + " Валюты успешно обновлены");
	}

	public static double ConvertToCurrency(double sum, Currency currencyInput, Currency currencyOutput)
	{
		double exchange = currencyInput.Exchange;
		double exchange2 = currencyOutput.Exchange;
		double num = exchange / exchange2;
		return Math.Floor(sum * num);
	}
}
