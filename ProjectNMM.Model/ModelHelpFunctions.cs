using System.Text;

namespace ProjectNMM.Model
{

	/// <summary>
	/// Static class which provides general functions for the model
	/// </summary>
	static public class ModelHelpFunctions
	{
		public const PlaystoneState DefaultAvailablePlaystoneState = PlaystoneState.Neutral;
		public const PlaystoneState DefaultNotAvailablePlaystoneState = PlaystoneState.NotAvailable;

		/// <summary>
		/// Initializes a new board
		/// </summary>
		/// <param name="playstones">Board</param>
		/// <param name="defaultValue">Default value of possible positions</param>
		static public void InitializePlaystoneStates(PlaystoneState[,] playstones, PlaystoneState defaultValue = DefaultAvailablePlaystoneState)
		{
			SetPlaystoneStates(DefaultNotAvailablePlaystoneState, playstones);

			#region first square
			playstones[0, 0] = defaultValue;
			playstones[0, 3] = defaultValue;
			playstones[0, 6] = defaultValue;
			playstones[3, 6] = defaultValue;
			playstones[6, 6] = defaultValue;
			playstones[6, 3] = defaultValue;
			playstones[6, 0] = defaultValue;
			playstones[3, 0] = defaultValue;
			#endregion

			#region second square
			playstones[1, 1] = defaultValue;
			playstones[1, 3] = defaultValue;
			playstones[1, 5] = defaultValue;
			playstones[3, 5] = defaultValue;
			playstones[5, 5] = defaultValue;
			playstones[5, 3] = defaultValue;
			playstones[5, 1] = defaultValue;
			playstones[3, 1] = defaultValue;
			#endregion

			#region third square
			playstones[2, 2] = defaultValue;
			playstones[2, 3] = defaultValue;
			playstones[2, 4] = defaultValue;
			playstones[3, 4] = defaultValue;
			playstones[4, 4] = defaultValue;
			playstones[4, 3] = defaultValue;
			playstones[4, 2] = defaultValue;
			playstones[3, 2] = defaultValue;
			#endregion
		}

