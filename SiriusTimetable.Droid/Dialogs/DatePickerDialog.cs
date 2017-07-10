using System;
using Android.App;
using Android.OS;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DatePickerDialog : DialogFragment
	{
		private Android.App.DatePickerDialog.IOnDateSetListener _listener;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as Android.App.DatePickerDialog.IOnDateSetListener;
			if (_listener == null) throw new Exception($"{Activity} must implement {typeof(Android.App.DatePickerDialog.IOnDateSetListener)}");
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var currently = DateTime.Now;
			var dialog = new Android.App.DatePickerDialog(Activity, Resource.Style.datepickerdialog, _listener,
				currently.Year,
				currently.Month - 1,
				currently.Day);
			return dialog;
		}
	}
}