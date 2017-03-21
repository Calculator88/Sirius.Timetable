using System.IO;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class SelectedTeamCacher : ISelectedTeamCacher
	{
		public SelectedTeamCacher(string cachePath)
		{
			_cacheLocation = cachePath;
		}

		private readonly string _cacheLocation;

		public string Get()
		{
			var fileName = Path.Combine(_cacheLocation, "savedTeam");
			return File.Exists(fileName) ? File.ReadAllText(fileName) : null;
		}

		public void Cache(string team)
		{
			var fileName = Path.Combine(_cacheLocation, "savedTeam");
			File.WriteAllText(fileName, team);
		}
	}
}