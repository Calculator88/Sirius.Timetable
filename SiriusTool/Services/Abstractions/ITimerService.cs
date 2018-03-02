using System;

namespace SiriusTool.Services.Abstractions
{
	public interface ITimerService
	{
		void SetHandler(Action action);
	}
}