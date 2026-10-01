namespace AccountsLibrary;

public static class BetConverter
{
	public static TypeBet GetTypeBet(string strTypeBet)
	{
		return strTypeBet switch
		{
			"Poslegol" => TypeBet.Poslegol, 
			"Fork" => TypeBet.Fork, 
			"Value" => TypeBet.Value, 
			"ValuePlus" => TypeBet.ValuePlus, 
			"ValueScore" => TypeBet.ValueScore, 
			_ => TypeBet._empty, 
		};
	}
}
