using System.IO;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class SelectedTeamCacher : ISelectedTeamCacher
	{
		private readonly string _cacheLocation;

		public SelectedTeamCacher(string cachePath)
		{
			_cacheLocation = cachePath;
		}

		public string GetTeam()
		{
			var fileName = Path.Combine(_cacheLocation, "savedTeam");
			return File.Exists(fileName) ? File.ReadAllText(fileName) : null;
		}

		public void CacheTeam(string team)
		{
			var fileName = Path.Combine(_cacheLocation, "savedTeam");
			File.WriteAllText(fileName, team);
		}
	}
}