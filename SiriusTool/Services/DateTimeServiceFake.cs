using System;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
{
	public class DateTimeServiceFake : IDateTimeService
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.ParseExact("06.02.2017 16:00", "dd.MM.yyyy HH:mm", null);
		}
	}
}