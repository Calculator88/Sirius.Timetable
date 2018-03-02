namespace SiriusTool.Services.Abstractions
{
	public interface ISelectedTeamCacher
	{
		string GetTeam();
		void CacheTeam(string team);
	}
}