using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProjectNMM.Model
{
	/// <summary>
	/// Class that represents a game
	/// </summary>
	[Serializable()]
	public class GameData
	{
		public GameType GameType;
		public List<BoardState> BoardStates;
		public bool MoveIsActive;
		public int InactiveMoveIndex1;
		public int InactiveMoveIndex2;
		public string PlayerName1;
		public string PlayerName2;
		public DateTime StartTime;
		public DateTime EndTime;
		public bool GameIsOver;
		public PlaystoneState Winner;
		public string Description;

		/// <summary>
		/// Constructor for a new game.
		/// </summary>
		/// <param name="gameType">Type of the game</param>
		public GameData(GameType gameType)
		{
			GameType = gameType;

			PlayerName1 = "";
			PlayerName2 = "";
			MoveIsActive = false;
			InactiveMoveIndex1 = -1;
			InactiveMoveIndex2 = -1;
			BoardStates = new List<BoardState>();
			StartTime = new DateTime();
			EndTime = new DateTime();
			GameIsOver = false;
			Winner = PlaystoneState.NotAvailable;
			Description = "";
		}

		/// <summary>
		/// Empty constructor, needed to deserialize from xml
		/// </summary>
		public GameData()
		{
		}
	}

	/// <summary>
	/// Class that represents a state of one turn
	/// </summary>
	[Serializable()]
	public class BoardState
	{
		[XmlIgnore]
		public PlaystoneState[,] Playstones;
		public PlaystoneState ActivePlayer;
		public int PlaystonesPlayer1;
		public int PlaystonesPlayer2;
		public bool BeforeLastTurnWasMill;
		public PlaystoneState[] SerializablePlaystones;

		/// <summary>
		/// Constructs a new turn with the board
		/// </summary>
		public BoardState()
		{
			Playstones = new PlaystoneState[7, 7];
			ModelHelpFunctions.InitializePlaystoneStates(Playstones);

			ActivePlayer = PlaystoneState.Neutral;

			PlaystonesPlayer1 = 0;
			PlaystonesPlayer2 = 0;

			BeforeLastTurnWasMill = false;
		}

		/// <summary>
		/// Clones the actual BoardState object
		/// </summary>
		/// <returns>New instance</returns>
		public BoardState Clone()
		{
			BoardState newState = new BoardState();

			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					newState.Playstones[i, j] = Playstones[i, j];
				}
			}
			newState.ActivePlayer = ActivePlayer;
			newState.PlaystonesPlayer1 = PlaystonesPlayer1;
			newState.PlaystonesPlayer2 = PlaystonesPlayer2;

			return newState;
		}

		/// <summary>
		/// Converts the two-dimensional array to a normal array, needed to serialize to xml
		/// </summary>
		public void ChangeToNormalArray()
		{
			SerializablePlaystones = new PlaystoneState[49];

			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					SerializablePlaystones[i * 7 + j] = Playstones[i, j];
				}
			}
		}

		/// <summary>
		/// Converts a normal array to a two-dimensional array, needed to deserialize from xml
		/// </summary>
		public void ChangeToDimensionalArray()
		{
			int index1 = 0;
			int index2 = -1;

			foreach (PlaystoneState serPlaystone in SerializablePlaystones)
			{
				index2++;

				if (index2 >= 7)
				{
					index2 = 0;
					index1++;
				}

				Playstones[index1, index2] = serPlaystone;
			}
		}
	}
}
