using System;
using System.Collections.ObjectModel;
using System.Linq;
using SiriusTimetable.Common.Helpers;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Common.ViewModels
{
	public class TimetableViewModel : ObservableObject
	{
		#region Private fields

		private ObservableCollection<TimetableItem> _timetable;
		private TimetableInfo _info;
		private DateTime _date;
		private string _teamName;
		private string _shortTeam;

		#endregion

		#region Constructors

		public TimetableViewModel(DateTime defaultDate)
		{
			Date = defaultDate;
		}

		#endregion

		#region Public properties

		public TimetableInfo TempTimetableInfo { get; set; }
		public TimetableInfo TimetableInfo
		{
			get { return _info; }
			set { SetProperty(ref _info, value); }
		}
		public string ShortTeam
		{
			get { return _shortTeam; }
			set { SetProperty(ref _shortTeam, value); }
		}
		public string TeamName
		{
			get { return _teamName; }
			set { SetProperty(ref _teamName, value); }
		}
		public DateTime Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}
		public ObservableCollection<TimetableItem> Timetable
		{
			get { return _timetable; }
			set { SetProperty(ref _timetable, value); }
		}

		#endregion

		#region Private methods


		#endregion

		#region Public methods

		public bool UpdateSchedule(string team)
		{
			//checking data
			var dataExists =
				TimetableInfo.Timetable != null &&
				TimetableInfo.ShortLongTeamNameDictionary != null;

			if(!dataExists) return false;

			//updating data
			try
			{
				var timetableAll = TimetableInfo.Timetable;
				var timetable = timetableAll[TimetableInfo.ShortLongTeamNameDictionary[team]];

				Timetable = new ObservableCollection<TimetableItem>(timetable.Select(arg => new TimetableItem(arg)));
				TeamName = TimetableInfo.ShortLongTeamNameDictionary[team];
				ShortTeam = team;
				Date = TimetableInfo.Date;
				return true;
			}
			catch
			{
				//discard all values
				Timetable = null;
				TeamName = null;
				ShortTeam = null;
				return false;
			}
		}

		#endregion
	}
}