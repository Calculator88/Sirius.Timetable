﻿using System;
using Sirius.Timetable.Helpers;

namespace Sirius.Timetable.Models
{
	public class TimetableHeader : ObservableObject
	{
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
		private string _team;
		private bool _isLoaded;
		private string _date;
	}
}