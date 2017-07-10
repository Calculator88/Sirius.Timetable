using System;
using System.Timers;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class TimerService : ITimerService
	{
		private readonly Timer _timer;
		private Action _action;
		private ElapsedEventHandler _handler;

		public TimerService()
		{
			_timer = new Timer {Interval = 60000};
		}

		public void SetHandler(Action action)
		{
			_timer.Stop();
			_timer.Elapsed -= _handler;
			_action = action;

			_handler = (sender, args) => { _action.Invoke(); };

			_timer.Elapsed += _handler;
			_timer.Start();
		}
	}
}