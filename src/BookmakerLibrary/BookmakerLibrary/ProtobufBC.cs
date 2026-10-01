using System;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;

namespace BookmakerLibrary;

public class ProtobufBC
{
	private Type TypeBC { get; set; }

	public TypeModel Serializer { get; set; }

	public ProtobufBC(Provider provider)
	{
		Serializer = SetUp(provider.GetType()).Compile();
	}

	private RuntimeTypeModel SetUp(Type typeBC)
	{
		RuntimeTypeModel runtimeTypeModel = RuntimeTypeModel.Create("SetUp");
		AddTypeToModel<Provider>(runtimeTypeModel).AddSubType(500, typeBC);
		AddTypeToModel<Championat>(runtimeTypeModel);
		AddTypeToModel<Match>(runtimeTypeModel);
		AddTypeToModel<MatchLine>(runtimeTypeModel);
		AddTypeToModel<TypesMatch>(runtimeTypeModel);
		AddTypeToModel<Event>(runtimeTypeModel);
		AddTypeToModel<Sport>(runtimeTypeModel);
		AddTypeToModel<Score>(runtimeTypeModel);
		AddTypeToModel<Coupon>(runtimeTypeModel);
		AddTypeToModel<PointScore>(runtimeTypeModel);
		AddTypeToModel<PointSubScore>(runtimeTypeModel);
		AddTypeToModel<CouponLine.CoefFrame>(runtimeTypeModel);
		return runtimeTypeModel;
	}

	private MetaType AddTypeToModel<T>(RuntimeTypeModel typeModel)
	{
		IOrderedEnumerable<string> source = from p in typeof(T).GetProperties()
			where p.CustomAttributes.All((CustomAttributeData a) => a.AttributeType != typeof(ProtoIgnoreAttribute))
			select p.Name into name
			orderby name
			select name;
		return typeModel.Add(typeof(T), applyDefaultBehaviour: true).Add(source.ToArray());
	}
}
