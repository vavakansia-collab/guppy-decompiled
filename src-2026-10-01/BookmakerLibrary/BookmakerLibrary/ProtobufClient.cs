using System;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;

namespace BookmakerLibrary;

public class ProtobufClient
{
	public TypeModel Serializer { get; set; }

	public ProtobufClient()
	{
		Serializer = SetUp(typeof(Provider)).Compile();
	}

	private static RuntimeTypeModel SetUp(Type typeProvider)
	{
		RuntimeTypeModel runtimeTypeModel = RuntimeTypeModel.Create("SetUp");
		AddTypeToModel<Provider>(runtimeTypeModel);
		AddTypeToModelFields<ProviderUserCompareDto>(runtimeTypeModel);
		AddTypeToModel<Championat>(runtimeTypeModel);
		AddTypeToModel<Match>(runtimeTypeModel);
		AddTypeToModel<MatchLine>(runtimeTypeModel);
		AddTypeToModel<TypesMatch>(runtimeTypeModel);
		AddTypeToModelFields<MatchUserCompareDto>(runtimeTypeModel);
		AddTypeToModel<Event>(runtimeTypeModel);
		AddTypeToModel<Sport>(runtimeTypeModel);
		AddTypeToModel<Score>(runtimeTypeModel);
		AddTypeToModel<Coupon>(runtimeTypeModel);
		AddTypeToModel<PointScore>(runtimeTypeModel);
		AddTypeToModel<PointSubScore>(runtimeTypeModel);
		AddTypeToModel<CouponLine.CoefFrame>(runtimeTypeModel);
		return runtimeTypeModel;
	}

	private static MetaType AddTypeToModel<T>(RuntimeTypeModel typeModel)
	{
		IOrderedEnumerable<string> source = from p in typeof(T).GetProperties()
			where p.CustomAttributes.All((CustomAttributeData a) => a.AttributeType != typeof(ProtoIgnoreAttribute))
			select p.Name into name
			orderby name
			select name;
		return typeModel.Add(typeof(T), applyDefaultBehaviour: true).Add(source.ToArray());
	}

	private static MetaType AddTypeToModelFields<T>(RuntimeTypeModel typeModel)
	{
		IOrderedEnumerable<string> source = from p in typeof(T).GetFields()
			where p.CustomAttributes.All((CustomAttributeData a) => a.AttributeType != typeof(ProtoIgnoreAttribute))
			select p.Name into name
			orderby name
			select name;
		return typeModel.Add(typeof(T), applyDefaultBehaviour: true).Add(source.ToArray());
	}
}
