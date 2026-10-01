using System;

namespace CommonLibrary;

public class Client
{
	public bool isAuth;

	public Guid Guid { get; set; }

	public string IpPort { get; set; }

	public TypesProject TypeProject { get; set; }

	public string NameProject { get; set; }

	public string Name { get; set; }

	public string VersionApp { get; set; }
}
