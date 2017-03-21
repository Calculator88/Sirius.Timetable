using System;
using SiriusTimetable.Common.Helpers;

namespace SiriusTimetable.Common.Models
{
	public class TimetableHeader : ObservableObject
	{
		private string _date;
		private bool _isLoaded;
		private string _team;
		private AsyncCommand _choseDate;
		public string Team
		{
			get { return _team; }
			set { SetProperty(ref _team, value); }
		}

		public string Date
		{
			get { return String.IsNullOrEmpty(_date) ? "" : _date; }
			set { SetProperty(ref _date, value); }
		}

		public bool IsLoaded
		{
			get { return _isLoaded; }
			set { SetProperty(ref _isLoaded, value); }
		}
		public AsyncCommand SelectDateCommand
		{
			get { return _choseDate; }
			set { SetProperty(ref _choseDate, value); }
		}
	}
}