using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiriusTool.Model;

namespace SiriusTool.Services.Abstractions
{
	public interface ITimetableProvider
	{
	    Task<Dictionary<string, List<Event>>> RequestTimetable(DateTime? start, DateTime? end);

	    void Cancel();
	}
}