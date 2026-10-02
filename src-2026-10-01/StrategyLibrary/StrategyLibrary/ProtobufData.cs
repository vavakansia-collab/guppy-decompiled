using System.Linq;
using System.Reflection;
using BookmakerLibrary;
using CompareLibrary;
using ProtoBuf;
using ProtoBuf.Meta;

namespace StrategyLibrary;

public class ProtobufData
{
	public TypeModel Serializer { get; set; }

	public ProtobufData()
	{
		Serializer = SetUp().Compile();
	}

	private static RuntimeTypeModel SetUp()
	{
		RuntimeTypeModel runtimeTypeModel = RuntimeTypeModel.Create("SetUp");
		AddTypeToModel<CompareMatch>(runtimeTypeModel);
		AddTypeToModel<Poslegol>(runtimeTypeModel);
		AddTypeToModel<Value>(runtimeTypeModel);
		AddTypeToModel<Fork>(runtimeTypeModel);
		AddTypeToModel<ValuePlus>(runtimeTypeModel);
		AddTypeToModel<ValueScore>(runtimeTypeModel);
		AddTypeToModel<Provider>(runtimeTypeModel);
		AddTypeToModel<Championat>(runtimeTypeModel);
		AddTypeToModel<Match>(runtimeTypeModel);
		AddTypeToModel<Event>(runtimeTypeModel);
		AddTypeToModel<Sport>(runtimeTypeModel);
		AddTypeToModel<Score>(runtimeTypeModel);
		AddTypeToModel<Coupon>(runtimeTypeModel);
		AddTypeToModel<PointScore>(runtimeTypeModel);
		AddTypeToModel<PointSubScore>(runtimeTypeModel);
		AddTypeToModel<CompareMatchParameter>(runtimeTypeModel);
		AddTypeToModel<CouponLine.CoefFrame>(runtimeTypeModel);
		AddTypeToModel<StrategyBase>(runtimeTypeModel);
		AddTypeToModel<SnapshotStrategis>(runtimeTypeModel);
		return runtimeTypeModel;
	}

	private static MetaType AddTypeToModel<T>(RuntimeTypeModel typeModel)
	{
		IOrderedEnumerable<string> properties = from p in typeof(T).GetProperties()
			where p.CustomAttributes.All((CustomAttributeData a) => a.AttributeType != typeof(ProtoIgnoreAttribute))
			select p.Name into name
			orderby name
			select name;
		return typeModel.Add(typeof(T), applyDefaultBehaviour: true).Add(properties.ToArray());
	}
}
