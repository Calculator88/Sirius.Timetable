using System;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ITimerService
	{
		void SetHandler(Action action);
	}
}