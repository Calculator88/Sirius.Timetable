using System;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class DateTimeService : IDateTimeService
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.Now;
		}
	}
}