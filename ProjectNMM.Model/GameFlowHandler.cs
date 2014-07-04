using System.Collections.Generic;
using System.Resources;

namespace ProjectNMM.Model
{
	/// <summary>
	/// Class which controls the game logic
	/// </summary>
	class GameFlowHandler
	{
		public GameData Game { get; private set; }
		public bool GameHasStarted { get; private set; }
		public bool LastTurnWasMill { get; private set; }
		public GameEvent GameEventPlayer1 { get; private set; }
		public GameEvent GameEventPlayer2 { get; private set; }
		private List<BoardState> _boardStates;
		private BoardState _lastBoardState;
		private bool _beforeLastTurnWasMill;
		private int LegimateTurns;

		/// <summary>
		/// Constructor to start a new game
		/// </summary>
		/// <param name="gameType">Type of the game</param>
		public GameFlowHandler(GameType gameType)
		{
			Game = new GameData(gameType);
			_boardStates = Game.BoardStates;

			_boardStates.Add(new BoardState());

			CurrentPlayer = PlaystoneState.Player1;
			ModelHelpFunctions.InitializePlaystoneStates(CurrentPlaystones, PlaystoneState.Selectable);

			GameHasStarted = false;
			LastTurnWasMill = false;
			LegimateTurns = 0;
			GameEventPlayer1 = GameEvent.NoEvent;
			GameEventPlayer2 = GameEvent.NoEvent;
			_lastBoardState = null;
		}

		/// <summary>
		/// Constructor to resume a saved game
		/// </summary>
		/// <param name="data">Game to resume</param>
		public GameFlowHandler(GameData data)
		{
			Game = data;
			_boardStates = Game.BoardStates;

			GameHasStarted = true;
			LastTurnWasMill = false;
			LegimateTurns = 0;
			GameEventPlayer1 = GameEvent.NoEvent;
			GameEventPlayer2 = GameEvent.NoEvent;
			_lastBoardState = null;
		}

		#region Properties

		/// <summary>
		/// Array with the current board
		/// </summary>
		public PlaystoneState[,] CurrentPlaystones
		{
			get { return _boardStates[_boardStates.Count - 1].Playstones; }
			private set { _boardStates[_boardStates.Count - 1].Playstones = value; }
		}

		/// <summary>
		/// Active player
		/// </summary>
		public PlaystoneState CurrentPlayer
		{
			get { return _boardStates[_boardStates.Count - 1].ActivePlayer; }
			private set { _boardStates[_boardStates.Count - 1].ActivePlayer = value; }
		}

		/// <summary>
		/// Not the active player
		/// </summary>
		public PlaystoneState NotCurrentPlayer
		{
			get
			{
				if (CurrentPlayer == PlaystoneState.Player1)
					return PlaystoneState.Player2;
				else
					return PlaystoneState.Player1;
			}
		}

		/// <summary>
		/// Event of the active player
		/// </summary>
		public GameEvent CurrentPlayerEvent
		{
			get
			{
				if (CurrentPlayer == PlaystoneState.Player1)
					return GameEventPlayer1;
				else
					return GameEventPlayer2;

			}
			private set
			{
				if (CurrentPlayer == PlaystoneState.Player1)
					GameEventPlayer1 = value;
				else
					GameEventPlayer2 = value;
			}
		}

		#endregion

		/// <summary>
		/// Makes the next turn
		/// </summary>
		/// <param name="index1">Chosen playstone</param>
		/// <param name="index2">Chosen playstone</param>
		public void PlaystoneChanged(int index1, int index2)
		{
			if (Game.GameIsOver)
				return;

			bool skipLastStep = false;

			GameEventPlayer1 = GameEvent.NoEvent;
			GameEventPlayer2 = GameEvent.NoEvent;

			// Chose the correct method
			if (LastTurnWasMill)
				skipLastStep = RemovePlaystone(index1, index2);
			else if (GameHasStarted)
				MovePlaystone(index1, index2);
			else
				SetPlaystone(index1, index2);

			// Reset the board after a mill
			if (!skipLastStep)
			{
				if (LastTurnWasMill)
				{
					CurrentPlayer = NotCurrentPlayer;
					ModelHelpFunctions.ReplacePlaystoneStates(PlaystoneState.Selectable, PlaystoneState.Neutral, CurrentPlaystones);
				}
			}

			// Make next step, if active player cannot remove a playstone after a mill
			if (LastTurnWasMill && !ArePlaystonesNotInMills(NotCurrentPlayer, CurrentPlaystones))
			{
				LastTurnWasMill = false;

				if (CurrentPlayer == PlaystoneState.Player1)
				{
					GameEventPlayer1 = GameEvent.NoPlaystonesRemovable;
					GameEventPlayer2 = GameEvent.NoEvent;
				}
				else
				{
					GameEventPlayer1 = GameEvent.NoEvent;
					GameEventPlayer2 = GameEvent.NoPlaystonesRemovable;
				}

				CurrentPlayer = NotCurrentPlayer;
			}

			// End the game, if one player has less than 2 playstones
			if (_boardStates[_boardStates.Count - 1].PlaystonesPlayer1 > 6 ||
				_boardStates[_boardStates.Count - 1].PlaystonesPlayer2 > 6)
			{
				Game.GameIsOver = true;
				CurrentPlayerEvent = GameEvent.GameOverNoPlaystonesLeft;

				if (_boardStates[_boardStates.Count - 1].PlaystonesPlayer1 > 6)
					Game.Winner = PlaystoneState.Player1;
				else
					Game.Winner = PlaystoneState.Player2;
			}
		}

