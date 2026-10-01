using System.Collections.ObjectModel;

namespace BookmakerLibrary;

public static class SportBL
{
	public static ObservableCollection<Sport> Sports { get; set; }

	static SportBL()
	{
		Sports = new ObservableCollection<Sport>();
		Sports.Add(new Sport(1, initIsAvailability: true, initIsAvailabilityRegular: false, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Футбол", "Футбол", "Футбол", "тайм", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true, isCancelPoints: true, 45));
		Sports.Add(new Sport(2, initIsAvailability: true, initIsAvailabilityRegular: false, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Хоккей", "Хоккей", "Хоккей", "период", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true, isCancelPoints: true, 20));
		Sports.Add(new Sport(3, initIsAvailability: true, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Баскетбол", "Баскетбол", "Баскетбол", "четверть", initCompareTotalScore: true, initIsConsistentScore: false, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: true, isThreeWayType: true, isCancelPoints: false, 10));
		Sports.Add(new Sport(4, initIsAvailability: true, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Теннис", "Теннис", "Теннис", "сет", initCompareTotalScore: false, initIsConsistentScore: true, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: false, isThreeWayType: false, isCancelPoints: true));
		Sports.Add(new Sport(5, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Бейсбол", "Бейсбол", "Бейсбол", "иннинг", initCompareTotalScore: true, initIsConsistentScore: false, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(6, initIsAvailability: true, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Волейбол", "Волейбол", "Волейбол", "партия", initCompareTotalScore: false, initIsConsistentScore: true, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: false, isThreeWayType: false, isCancelPoints: true, 0, 10));
		Sports.Add(new Sport(7, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Регби", "Регби", "Регби", "период", initCompareTotalScore: false, initIsConsistentScore: false, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(8, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Гандбол", "Гандбол", "Гандбол", "тайм", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true, isCancelPoints: false, 30));
		Sports.Add(new Sport(10, initIsAvailability: true, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "НастольныйТеннис", "Наст.теннис", "Настольный теннис", "партия", initCompareTotalScore: false, initIsConsistentScore: true, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: false, isThreeWayType: false, isCancelPoints: false, 0, 12));
		Sports.Add(new Sport(13, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "АмериканскийФутбол", "Амер.футбол", "Американский футбол", "партия", initCompareTotalScore: true, initIsConsistentScore: false, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(14, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Футзал", "Футзал", "Футзал", "тайм", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(15, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "ХоккейСМячом", "Хоккей с мячом", "Хоккей с мячом", "тайм", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(16, initIsAvailability: true, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Бадминтон", "Бадминтон", "Бадминтон", "партия", initCompareTotalScore: false, initIsConsistentScore: true, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: false, isThreeWayType: false, isCancelPoints: false, 0, 10));
		Sports.Add(new Sport(17, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "ВодноеПоло", "Водное поло", "Водное поло", "четверть", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(25, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "ПляжныйФутбол", "Пляжн.футбол", "Пляжный футбол", "тайм", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
		Sports.Add(new Sport(28, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "АвстралийскийФутбол", "Австрал.футбол", "Австралийский футбол", "период", initCompareTotalScore: true, initIsConsistentScore: false, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: true, isThreeWayType: true, isCancelPoints: false, 20));
		Sports.Add(new Sport(29, initIsAvailability: true, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "ПляжныйВолейбол", "Пляжн.волейбол", "Пляжный волейбол", "период", initCompareTotalScore: false, initIsConsistentScore: true, initIsSlowTimeGoal: false, isFullScoreParallelPeriod: false, isThreeWayType: false));
		Sports.Add(new Sport(67, initIsAvailability: false, initIsAvailabilityRegular: true, initIsAvailabilityCyber: true, initIsAvailabilityShort: true, "Флорбол", "Флорбол", "Флорбол", "период", initCompareTotalScore: true, initIsConsistentScore: true, initIsSlowTimeGoal: true, isFullScoreParallelPeriod: true, isThreeWayType: true));
	}

	public static Sport GetSportByID(int idSport)
	{
		Sport result = null;
		foreach (Sport sport in Sports)
		{
			if (sport.IdSport == idSport)
			{
				result = sport;
			}
		}
		return result;
	}

	public static int GetIdSportByName(string nameSport)
	{
		if (nameSport.ToLower().Contains("футбол") || nameSport.ToUpper().Contains("FIFA") || nameSport == "Football")
		{
			return 1;
		}
		if (nameSport.ToLower().Contains("хоккей") || nameSport == "Кибер NHL" || nameSport == "IceHockey")
		{
			return 2;
		}
		if (nameSport.ToLower().Contains("баскетбол") || nameSport.ToUpper().Contains("NBA") || nameSport == "Basketball")
		{
			return 3;
		}
		if (nameSport == "Теннис" || nameSport == "Tennis")
		{
			return 4;
		}
		if (nameSport == "Бейсбол")
		{
			return 5;
		}
		if (nameSport == "Волейбол" || nameSport == "Volleyball")
		{
			return 6;
		}
		switch (nameSport)
		{
		case "Регби":
			return 7;
		case "Гандбол":
			return 8;
		default:
			if (!(nameSport == "TableTennis"))
			{
				if (nameSport == "Американский футбол" || nameSport == "АмериканскийФутбол")
				{
					return 13;
				}
				if (nameSport == "Футзал")
				{
					return 14;
				}
				if (nameSport == "Хоккей с мячом" || nameSport == "ХоккейСМячом")
				{
					return 15;
				}
				if (nameSport == "Бадминтон")
				{
					return 16;
				}
				if (nameSport == "Водное поло" || nameSport == "ВодноеПоло")
				{
					return 17;
				}
				if (nameSport == "ПляжныйФутбол" || nameSport == "Пляжный футбол")
				{
					return 25;
				}
				if (nameSport == "Австралийский футбол" || nameSport == "АвстралийскийФутбол")
				{
					return 28;
				}
				if (nameSport == "Пляжный волейбол" || nameSport == "ПляжныйВолейбол")
				{
					return 29;
				}
				if (nameSport == "Флорбол")
				{
					return 67;
				}
				return 0;
			}
			goto case "Настольный теннис";
		case "Настольный теннис":
		case "НастольныйТеннис":
			return 10;
		}
	}

	public static int GetIndexSubScoreTennis(string subScoreTeam)
	{
		if (subScoreTeam == "")
		{
			return 0;
		}
		if (subScoreTeam == "0" || subScoreTeam == "Game")
		{
			return 0;
		}
		switch (subScoreTeam)
		{
		case "15":
			return 1;
		case "30":
			return 2;
		case "40":
			return 3;
		default:
			if (subScoreTeam.Substring(0, 1).ToUpper() == "A" || subScoreTeam.Substring(0, 1).ToUpper() == "А")
			{
				return 4;
			}
			return 0;
		}
	}

	public static bool CheckAvailabilitySportById(int IDSport)
	{
		foreach (Sport sport in Sports)
		{
			if (sport.IdSport == IDSport && sport.IsAvailability)
			{
				return true;
			}
		}
		return false;
	}
}
