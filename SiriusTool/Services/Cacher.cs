using System.IO;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
{
	public class Cacher : ICacher
	{
		public Cacher(string cacheDir)
		{
			CacheDirectory = cacheDir;
		}
		public string CacheDirectory { get;	}
	    public FileInfo GetInfo(string fileName)
	    {
	        return new FileInfo(fileName);
	    }

	    public FileInfo GetInfoForTeam(string team)
	    {
	        var cacheStr = Path.Combine(CacheDirectory, "schedules", $"{team}.json");
            return new FileInfo(cacheStr);
	    }
	}
}