using Newtonsoft.Json;
using System.IO;

namespace HeatEquationSolver.Settings
{
	public static class DataManager
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
				ResetSetting();
			}
		}

		public static void SaveSettingsToFile()
		{
			File.WriteAllText(FilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}

		public static void ResetSetting()
		{
			Settings = new Settings();
		}
	}
}
