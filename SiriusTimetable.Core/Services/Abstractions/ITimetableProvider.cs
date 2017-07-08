using SiriusTimetable.Core.Timetable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ITimetableProvider
	{
		Task<TimetableInfo> GetTimetableInfo(DateTime date);
	}
}