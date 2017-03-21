using Newtonsoft.Json;
using System.IO;

namespace HeatEquationSolver
{
	public class DataManager
	{
		//JsonSerializerSettings jsonSet = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };

		public void SaveToJson()
		{
			File.WriteAllText("settings.json",JsonConvert.SerializeObject(new Settings(), Formatting.Indented));
		}

		//public IEnumerable<T> LoadFromJson(string path)
		//{
		//	return JsonConvert.DeserializeObject<IEnumerable<T>>(File.ReadAllText(path));
		//}
	}
}