		/// <summary>
		/// Sets the whole board to a single state
		/// </summary>
		/// <param name="state">State to set</param>
		/// <param name="playstones">Board</param>
		static public void SetPlaystoneStates(PlaystoneState state, PlaystoneState[,] playstones)
		{
			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					playstones[i, j] = state;
				}
			}
		}

		/// <summary>
		/// Replaces playstones
		/// </summary>
		/// <param name="stateOld">State to search for</param>
		/// <param name="stateNew">State to replace with</param>
		/// <param name="playstones">Board</param>
		static public void ReplacePlaystoneStates(PlaystoneState stateOld, PlaystoneState stateNew, PlaystoneState[,] playstones)
		{
			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					if (playstones[i, j] == stateOld)
						playstones[i, j] = stateNew;
				}
			}
		}

		/// <summary>
		/// Counts the amount of playstones
		/// </summary>
		/// <param name="state">State to search for</param>
		/// <param name="playstones">Board</param>
		/// <returns>Amount of found states</returns>
		static public int CountPlaystoneStates(PlaystoneState state, PlaystoneState[,] playstones)
		{
			int tmpInt = 0;

			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					if (playstones[i, j] == state)
						tmpInt++;
				}
			}

			return tmpInt;
		}

		/// <summary>
		/// Copies a board
		/// </summary>
		/// <param name="playstones">Board to copy</param>
		/// <returns>Copied board</returns>
		static public PlaystoneState[,] CopyPlaystoneStates(PlaystoneState[,] playstones)
		{
			PlaystoneState[,] stones = new PlaystoneState[7, 7];

			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					stones[i, j] = playstones[i, j];
				}
			}

			return stones;
		}

		/// <summary>
		/// Merges a board
		/// </summary>
		/// <param name="actualBoard">Board</param>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <param name="newState">State to set</param>
		/// <returns>Merged board</returns>
		static public PlaystoneState[,] MergePlaystones(PlaystoneState[,] actualBoard, int index1, int index2,
			PlaystoneState newState)
		{
			PlaystoneState[,] playstones = (PlaystoneState[,])actualBoard.Clone();

			playstones[index1, index2] = newState;

			return playstones;
		}

		/// <summary>
		/// Returns the square
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <returns>square</returns>
		static public BoardSquare GetSquare(int index1, int index2)
		{
			if ((index1 == 0 && index2 == 0) ||
				(index1 == 0 && index2 == 3) ||
				(index1 == 0 && index2 == 6) ||
				(index1 == 3 && index2 == 6) ||
				(index1 == 6 && index2 == 6) ||
				(index1 == 6 && index2 == 3) ||
				(index1 == 6 && index2 == 0) ||
				(index1 == 3 && index2 == 0))
				return BoardSquare.OutterSquare;

			if ((index1 == 1 && index2 == 1) ||
				(index1 == 1 && index2 == 3) ||
				(index1 == 1 && index2 == 5) ||
				(index1 == 3 && index2 == 5) ||
				(index1 == 5 && index2 == 5) ||
				(index1 == 5 && index2 == 3) ||
				(index1 == 5 && index2 == 1) ||
				(index1 == 3 && index2 == 1))
				return BoardSquare.MiddleSquare;

			if ((index1 == 2 && index2 == 2) ||
				(index1 == 2 && index2 == 3) ||
				(index1 == 2 && index2 == 4) ||
				(index1 == 3 && index2 == 4) ||
				(index1 == 4 && index2 == 4) ||
				(index1 == 4 && index2 == 3) ||
				(index1 == 4 && index2 == 2) ||
				(index1 == 3 && index2 == 2))
				return BoardSquare.InnerSquare;

			return BoardSquare.NoSquare;
		}

		/// <summary>
		/// Returns the position inside a square
		/// </summary>
		/// <param name="index1">Chosen Playstone</param>
		/// <param name="index2">Chosen playstone</param>
		/// <returns>Position inside square</returns>
		static public SquarePosition GetPosition(int index1, int index2)
		{
			switch (GetSquare(index1, index2))
			{
			case BoardSquare.OutterSquare:
				if (index1 == 0 && index2 == 0)
					return SquarePosition.TopLeft;
				if (index1 == 0 && index2 == 3)
					return SquarePosition.TopMiddle;
				if (index1 == 0 && index2 == 6)
					return SquarePosition.TopRight;
				if (index1 == 3 && index2 == 6)
					return SquarePosition.Right;
				if (index1 == 6 && index2 == 6)
					return SquarePosition.BottomRight;
				if (index1 == 6 && index2 == 3)
					return SquarePosition.BottomMiddle;
				if (index1 == 6 && index2 == 0)
					return SquarePosition.BottomLeft;
				if (index1 == 3 && index2 == 0)
					return SquarePosition.Left;
				break;
			case BoardSquare.MiddleSquare:
				if (index1 == 1 && index2 == 1)
					return SquarePosition.TopLeft;
				if (index1 == 1 && index2 == 3)
					return SquarePosition.TopMiddle;
				if (index1 == 1 && index2 == 5)
					return SquarePosition.TopRight;
				if (index1 == 3 && index2 == 5)
					return SquarePosition.Right;
				if (index1 == 5 && index2 == 5)
					return SquarePosition.BottomRight;
				if (index1 == 5 && index2 == 3)
					return SquarePosition.BottomMiddle;
				if (index1 == 5 && index2 == 1)
					return SquarePosition.BottomLeft;
				if (index1 == 3 && index2 == 1)
					return SquarePosition.Left;
				break;
			case BoardSquare.InnerSquare:
				if (index1 == 2 && index2 == 2)
					return SquarePosition.TopLeft;
				if (index1 == 2 && index2 == 3)
					return SquarePosition.TopMiddle;
				if (index1 == 2 && index2 == 4)
					return SquarePosition.TopRight;
				if (index1 == 3 && index2 == 4)
					return SquarePosition.Right;
				if (index1 == 4 && index2 == 4)
					return SquarePosition.BottomRight;
				if (index1 == 4 && index2 == 3)
					return SquarePosition.BottomMiddle;
				if (index1 == 4 && index2 == 2)
					return SquarePosition.BottomLeft;
				if (index1 == 3 && index2 == 2)
					return SquarePosition.Left;
				break;
			default:
				break;
			}

			return SquarePosition.NoCorner;
		}

		/// <summary>
		/// Returns indexes of a position
		/// </summary>
		/// <param name="square">Square on the board</param>
		/// <param name="position">Position inside squar</param>
		/// <param name="index1">Index</param>
		/// <param name="index2">Index</param>
		static public void GetIndexes(BoardSquare square, SquarePosition position, ref int index1, ref int index2)
		{
			index1 = -1;
			index2 = -1;

			for (int i = 0; i <= 6; i++)
			{
				for (int j = 0; j <= 6; j++)
				{
					if (GetSquare(i, j) == square && GetPosition(i, j) == position)
					{
						index1 = i;
						index2 = j;

						return;
					}
				}
			}
		}

		/// <summary>
		/// Removes special charactes
		/// </summary>
		/// <param name="str">Input string</param>
		/// <returns>Normalized string</returns>
		static public string RemoveSpecialCharacters(string str)
		{
			if (string.IsNullOrEmpty(str))
				return "";

			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}
	}
}
