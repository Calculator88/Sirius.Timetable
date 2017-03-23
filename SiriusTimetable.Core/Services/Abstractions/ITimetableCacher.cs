using System;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ITimetableCacher
	{
		bool? IsStale(DateTime date);
		string Get(DateTime dateToGet);
		void Cache(string timetableJsonText, DateTime dateToCache);
	}
}