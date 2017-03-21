namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ISelectedTeamCacher
	{
		string Get();
		void Cache(string team);
	}
}