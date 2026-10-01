using System;
using CommonLibrary;
using Newtonsoft.Json;

namespace SnapshotsLibrary;

public class AppExchangeSnapshots
{
	public DateTime DateTimeMessage { get; set; } = DateTime.Now;

	public TypesSnapshotsData TypeData { get; set; }

	public byte[] DataByte { get; set; }

	public string DataStr { get; set; }

	public string Serialize()
	{
		return JsonConvert.SerializeObject(this, CommonStaticFunction.JsonSettings);
	}

	public static AppExchangeSnapshots Deserialize(string data)
	{
		return JsonConvert.DeserializeObject<AppExchangeSnapshots>(data, CommonStaticFunction.JsonSettings);
	}
}
