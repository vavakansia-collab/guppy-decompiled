using System.ComponentModel;

namespace AccountsLibrary;

public class OwnerAccount : UserContact, INotifyPropertyChanged
{
	private byte[] borderBrushMass = new byte[4];

	public string OwnerGuid { get; set; }

	public long TelegramIdChat { get; set; } = 0L;

	public byte[] BorderBrushMass
	{
		get
		{
			return borderBrushMass;
		}
		set
		{
			borderBrushMass = value;
			RaisePropertyChanged("BorderBrushMass");
		}
	}

	public bool RuleAdmin { get; set; } = false;

	public bool RuleCupis { get; set; } = false;

	public bool RuleProxy { get; set; } = false;

	public bool AvailabilityPoslegol { get; set; } = false;

	public bool AvailabilityValue { get; set; } = false;

	public bool AvailabilityFork { get; set; } = false;

	public bool AvailabilityValuePlus { get; set; } = false;

	public bool AvailabilityValueScore { get; set; } = false;

	public bool AvailabilityRandomBets { get; set; } = false;

	public bool AvailabilitySnapshotsViewer { get; set; } = false;

	public bool AvailabilityDropCoef { get; set; } = false;

	public bool AvailabilityCreateAccounts { get; set; } = false;

	public bool AvailabilityAdvancedSettings { get; set; } = false;

	public bool IsShortBetsInfo { get; set; } = false;

	public override string ToString()
	{
		return base.Name ?? "";
	}
}
