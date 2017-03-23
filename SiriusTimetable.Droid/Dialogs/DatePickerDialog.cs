using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DatePickerDialog : DialogFragment, Android.App.DatePickerDialog.IOnDateSetListener,
		IDatePickerDialogService
	{
		public const string DatePickerTag = "DatePickerDialog";

		private readonly FragmentManager _manager;

		private TaskCompletionSource<DateTime?> _completion;

		public DatePickerDialog(FragmentManager manager)
		{
			_manager = manager;
		}

		public async Task<DateTime?> SelectedDate()
		{
			_completion = new TaskCompletionSource<DateTime?>();
			Show(_manager, DatePickerTag);
			return await _completion.Task;
		}

		public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
		{
			DateTime? selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
			_completion.TrySetResult(selectedDate);
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var currently = DateTime.Now;
			var dialog = new Android.App.DatePickerDialog(Activity,
				this,
				currently.Year,
				currently.Month - 1,
				currently.Day);
			return dialog;
		}

		public override void OnDismiss(IDialogInterface dialog)
		{
			_completion.TrySetResult(null);
			base.OnDismiss(dialog);
		}
	}
}