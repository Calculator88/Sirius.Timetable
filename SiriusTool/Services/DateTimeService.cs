using System;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
{
	public class DateTimeService : IDateTimeService
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.Now;
		}
	}
}