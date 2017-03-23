using System.Threading.Tasks;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ISelectTeamDialogService
	{
		Task<string> SelectedTeam(TimetableInfo info);
	}
}