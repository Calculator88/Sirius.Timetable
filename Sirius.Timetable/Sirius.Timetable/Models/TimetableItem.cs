using System;
using SiriusTimetable.Common.Helpers;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Common.Models
{
	public class TimetableItem : ObservableObject
	{
		private string _busFrom;
		private string _busTo;
		private int _color;
		private bool _isBus;
		private bool _isPlace;
		private bool _isSelected;
		private string _place;
		private string _start;

		public TimetableItem(Activity activity)
		{
			Parent = activity;
			Start = activity.Start.ToString("HH:mm");
			End = activity.End.ToString("HH:mm");
			if (activity.BusTo != null) BusTo = activity.BusTo.Value.ToString("HH:mm");
			if (activity.BusFrom != null) BusFrom = activity.BusFrom.Value.ToString("HH:mm");
			Title = activity.Title;
			Place = activity.Place;
			IsBus = !String.IsNullOrEmpty(BusTo);
			IsPlace = !String.IsNullOrEmpty(Place);
		}

		public Activity Parent { get; }

		public int Color
		{
			get { return _color; }
			set { SetProperty(ref _color, value); }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				SetProperty(ref _isSelected, value);
			}
		}

		public string Start
		{
			get { return _start; }
			set { SetProperty(ref _start, value); }
		}

		public string End { get; set; }

		public string BusTo
		{
			get { return String.IsNullOrEmpty(_busTo) ? "" : _busTo; }
			set { _busTo = value; }
		}

		public string BusFrom
		{
			get { return String.IsNullOrEmpty(_busFrom) ? "" : _busFrom; }
			set { _busFrom = value; }
		}

		public string Title { get; set; }

		public string Place
		{
			get { return String.IsNullOrEmpty(_place) ? "" : _place; }
			set { _place = value; }
		}

		public bool IsBus
		{
			get { return _isBus && _isSelected; }
			set { SetProperty(ref _isBus, value); }
		}

		public bool IsPlace
		{
			get { return _isPlace && _isSelected; }
			set { SetProperty(ref _isPlace, value); }
		}
	}
}