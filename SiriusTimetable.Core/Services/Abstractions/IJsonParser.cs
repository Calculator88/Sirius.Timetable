using System.Collections.Generic;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface IJsonParser
	{
		T ParseJson<T>(string jsonString);
		string SerializeObject(object obj);
	}
}