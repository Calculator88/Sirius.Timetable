using Newtonsoft.Json;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Core.Services
{
	public class JsonParser : IJsonParser
	{
		public T ParseJson<T>(string jsonString)
		{
			return JsonConvert.DeserializeObject<T>(jsonString);
		}

		public string SerializeObject(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
	}
}