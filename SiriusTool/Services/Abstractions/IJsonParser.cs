namespace SiriusTool.Services.Abstractions
{
	public interface IJsonParser
	{
		T ParseJson<T>(string jsonString);
		string SerializeObject(object obj);
	}
}