		/// <summary>
		/// Withdraws the last turn
		/// </summary>
		public void UndoLastTurn()
		{
			if (!GameHasStarted || _boardStates.Count < 2 || Game.GameIsOver)
				return;
			if (Game.MoveIsActive || LastTurnWasMill)
			{
				CurrentPlayerEvent = GameEvent.FinishTurn;
				return;
			}
			if (_boardStates[_boardStates.Count - 1].BeforeLastTurnWasMill)
			{
				CurrentPlayerEvent = GameEvent.CannotUndoMill;
				return;
			}

			_lastBoardState = _boardStates[_boardStates.Count - 1];
			_boardStates.RemoveAt(_boardStates.Count - 1);
			LastTurnWasMill = false;
		}

		/// <summary>
		/// Repeats the last turn, if possible
		/// </summary>
		public void RedoLastTurn()
		{
			// Checks for various exeptions
			if (_lastBoardState == null)
				return;
			if (Game.MoveIsActive || LastTurnWasMill)
			{
				CurrentPlayerEvent = GameEvent.FinishTurn;
				return;
			}

			_boardStates.RemoveAt(_boardStates.Count - 1);
			_boardStates.Add(_lastBoardState);
			LastTurnWasMill = _beforeLastTurnWasMill;

			_lastBoardState = null;
			_beforeLastTurnWasMill = false;
		}

		/// <summary>
		/// Sets a playstone
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		private void SetPlaystone(int index1, int index2)
		{
			// Checks for various exeptions
			if (CurrentPlaystones[index1, index2] != PlaystoneState.Selectable)
				return;
			if (IsMill(index1, index2, CurrentPlayer, CurrentPlaystones, true))
			{
				LastTurnWasMill = true;
				CurrentPlayerEvent = GameEvent.PlayerHasMill;
			}

			NextStep(index1, index2, CurrentPlayer);

			// When all player have placed their playstones, the game begins
			if (LegimateTurns > 16)
			{
				GameHasStarted = true;
				LegimateTurns = 0;

				BoardState tmpState = _boardStates[_boardStates.Count - 1].Clone();
				Game.BoardStates = new List<BoardState>();
				Game.BoardStates.Add(tmpState);
				_boardStates = Game.BoardStates;

				ModelHelpFunctions.ReplacePlaystoneStates(
					PlaystoneState.Selectable,
					PlaystoneState.Neutral,
					CurrentPlaystones
				);
			}
			else
				LegimateTurns++;
		}

