using CommonLibrary;
using Newtonsoft.Json;

namespace SnapshotsLibrary;

public class AppExchangeDataServerLine
{
	public TypesDataServerLine TypeData { get; set; }

	public byte[] DataByte { get; set; }

	public string DataStr { get; set; }

	public string Serialize()
	{
		return JsonConvert.SerializeObject(this, CommonStaticFunction.JsonSettings);
	}

	public static AppExchangeDataServerLine Deserialize(string data)
	{
		return JsonConvert.DeserializeObject<AppExchangeDataServerLine>(data, CommonStaticFunction.JsonSettings);
	}
}
