using System;
using Newtonsoft.Json;

namespace CommonLibrary;

public class AppExchange
{
	public DateTime DateTimeMessage { get; set; } = DateTime.Now;

	public TypesProject TypeProject { get; set; }

	public TypesData TypeData { get; set; }

	public byte[] DataByte { get; set; }

	public string DataStr { get; set; }

	public string Serialize()
	{
		return JsonConvert.SerializeObject(this, CommonStaticFunction.JsonSettings);
	}

	public static AppExchange Deserialize(string data)
	{
		return JsonConvert.DeserializeObject<AppExchange>(data, CommonStaticFunction.JsonSettings);
	}
}