		/// <summary>
		/// Moves a playstone
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		private void MovePlaystone(int index1, int index2)
		{
			if (!Game.MoveIsActive && CurrentPlaystones[index1, index2] == CurrentPlayer)
			{// Move playstone
				// If the player cannot move, he loses
				if (!AreMovesPossible(CurrentPlayer, CurrentPlaystones) &&
					!(ModelHelpFunctions.CountPlaystoneStates(CurrentPlayer, CurrentPlaystones) < 3))
				{
					Game.GameIsOver = true;
					CurrentPlayerEvent = GameEvent.GameOverNoMovesPossible;
					Game.Winner = NotCurrentPlayer;
					return;
				}
				// The player can jump if he has only 3 playstones left
				if (ModelHelpFunctions.CountPlaystoneStates(CurrentPlayer, CurrentPlaystones) < 3)
				{
					Game.GameIsOver = true;
					CurrentPlayerEvent = GameEvent.GameOverNoPlaystonesLeft;
					Game.Winner = NotCurrentPlayer;
					return;
				}

				NextStep(index1, index2, CurrentPlaystones[index1, index2]);

				CurrentPlayer = NotCurrentPlayer;

				if (ModelHelpFunctions.CountPlaystoneStates(CurrentPlayer, CurrentPlaystones) <= 3)
					PossibleMoves(index1, index2, CurrentPlayer, CurrentPlaystones, true);
				else
					PossibleMoves(index1, index2, CurrentPlayer, CurrentPlaystones);

				Game.InactiveMoveIndex1 = index1;
				Game.InactiveMoveIndex2 = index2;

				Game.MoveIsActive = true;
				LastTurnWasMill = false;

				return;
			}
			else if (!Game.MoveIsActive)
			{// Wrong playstone chosen
				CurrentPlayerEvent = GameEvent.InvalidPlaystone;
				return;
			}
			else if (CurrentPlaystones[index1, index2] == CurrentPlayer)
			{// Calculate possible moves
				ModelHelpFunctions.ReplacePlaystoneStates(
					PlaystoneState.Selectable,
					PlaystoneState.Neutral,
					CurrentPlaystones
				);

				PossibleMoves(index1, index2, CurrentPlayer, CurrentPlaystones);

				Game.InactiveMoveIndex1 = index1;
				Game.InactiveMoveIndex2 = index2;

				return;
			}
			if (CurrentPlaystones[index1, index2] != PlaystoneState.Selectable)
			{// Wrong playstone chosen
				CurrentPlayerEvent = GameEvent.InvalidPlaystone;
				return;
			}

			CurrentPlaystones[Game.InactiveMoveIndex1, Game.InactiveMoveIndex2] = PlaystoneState.Neutral;

			if (IsMill(index1, index2, CurrentPlayer, CurrentPlaystones, true))
			{
				LastTurnWasMill = true;
				CurrentPlayerEvent = GameEvent.PlayerHasMill;
			}

			CurrentPlaystones[index1, index2] = CurrentPlayer;

			ModelHelpFunctions.ReplacePlaystoneStates(
				PlaystoneState.Selectable,
				PlaystoneState.Neutral,
				CurrentPlaystones
			);

			Game.InactiveMoveIndex1 = -1;
			Game.InactiveMoveIndex2 = -1;

			CurrentPlayer = NotCurrentPlayer;

			Game.MoveIsActive = false;
		}

