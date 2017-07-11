using System;
using Android.App;
using Android.OS;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DatePickerDialog : Android.Support.V7.App.AppCompatDialogFragment
	{
		private Android.App.DatePickerDialog.IOnDateSetListener _listener;

		private const string YEAR = "YEAR";
		private const string MONTH = "MONTH";
		private const string DAY = "DAY";
		private int _year;
		private int _month;
		private int _day;

		public DatePickerDialog(DateTime date)
		{
			_year = date.Year;
			_month = date.Month;
			_day = date.Day;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as Android.App.DatePickerDialog.IOnDateSetListener;

			var currentTime = ServiceLocator.GetService<IDateTimeService>().GetCurrentTime();
			if(savedInstanceState == null) return;

			_year = savedInstanceState.GetInt(YEAR);
			_month = savedInstanceState.GetInt(MONTH);
			_day = savedInstanceState.GetInt(DAY);
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt(YEAR, _year);
			outState.PutInt(MONTH, _month);
			outState.PutInt(DAY, _day);
			base.OnSaveInstanceState(outState);
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = new Android.App.DatePickerDialog(Activity, Resource.Style.datepickerdialog, _listener, _year, _month - 1,	_day);
		
			return dialog;
		}
	}
}