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
		RuntimeTypeModel model = RuntimeTypeModel.Create("SetUp");
		AddTypeToModel<AppExchange>(model);
		AddTypeToModel<AMExchange>(model);
		AddTypeToModel<StaticDataExchange>(model);
		AddTypeToModel<OwnerAccount>(model);
		AddTypeToModel<AccountSettings>(model);
		AddTypeToModel<AccountCommand>(model);
		AddTypeToModel<Cupis>(model);
		AddTypeToModel<Currency>(model);
		AddTypeToModel<ParameterAccount>(model);
		AddTypeToModel<PoslegolSettings>(model);
		AddTypeToModel<ValueSettings>(model);
		AddTypeToModel<ValuesPlusSettings>(model);
		AddTypeToModel<ValueScoreSettings>(model);
		AddTypeToModel<RandomSettings>(model);
		AddTypeToModel<Proxy>(model);
		AddTypeToModel<StatusAccount>(model);
		AddTypeToModel<IssueTypeParameter>(model);
		AddTypeToModel<ParametersSport>(model);
		AddTypeToModel<Country>(model);
		AddTypeToModel<Sport>(model);
		AddTypeToModel<StatusesRegistration>(model);
		AddTypeToModel<CouponLine.CoefFrame>(model);
		AddTypeToModel<CardBank>(model);
		AddTypeToModel<StrategySettingsBase>(model);
		AddTypeToModel<AccountCommandEnum>(model);
		AddTypeToModel<SortKoefEnum>(model);
		AddTypeToModel<RuleTelegramBot>(model);
		AddTypeToModel<RuleTelegramBotPoslegoal>(model);
		AddTypeToModel<RuleTelegramBotValue>(model);
		AddTypeToModel<RuleTelegramBotValuePlus>(model);
		AddTypeToModel<RuleTelegramBotValueScore>(model);
		return model;
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
