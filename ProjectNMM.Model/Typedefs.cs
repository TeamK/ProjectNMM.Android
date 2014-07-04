namespace ProjectNMM.Model
{
	// Enums for the model

	public enum GameType
	{
		Undefined                   = 0,
		PlayerVsPlayer              = 1,
		PlayerVsMachine             = 2,
		MachineVsMachine            = 3,
		Online                      = 4
	};

	public enum PlaystoneState
	{
		NotAvailable                = -1,
		Neutral                     = 0,
		Player1                     = 1,
		Player2                     = 2,
		Selectable                  = 3
	};

	public enum GameEvent
	{
		NoEvent                     = 0,
		PlayerHasMill               = 1,
		WrongPlaystoneAfterMill     = 2,
		CannotBreakMill             = 3,
		InvalidPlaystone            = 4,
		GameOverNoPlaystonesLeft    = 5,
		GameOverNoMovesPossible     = 6,
		NoPlaystonesRemovable       = 7,
		FinishTurn                  = 8,
		CannotUndoMill              = 9
	};

	public enum BoardSquare
	{
		NoSquare                    = 0,
		OutterSquare                = 1,
		MiddleSquare                = 2,
		InnerSquare                 = 3
	};

	public enum SquarePosition
	{
		NoCorner                    = 0,
		TopLeft                     = 1,
		TopMiddle                   = 2,
		TopRight                    = 3,
		Right                       = 4,
		BottomRight                 = 5,
		BottomMiddle                = 6,
		BottomLeft                  = 7,
		Left                        = 8 
	};
}
