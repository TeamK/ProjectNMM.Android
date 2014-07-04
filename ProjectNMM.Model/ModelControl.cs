using System;
using System.IO;

namespace ProjectNMM.Model
{
	public class ModelControl
	{
		private GameFlowHandler _gameHandler;
		private Random _random;

		/// <summary>
		/// Constructor to make a new instance
		/// </summary>
		public ModelControl()
		{
			_random = new Random();
		}

		#region Properties

		/// <summary>
		/// Game started or not
		/// </summary>
		public bool GameInProgress
		{
			get
			{
				if (_gameHandler == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Active player has not finished his turn
		/// </summary>
		public bool MoveInProgress
		{
			get
			{
				if (_gameHandler.Game.MoveIsActive ||
					_gameHandler.LastTurnWasMill)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Type of the game
		/// </summary>
		public GameType GameType
		{
			get { return _gameHandler.Game.GameType; }
			set { _gameHandler.Game.GameType = value; }
		}

		/// <summary>
		/// Current board
		/// </summary>
		public PlaystoneState[,] Playstones
		{
			get { return _gameHandler.CurrentPlaystones; }
		}

		/// <summary>
		/// Active player
		/// </summary>
		public PlaystoneState ActivePlayer
		{
			get { return _gameHandler.CurrentPlayer; }
		}

		/// <summary>
		/// Active player has not finished his move
		/// </summary>
		public bool MoveIsActive
		{
			get { return _gameHandler.Game.MoveIsActive; }
		}

		/// <summary>
		/// Name of Player 1
		/// </summary>
		public string PlayerName1
		{
			get { return _gameHandler.Game.PlayerName1; }
			set { _gameHandler.Game.PlayerName1 = ModelHelpFunctions.RemoveSpecialCharacters(value); }
		}

		/// <summary>
		/// Name of Player 2
		/// </summary>
		public string PlayerName2
		{
			get { return _gameHandler.Game.PlayerName2; }
			set { _gameHandler.Game.PlayerName2 = ModelHelpFunctions.RemoveSpecialCharacters(value); }
		}

		/// <summary>
		/// Amount of playstones collected by Player 1
		/// </summary>
		public int PlaystonesPlayer1
		{
			get { return _gameHandler.Game.BoardStates[_gameHandler.Game.BoardStates.Count - 1].PlaystonesPlayer1; }
		}

		/// <summary>
		/// Amount of playstones collected by Player 2
		/// </summary>
		public int PlaystonesPlayer2
		{
			get { return _gameHandler.Game.BoardStates[_gameHandler.Game.BoardStates.Count - 1].PlaystonesPlayer2; }
		}

		/// <summary>
		/// Start time of the game
		/// </summary>
		public DateTime StartTime
		{
			get { return _gameHandler.Game.StartTime; }
		}

		/// <summary>
		/// End time of the game
		/// </summary>
		public DateTime EndTime
		{
			get { return _gameHandler.Game.EndTime; }
		}

		/// <summary>
		/// Game has finished
		/// </summary>
		public bool GameIsOver
		{
			get { return _gameHandler.Game.GameIsOver; }
		}

		/// <summary>
		/// Player which has won
		/// </summary>
		public PlaystoneState Winner
		{
			get { return _gameHandler.Game.Winner; }
		}

		/// <summary>
		/// Description of the game
		/// </summary>
		public string Description
		{
			get { return _gameHandler.Game.Description; }
			set { _gameHandler.Game.Description = value; }
		}

		/// <summary>
		/// Player has to chose a playstone to remove after a mill
		/// </summary>
		public bool LastTurnWasMill
		{
			get { return _gameHandler.LastTurnWasMill; }
		}

		/// <summary>
		/// Game event of Player 1
		/// </summary>
		public GameEvent GameEventPlayer1 
		{
			get { return _gameHandler.GameEventPlayer1; }
		}

		/// <summary>
		/// Game event of Player 2
		/// </summary>
		public GameEvent GameEventPlayer2
		{
			get { return _gameHandler.GameEventPlayer2; }
		}

		#endregion

		/// <summary>
		/// Start a new game
		/// </summary>
		/// <param name="gameType">Type of the game</param>
		public void StartNewGame(GameType gameType)
		{
			_gameHandler = new GameFlowHandler(gameType);
		}

		/// <summary>
		/// Changes the board after a change
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		public void PlaystoneChanged(int index1, int index2)
		{
			if (!GameInProgress)
				return;

			_gameHandler.PlaystoneChanged(index1, index2);

			if (GameType == GameType.PlayerVsMachine &&
				ActivePlayer == PlaystoneState.Player2)
				ExecuteMachineStep();
		}

		/// <summary>
		/// Finishes an automatic game
		/// </summary>
		public void EndManagedGame()
		{
			if (_gameHandler == null ||
				_gameHandler.Game.GameType != GameType.MachineVsMachine)
				return;

			while (!GameIsOver)
			{
				NextManagedStep();
			}
		}

		/// <summary>
		/// Makes next step in an automatic game
		/// </summary>
		public void NextManagedStep()
		{
			if (_gameHandler != null &&
				_gameHandler.Game.GameType == GameType.MachineVsMachine)
				ExecuteMachineStep();
		}

		/// <summary>
		/// Undo last turn
		/// </summary>
		public void UndoTurn()
		{
			if (GameInProgress)
				_gameHandler.UndoLastTurn();
		}

		/// <summary>
		/// Redo last turn
		/// </summary>
		public void RedoTurn()
		{
			if (GameInProgress)
				_gameHandler.RedoLastTurn();
		}

		/// <summary>
		/// Saves active game
		/// </summary>
		/// <param name="path">Filepath</param>
		/// <returns>True if successful, false if failure</returns>
		public bool SaveGame(string path)
		{
			if (path == "" || !_gameHandler.GameHasStarted ||
				_gameHandler.Game.MoveIsActive || !GameInProgress)
				return false;

			if (File.Exists(path))
				File.Delete(path);
			if (File.Exists(path))
				return false;

			bool returnValue = false;

			try
			{
				returnValue = GameFileFunctions.SaveGame(_gameHandler.Game, path);
			}
			catch
			{
				returnValue = false;
			}

			return returnValue;
		}

		/// <summary>
		/// Loads a game
		/// </summary>
		/// <param name="path">Filename</param>
		/// <returns>True if successful, false if failure</returns>
		public bool LoadGame(string path)
		{
			if (path == "")
				return false;
			if (!File.Exists(path))
				return false;

			bool returnValue = false;
			GameData data = null;

			try
			{
				returnValue = GameFileFunctions.LoadGame(ref data, path);
			}
			catch
			{
				returnValue = false;
			}

			if (!returnValue || data == null)
				return false;

			_gameHandler = new GameFlowHandler(data);

			return true;
		}

		/// <summary>
		/// Executes the next AI turn
		/// </summary>
		private void ExecuteMachineStep()
		{
			if (GameIsOver)
				return;

			int index1 = -1, index2 = -1;

			// Make a step depended of the board situation
			if (LastTurnWasMill)
			{// Remove playstone after mill
				ArtificialIntelligence.ChoseRandomPlaystone(_gameHandler.NotCurrentPlayer, Playstones, ref index1, ref index2, _random);
			}
			else if (!_gameHandler.GameHasStarted || (MoveIsActive && ModelHelpFunctions.CountPlaystoneStates(PlaystoneState.Selectable, Playstones) > 0))
			{// Set a playstone
				ArtificialIntelligence.ChoseRandomPlaystone(PlaystoneState.Selectable, Playstones, ref index1, ref index2, _random);
			}
			else
			{// Select a playstone to move
				ArtificialIntelligence.ChoseRandomPlaystone(ActivePlayer, Playstones, ref index1, ref index2, _random);
			}

			PlaystoneChanged(index1, index2);
		}
	}
}
