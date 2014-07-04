using System;

namespace ProjectNMM.Model
{
	/// <summary>
	/// Static class with methods for AI calulations
	/// </summary>
	static class ArtificialIntelligence
	{
		/// <summary>
		/// Makes a random turn
		/// </summary>
		/// <param name="state">Playstone for the turn</param>
		/// <param name="playstones">Actual board</param>
		/// <param name="index1">Return value for chosen playstone</param>
		/// <param name="index2">Return value for chosen playstone</param>
		/// <param name="rnd">Random number generator</param>
		static public void ChoseRandomPlaystone(PlaystoneState state, PlaystoneState[,] playstones, ref int index1,
			ref int index2, Random rnd)
		{
			while (true)
			{
				int i = rnd.Next(7), j = rnd.Next(7);

				if (playstones[i, j] == state)
				{
					index1 = i;
					index2 = j;

					break;
				}
			}
		}
	}
}
