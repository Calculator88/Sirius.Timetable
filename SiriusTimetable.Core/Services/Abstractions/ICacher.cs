using System;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ICacher
	{
		void Cache(string path, string content);
		string Get(string path);
		string CacheDirectory { get; }
		bool Exists(string path);
		DateTime GetCreationTime(string path);
	}
}