using System;
using System.IO;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class Cacher : ICacher
	{
		public Cacher(String cacheDir)
		{
			CacheDirectory = cacheDir;
		}
		public String CacheDirectory { get;	}

		public string Get(string path)
		{
			return !Exists(path) ? null : File.ReadAllText(path);
		}

		public void Cache(string path, string content)
		{
			File.WriteAllText(path, content);
		}

		public bool Exists(string path)
		{
			return File.Exists(path);
		}

		public DateTime GetCreationTime(string path)
		{
			if(Exists(path)) return File.GetCreationTimeUtc(path);
			else return DateTime.MinValue;
		}
	}
}