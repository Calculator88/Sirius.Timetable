using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SiriusTool.Framework;
using SiriusTool.Helpers;
using SiriusTool.Model;
using SiriusTool.Services;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.ViewModels
{
	public class TimetableViewModel : ObservableObject
	{
		#region Private fields

		private ObservableCollection<Event> _currentTimetable;
		private TimetableInfo _info;
		private DateTime _date;
		private string _teamName;
		private string _shortTeam;
	    private bool _isBusy;
	    private readonly TimetableFactory _timetableFactory;
	    private readonly ITimetableProvider _timetableProvider;
	    private readonly TimeSpan _timetableLifeTime = TimeSpan.FromMinutes(10);

		#endregion

		#region Constructors

		public TimetableViewModel(DateTime defaultDate)
		{
			Date = defaultDate;
            _timetableFactory = new TimetableFactory();
		    _timetableProvider = ServiceLocator.GetService<ITimetableProvider>();
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

		public ObservableCollection<Event> CurrentTimetable
		{
			get => _currentTimetable;
		    set => SetProperty(ref _currentTimetable, value);
		}

	    public bool IsBusy
	    {
	        get => _isBusy;
	        set => SetProperty(ref _isBusy, value);
	    }

	    public event ExceptionOccuredEventHandler ExceptionOccured;

		#endregion

		#region Private methods


		#endregion

		#region Public methods

	    public async Task GetTimetable(DateTime date, bool forceNet = false)
	    {
	        if (date.Date is var d && !forceNet && 
	            _timetableFactory.TimetableExists(d) && 
	            DateTime.UtcNow - _timetableFactory.GetCreationTime(d) < _timetableLifeTime)
	        {
	            TimetableInfo = _timetableFactory.GetInfo(d);
	            Date = date.Date;
	            CurrentTimetable = null;
	            TeamName = null;
	            ShortTeam = null;
                return;
	        }


	        IsBusy = true;
	        try
	        {
	            var timetable = await _timetableProvider.RequestTimetable(date, null);
	            var info = new TimetableInfo(timetable, date.Date);
                _timetableFactory.ForceAdd(date.Date, info);
	            TimetableInfo = info;
	            Date = date.Date;
	            CurrentTimetable = null;
	            TeamName = null;
	            ShortTeam = null;
	        }
	        catch (Exception exception)
	        {
	            if (!_timetableFactory.TimetableExists(date.Date))
	            {
                    ExceptionOccured?.Invoke(exception);
	            }
	            else
	            {
	                TimetableInfo = _timetableFactory.GetInfo(date.Date);
	                Date = date.Date;
	                CurrentTimetable = null;
	                TeamName = null;
	                ShortTeam = null;
                }
            }

	        IsBusy = false;
	    }

        public void UpdateSchedule(string team)
		{
			//checking data
			var dataExists =
				TimetableInfo.Timetable != null &&
				TimetableInfo.ShortLongTeamNameDictionary != null;

		    if (!dataExists)
		    {
                ExceptionOccured?.Invoke(new Exception($"Unexpected exception occured: cannot get data from {nameof(TimetableInfo)}"));
                return;
		    }

			//updating data
			try
			{
				var timetableAll = TimetableInfo.Timetable;
				var timetable = timetableAll[TimetableInfo.ShortLongTeamNameDictionary[team]];

				TeamName = TimetableInfo.ShortLongTeamNameDictionary[team];
				CurrentTimetable = new ObservableCollection<Event>(timetable);
				ShortTeam = team;
				Date = TimetableInfo.Date;
			}
			catch(Exception exception)
			{
                ExceptionOccured?.Invoke(exception);
			}
		}

		#endregion
	}
}