		/// <summary>
		/// Removes a playstone
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <returns></returns>
		private bool RemovePlaystone(int index1, int index2)
		{
			// Checks for various exeptions
			if (!LastTurnWasMill)
				return true;
			if (CurrentPlaystones[index1, index2] != NotCurrentPlayer)
			{
				CurrentPlayerEvent = GameEvent.WrongPlaystoneAfterMill;
				return true;
			}
			if (IsMill(index1, index2, NotCurrentPlayer, CurrentPlaystones))
			{
				CurrentPlayerEvent = GameEvent.CannotBreakMill;
				return true;
			}

			if (GameHasStarted)
				NextStep(index1, index2, PlaystoneState.Neutral);
			else
				NextStep(index1, index2, PlaystoneState.Selectable);

			if (NotCurrentPlayer == PlaystoneState.Player1)
				_boardStates[_boardStates.Count - 1].PlaystonesPlayer1++;
			else
				_boardStates[_boardStates.Count - 1].PlaystonesPlayer2++;

			LastTurnWasMill = false;

			if (!GameHasStarted)
				ModelHelpFunctions.ReplacePlaystoneStates(PlaystoneState.Neutral, PlaystoneState.Selectable, CurrentPlaystones);

			if (_boardStates.Count >= 2)
				_boardStates[_boardStates.Count - 1].BeforeLastTurnWasMill = true;

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <param name="state">New state of the chosen playstone</param>
		private void NextStep(int index1, int index2, PlaystoneState state)
		{
			BoardState actualState = _boardStates[_boardStates.Count - 1];

			_boardStates.Add(new BoardState());

			CurrentPlaystones = ModelHelpFunctions.MergePlaystones(actualState.Playstones, index1, index2, state);

			if (actualState.ActivePlayer == PlaystoneState.Player1)
				CurrentPlayer = PlaystoneState.Player2;
			else
				CurrentPlayer = PlaystoneState.Player1;
			_boardStates[_boardStates.Count - 1].PlaystonesPlayer1 = actualState.PlaystonesPlayer1;
			_boardStates[_boardStates.Count - 1].PlaystonesPlayer2 = actualState.PlaystonesPlayer2;
		}

		/// <summary>
		/// Checks if there is a mill
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <param name="playerToCheck">Active player</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="checkPartial">If true, the method checks if there is a mill after this turn</param>
		/// <returns>True if mill, false if no mill</returns>
		static public bool IsMill(int index1, int index2, PlaystoneState playerToCheck, PlaystoneState[,] playstones,
			bool checkPartial = false)
		{
			bool isMill = false;

			// Check middle fields first
			if (index1 == 3)
			{
				if (index2 <= 2)
					isMill = CheckHorizontalForMill(3, playerToCheck, playstones, checkPartial, 0, 2);
				else if (index2 >= 4)
					isMill = CheckHorizontalForMill(3, playerToCheck, playstones, checkPartial, 4, 6);

				if (isMill)
					return true;

				if (CheckVerticalForMill(index2, playerToCheck, playstones, checkPartial))
					return true;
				else
					return false;
			}
			else if (index2 == 3)
			{
				if (index1 <= 2)
					isMill = CheckVerticalForMill(3, playerToCheck, playstones, checkPartial, 0, 2);
				else if (index1 >= 4)
					isMill = CheckVerticalForMill(3, playerToCheck, playstones, checkPartial, 4, 6);

				if (isMill)
					return true;

				if (CheckHorizontalForMill(index1, playerToCheck, playstones, checkPartial))
					return true;
				else
					return false;
			}

			// Check corner fields
			if (CheckHorizontalForMill(index1, playerToCheck, playstones, checkPartial))
				return true;
			if (CheckVerticalForMill(index2, playerToCheck, playstones, checkPartial))
				return true;

			return false;
		}

		/// <summary>
		/// Help function for IsMill
		/// </summary>
		/// <param name="line">Row to check</param>
		/// <param name="playerToCheck">Active player</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="checkPartial">If true, the method checks if there is a mill after this turn</param>
		/// <param name="from">From this column</param>
		/// <param name="to">To this column</param>
		/// <returns>True if mill, false if no mill</returns>
		static private bool CheckHorizontalForMill(int line, PlaystoneState playerToCheck, PlaystoneState[,] playstones,
			bool checkPartial = false, int from = 0, int to = 6)
		{
			int tmpInt = 0;

			for (int i = from; i <= to; i++)
			{
				if (playstones[line, i] == playerToCheck)
					tmpInt++;
			}

			if (tmpInt == 2 && checkPartial)
				return true;
			else if (tmpInt == 3)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Help function for IsMill
		/// </summary>
		/// <param name="column">Column to check</param>
		/// <param name="playerToCheck">Active player</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="checkPartial">If true, the method checks if there is a mill after this turn</param>
		/// <param name="from">From this row</param>
		/// <param name="to">To this row</param>
		/// <returns>True if mill, false if no mill</returns>
		static private bool CheckVerticalForMill(int column, PlaystoneState playerToCheck, PlaystoneState[,] playstones,
			bool checkPartial = false, int from = 0, int to = 6)
		{
			int tmpInt = 0;

			for (int i = from; i <= to; i++)
			{
				if (playstones[i, column] == playerToCheck)
					tmpInt++;
			}

			if (tmpInt == 2 && checkPartial)
				return true;
			else if (tmpInt == 3)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Sets the possible moves on the board
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <param name="activePlayer">Active Player</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="freeJumps">If true, the player can jump on every field</param>
		static public void PossibleMoves(int index1, int index2, PlaystoneState activePlayer, PlaystoneState[,] playstones,
			bool freeJumps = false)
		{
			if (freeJumps)
			{
				ModelHelpFunctions.ReplacePlaystoneStates(PlaystoneState.Neutral, PlaystoneState.Selectable, playstones);

				return;
			}

			switch (ModelHelpFunctions.GetPosition(index1, index2))
			{
			case SquarePosition.TopLeft:
				CheckCornerForMove(index1, index2, playstones, SquarePosition.Left, SquarePosition.TopMiddle);
				break;
			case SquarePosition.TopMiddle:
				CheckMiddleForMove(index1, index2, playstones, SquarePosition.TopLeft, SquarePosition.TopRight);
				break;
			case SquarePosition.TopRight:
				CheckCornerForMove(index1, index2, playstones, SquarePosition.TopMiddle, SquarePosition.Right);
				break;
			case SquarePosition.Right:
				CheckMiddleForMove(index1, index2, playstones, SquarePosition.TopRight, SquarePosition.BottomRight);
				break;
			case SquarePosition.BottomRight:
				CheckCornerForMove(index1, index2, playstones, SquarePosition.Right, SquarePosition.BottomMiddle);
				break;
			case SquarePosition.BottomMiddle:
				CheckMiddleForMove(index1, index2, playstones, SquarePosition.BottomRight, SquarePosition.BottomLeft);
				break;
			case SquarePosition.BottomLeft:
				CheckCornerForMove(index1, index2, playstones, SquarePosition.BottomMiddle, SquarePosition.Left);
				break;
			case SquarePosition.Left:
				CheckMiddleForMove(index1, index2, playstones, SquarePosition.BottomLeft, SquarePosition.TopLeft);
				break;
			default:
				return;
			}
		}

		/// <summary>
		/// Help function for PossibleMoves
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="pos1">Position next to the corner</param>
		/// <param name="pos2">Position next to the corner</param>
		static private void CheckCornerForMove(int index1, int index2,
			PlaystoneState[,] playstones, SquarePosition pos1, SquarePosition pos2)
		{
			BoardSquare square = ModelHelpFunctions.GetSquare(index1, index2);

			// Check fields next to this one
			CheckPositionForMove(playstones, square, pos1);
			CheckPositionForMove(playstones, square, pos2);
		}

		/// <summary>
		/// Help function for PossibleMoves
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="pos1">Position in the corner</param>
		/// <param name="pos2">Position in the corner</param>
		static private void CheckMiddleForMove(int index1, int index2,
			PlaystoneState[,] playstones, SquarePosition pos1, SquarePosition pos2)
		{
			BoardSquare square = ModelHelpFunctions.GetSquare(index1, index2);

			// Check fields next to this one
			CheckPositionForMove(playstones, square, pos1);
			CheckPositionForMove(playstones, square, pos2);

			// Check fields in the middle
			switch (square)
			{
			case BoardSquare.OutterSquare:
			case BoardSquare.InnerSquare:
				CheckPositionForMove(playstones, BoardSquare.MiddleSquare, ModelHelpFunctions.GetPosition(index1, index2));
				break;
			case BoardSquare.MiddleSquare:
				CheckPositionForMove(playstones, BoardSquare.OutterSquare, ModelHelpFunctions.GetPosition(index1, index2));
				CheckPositionForMove(playstones, BoardSquare.InnerSquare, ModelHelpFunctions.GetPosition(index1, index2));
				break;
			}
		}

		/// <summary>
		/// Help function for PossibleMoves
		/// </summary>
		/// <param name="playstones">Actual board</param>
		/// <param name="square">Board square</param>
		/// <param name="pos">Position inside the square</param>
		static private void CheckPositionForMove(PlaystoneState[,] playstones, BoardSquare square,
			SquarePosition pos)
		{
			int i = 0, j = 0;

			ModelHelpFunctions.GetIndexes(square, pos, ref i, ref j);
			if (playstones[i, j] == PlaystoneState.Neutral)
				playstones[i, j] = PlaystoneState.Selectable;
		}

		/// <summary>
		/// Checks if the actual player can make moves
		/// </summary>
		/// <param name="activePlayer">Active Player</param>
		/// <param name="playstones">Current board</param>
		/// <returns>True if the player can make moves, false if not</returns>
		static public bool AreMovesPossible(PlaystoneState activePlayer, PlaystoneState[,] playstones)
		{
			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					if (playstones[i, j] == activePlayer)
					{
						PlaystoneState[,] stones = ModelHelpFunctions.CopyPlaystoneStates(playstones);

						PossibleMoves(i, j, activePlayer, stones);

						if (ModelHelpFunctions.CountPlaystoneStates(PlaystoneState.Selectable, stones) > 0)
							return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if there are playstones in mills
		/// </summary>
		/// <param name="player">Player to check</param>
		/// <param name="playstones">Current board</param>
		/// <returns>True if there are mills, false if not</returns>
		static public bool ArePlaystonesInMills(PlaystoneState player, PlaystoneState[,] playstones)
		{
			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					if (playstones[i, j] == player)
					{
						if (IsMill(i, j, player, playstones))
							return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if there are playstones not in mills
		/// </summary>
		/// <param name="player">Player to check</param>
		/// <param name="playstones">Current board</param>
		/// <returns>True if there are playstones available which are not in a mill, false if not</returns>
		static public bool ArePlaystonesNotInMills(PlaystoneState player, PlaystoneState[,] playstones)
		{
			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					if (playstones[i, j] == player)
					{
						if (!IsMill(i, j, player, playstones))
							return true;
					}
				}
			}

			return false;
		}
	}
}
