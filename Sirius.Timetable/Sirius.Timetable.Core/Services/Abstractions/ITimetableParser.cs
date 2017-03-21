using System.Collections.Generic;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ITimetableParser
	{
		Dictionary<string, Timetable.Timetable> ParseTimetables(string jsonString);
	}
}