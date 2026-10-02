namespace BookmakerLibrary;

public class Sport
{
	public int IdSport;

	public string PeriodName;

	public bool CompareTotalScore;

	public bool IsConsistentScore;

	public bool IsSlowTimeGoal;

	public bool IsThreeWayType;

	public bool IsCancelPoints;

	public bool IsFullScoreParallelPeriod;

	public int PeriodDurationMinutesDefault;

	public bool IsAvailability { get; set; } = true;

	public bool IsAvailabilityRegular { get; set; } = true;

	public bool IsAvailabilityCyber { get; set; } = true;

	public bool IsAvailabilityShort { get; set; } = true;

	public string SysName { get; set; }

	public string ShortName { get; set; }

	public string FullName { get; set; }

	public int TotalPointsToCompare { get; set; }

	public Sport(int initIDSport, bool initIsAvailability, bool initIsAvailabilityRegular, bool initIsAvailabilityCyber, bool initIsAvailabilityShort, string initSysName, string initShortName, string initFullName, string initPeriodName, bool initCompareTotalScore, bool initIsConsistentScore, bool initIsSlowTimeGoal, bool isFullScoreParallelPeriod, bool isThreeWayType, bool isCancelPoints = false, int initPeriodDurationMinutesDefault = 0, int initTotalPointsToCompare = 8)
	{
		IsAvailability = initIsAvailability;
		IsAvailabilityRegular = initIsAvailabilityRegular;
		IsAvailabilityCyber = initIsAvailabilityCyber;
		IsAvailabilityShort = initIsAvailabilityShort;
		IdSport = initIDSport;
		SysName = initSysName;
		ShortName = initShortName;
		FullName = initFullName;
		PeriodName = initPeriodName;
		CompareTotalScore = initCompareTotalScore;
		IsConsistentScore = initIsConsistentScore;
		IsSlowTimeGoal = initIsSlowTimeGoal;
		IsThreeWayType = isThreeWayType;
		IsCancelPoints = isCancelPoints;
		PeriodDurationMinutesDefault = initPeriodDurationMinutesDefault;
		TotalPointsToCompare = initTotalPointsToCompare;
		IsFullScoreParallelPeriod = isFullScoreParallelPeriod;
	}
}
