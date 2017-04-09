using System;
using Android.App;
using Android.OS;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DatePickerDialog : DialogFragment
	{
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var currently = DateTime.Now;
			var dialog = new Android.App.DatePickerDialog(Activity, (Android.App.DatePickerDialog.IOnDateSetListener)Activity,
				currently.Year,
				currently.Month - 1,
				currently.Day);
			return dialog;
		}
	}
}