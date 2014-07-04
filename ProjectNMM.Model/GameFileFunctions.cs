using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectNMM.Model
{
	/// <summary>
	/// Static class with methods to load/save a game
	/// </summary>
	static class GameFileFunctions
	{
		/// <summary>
		/// Saves a game
		/// </summary>
		/// <param name="data">Game to save</param>
		/// <param name="path">Filepath</param>
		/// <returns>True if successful, false if failure</returns>
		public static bool SaveGame(GameData data, string path)
		{
			data.BoardStates.ForEach(b => b.ChangeToNormalArray());
			XmlSerializer serializerObj = new XmlSerializer(typeof(GameData));
			TextWriter streamWriter = new StreamWriter(path);

			serializerObj.Serialize(streamWriter, data);
			streamWriter.Close();

			return true;
		}

		/// <summary>
		/// Loads a game
		/// </summary>
		/// <param name="data">Game to load</param>
		/// <param name="path">Filepath</param>
		/// <returns>True if successful, false if failure</returns>
		public static bool LoadGame(ref GameData data, string path)
		{
			XmlSerializer serializerObj = new XmlSerializer(typeof(GameData));
			TextReader textReader = new StreamReader(path);

			if (!serializerObj.CanDeserialize(XmlReader.Create(textReader)))
			{
				textReader.Close();
				return false;
			}
			textReader.Close();
			textReader = new StreamReader(path);

			data = (GameData)serializerObj.Deserialize(textReader);
			textReader.Close();

			data.BoardStates.ForEach(b => b.ChangeToDimensionalArray());

			return true;
		}
	}
}
