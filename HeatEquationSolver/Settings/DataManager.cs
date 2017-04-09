using Newtonsoft.Json;
using System.IO;

namespace HeatEquationSolver.Settings
{
	public static class DataManager
	{
		private const string FilePath = "settings.json";
		public static readonly Settings Settings;

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
