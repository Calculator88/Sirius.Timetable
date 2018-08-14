using System;
using Android.App;
using Android.OS;

namespace SiriusTool.Dialogs
{
	public class DatePickerDialog : Android.Support.V7.App.AppCompatDialogFragment
	{
		private const string Year = "com.sirius.timetable.DatePickerDialog.YEAR";
		private const string Month = "com.sirius.timetable.DatePickerDialog.MONTH";
		private const string Day = "com.sirius.timetable.DatePickerDialog.DAY";
		private int _year;
		private int _month;
		private int _day;

	    public DatePickerDialog()
	    {

	    }

		public DatePickerDialog(DateTime date)
		{
			_year = date.Year;
			_month = date.Month;
			_day = date.Day;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if(savedInstanceState == null) return;

			_year = savedInstanceState.GetInt(Year);
			_month = savedInstanceState.GetInt(Month);
			_day = savedInstanceState.GetInt(Day);
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt(Year, _year);
			outState.PutInt(Month, _month);
			outState.PutInt(Day, _day);
			base.OnSaveInstanceState(outState);
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = new Android.App.DatePickerDialog(Activity, Resource.Style.datepickerdialog);
            dialog.DatePicker.UpdateDate(_year, _month - 1, _day);
		    var dt1970 = new DateTime(1970, 1, 1);

            //����������� ���� - 1 ����� �������� ������
            dialog.DatePicker.MinDate = (long)(new DateTime(_year, _month, 1).ToUniversalTime() - dt1970).TotalMilliseconds;

            //������������ ���� - 28 ����� �������� ������
            dialog.DatePicker.MaxDate = (long)(new DateTime(_year, _month, 28).ToUniversalTime() - dt1970).TotalMilliseconds;
			return dialog;
		}
	}
}