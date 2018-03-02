using Newtonsoft.Json;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
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