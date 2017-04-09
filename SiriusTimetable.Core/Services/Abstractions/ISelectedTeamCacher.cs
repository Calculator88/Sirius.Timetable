namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ISelectedTeamCacher
	{
		string GetTeam();
		void CacheTeam(string team);
	}
}