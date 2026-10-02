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
		StrategySettingsBase so = value as StrategySettingsBase;
		if (so.TypeStrategy == TypesStrategy.Poslegoal)
		{
			return new ucSettingsPoslegol();
		}
		if (so.TypeStrategy == TypesStrategy.Value)
		{
			return new ucSettingsValue();
		}
		if (so.TypeStrategy == TypesStrategy.Fork)
		{
			return new ucSettingsFork();
		}
		if (so.TypeStrategy == TypesStrategy.ValuePlus)
		{
			return new ucSettingsValuesPlus();
		}
		if (so.TypeStrategy == TypesStrategy.ValueScore)
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
