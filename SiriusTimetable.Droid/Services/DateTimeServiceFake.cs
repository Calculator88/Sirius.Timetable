using System;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class DateTimeServiceFake : IDateTimeService
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.ParseExact("06.02.2017 16:00", "dd.MM.yyyy HH:mm", null);
		}
	}
}