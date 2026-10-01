using System.Linq;
using System.Reflection;
using BookmakerLibrary;
using CommonLibrary;
using ProtoBuf;
using ProtoBuf.Meta;

namespace AccountsLibrary;

public class ProtobufAM
{
	public TypeModel Serializer { get; set; }

	public ProtobufAM()
	{
		Serializer = SetUp().Compile();
	}

	private static RuntimeTypeModel SetUp()
	{
		RuntimeTypeModel runtimeTypeModel = RuntimeTypeModel.Create("SetUp");
		AddTypeToModel<AppExchange>(runtimeTypeModel);
		AddTypeToModel<AMExchange>(runtimeTypeModel);
		AddTypeToModel<StaticDataExchange>(runtimeTypeModel);
		AddTypeToModel<OwnerAccount>(runtimeTypeModel);
		AddTypeToModel<AccountSettings>(runtimeTypeModel);
		AddTypeToModel<AccountCommand>(runtimeTypeModel);
		AddTypeToModel<Cupis>(runtimeTypeModel);
		AddTypeToModel<Currency>(runtimeTypeModel);
		AddTypeToModel<ParameterAccount>(runtimeTypeModel);
		AddTypeToModel<PoslegolSettings>(runtimeTypeModel);
		AddTypeToModel<ValueSettings>(runtimeTypeModel);
		AddTypeToModel<ValuesPlusSettings>(runtimeTypeModel);
		AddTypeToModel<ValueScoreSettings>(runtimeTypeModel);
		AddTypeToModel<RandomSettings>(runtimeTypeModel);
		AddTypeToModel<Proxy>(runtimeTypeModel);
		AddTypeToModel<StatusAccount>(runtimeTypeModel);
		AddTypeToModel<IssueTypeParameter>(runtimeTypeModel);
		AddTypeToModel<ParametersSport>(runtimeTypeModel);
		AddTypeToModel<Country>(runtimeTypeModel);
		AddTypeToModel<Sport>(runtimeTypeModel);
		AddTypeToModel<StatusesRegistration>(runtimeTypeModel);
		AddTypeToModel<CouponLine.CoefFrame>(runtimeTypeModel);
		AddTypeToModel<CardBank>(runtimeTypeModel);
		AddTypeToModel<StrategySettingsBase>(runtimeTypeModel);
		AddTypeToModel<AccountCommandEnum>(runtimeTypeModel);
		AddTypeToModel<SortKoefEnum>(runtimeTypeModel);
		AddTypeToModel<RuleTelegramBot>(runtimeTypeModel);
		AddTypeToModel<RuleTelegramBotPoslegoal>(runtimeTypeModel);
		AddTypeToModel<RuleTelegramBotValue>(runtimeTypeModel);
		AddTypeToModel<RuleTelegramBotValuePlus>(runtimeTypeModel);
		AddTypeToModel<RuleTelegramBotValueScore>(runtimeTypeModel);
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
}
