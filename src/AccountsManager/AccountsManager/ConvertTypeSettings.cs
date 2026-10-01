using System;
using System.Globalization;
using System.Windows.Data;
using AccountsLibrary;
using CommonLibrary;

namespace AccountsManager;

internal class ConvertTypeSettings : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		StrategySettingsBase strategySettingsBase = value as StrategySettingsBase;
		if (strategySettingsBase.TypeStrategy == TypesStrategy.Poslegoal)
		{
			return new ucSettingsPoslegol();
		}
		if (strategySettingsBase.TypeStrategy == TypesStrategy.Value)
		{
			return new ucSettingsValue();
		}
		if (strategySettingsBase.TypeStrategy == TypesStrategy.Fork)
		{
			return new ucSettingsFork();
		}
		if (strategySettingsBase.TypeStrategy == TypesStrategy.ValuePlus)
		{
			return new ucSettingsValuesPlus();
		}
		if (strategySettingsBase.TypeStrategy == TypesStrategy.ValueScore)
		{
			return new ucSettingsValueScore();
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString();
	}
}
