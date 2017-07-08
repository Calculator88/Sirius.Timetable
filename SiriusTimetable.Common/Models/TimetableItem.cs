using System;
using SiriusTimetable.Common.Helpers;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Common.Models
{
	public class TimetableItem : ObservableObject
	{
		private DateTime? _busTo;
		private DateTime? _busFrom;
		private DateTime? _start;
		private DateTime? _end;
		private string _place;
		private string _title;

		public TimetableItem(Activity activity)
		{
			Parent = activity;
			Start = activity.Start;
			End = activity.End;
			BusTo = activity.BusTo;
			BusFrom = activity.BusFrom;
			Title = activity.Title;
			Place = activity.Place;
		}

		public Activity Parent { get; }

		public DateTime? Start
		{
			get { return _start; }
			set { SetProperty(ref _start, value); }
		}

		public DateTime? End
		{
			get { return _end; }
			set { SetProperty(ref _end, value); }
		}

		public DateTime? BusTo
		{
			get { return  _busTo; }
			set { SetProperty(ref _busTo, value); }
		}

		public DateTime? BusFrom
		{
			get { return _busFrom; }
			set { SetProperty(ref _busFrom, value); }
		}

		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		public string Place
		{
			get { return _place; }
			set { SetProperty(ref _place, value); }
		}
	}
}