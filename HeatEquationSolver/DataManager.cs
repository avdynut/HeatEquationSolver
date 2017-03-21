using Newtonsoft.Json;
using System.IO;

namespace HeatEquationSolver
{
	public class DataManager
	{
		private const string FilePath = "settings.json";
		public static Settings Settings;

		static DataManager()
		{
			try
			{
				Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));
			}
			catch
			{
				Settings = new Settings();
			}
		}

		public static void SaveSettingsToFile()
		{
			File.WriteAllText(FilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}
	}
}
