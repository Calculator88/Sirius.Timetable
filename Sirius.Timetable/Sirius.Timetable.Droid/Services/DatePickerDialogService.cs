using System;
using System.Threading.Tasks;
using Android.App;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class DatePickerDialogService : IDatePickerDialogService
	{
		private DateTime? _date;
		private readonly FragmentManager _manager;
		private TaskCompletionSource<DateTime?> _completion = new TaskCompletionSource<DateTime?>();
		public DatePickerDialogService(FragmentManager manager)
		{
			_manager = manager;
		}

		public async Task<DateTime?> SelectedDate()
		{
			ChoosenDate();
			return await _completion.Task;
		}
		public void ChoosenDate()
		{
			var frag = DatePickerFragment.NewInstance(delegate(DateTime? time)
			{
				_date = time;
				SetDate(_date);
			});
			frag.Show(_manager, DatePickerFragment.TAG);
		}

		private void SetDate(DateTime? date)
		{
			_completion.TrySetResult(date);
			_completion = new TaskCompletionSource<DateTime?>();
		}
	}
}