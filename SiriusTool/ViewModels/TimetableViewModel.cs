using System;
using System.Collections.ObjectModel;
using Android.Util;
using SiriusTool.Helpers;
using SiriusTool.Model;

namespace SiriusTool.ViewModels
{
	public class TimetableViewModel : ObservableObject
	{
		#region Private fields

		private ObservableCollection<Event> _timetable;
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

		public TimetableInfo TimetableInfo
		{
			get => _info;
		    set => SetProperty(ref _info, value);
		}
		public string ShortTeam
		{
			get => _shortTeam;
		    set => SetProperty(ref _shortTeam, value);
		}
		public string TeamName
		{
			get => _teamName;
		    set => SetProperty(ref _teamName, value);
		}
		public DateTime Date
		{
			get => _date;
		    set => SetProperty(ref _date, value);
		}
		public ObservableCollection<Event> Timetable
		{
			get => _timetable;
		    set => SetProperty(ref _timetable, value);
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

				Timetable = new ObservableCollection<Event>(timetable);
				TeamName = TimetableInfo.ShortLongTeamNameDictionary[team];
				ShortTeam = team;
				Date = TimetableInfo.Date;
				return true;
			}
			catch(Exception ex)
			{
			    Log.Error("SiriusTool", $"An error occured: {ex.Message}");

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