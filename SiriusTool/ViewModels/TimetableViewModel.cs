using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
	    private DateTime _updatingDate;
		private string _teamName;
		private string _shortTeam;
	    private bool _isBusy;
	    private readonly TimetableFactory _timetableFactory;
	    private readonly ITimetableProvider _timetableProvider;
	    private readonly TimeSpan _timetableLifeTime = TimeSpan.FromMinutes(10);
	    private bool _isUpdating;

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

		#endregion

		#region Private methods


		#endregion

		#region Public methods

	    public TimetableInfo GetTemporaryTimetableInfo()
	    {
	        if (_isUpdating) return _timetableFactory.GetInfo(_updatingDate.Date);
	        return _timetableFactory.GetInfo(Date.Date);
	    }
	    public bool ExistsInfo()
	    {
	        return _timetableFactory.TimetableExists(_isUpdating ? _updatingDate : Date);
	    }

        /// <summary>
        /// Начинает цикл обновления расписания для новой выбранной даты. 
        /// Следующий вызов функции GetTimetable получит расписание для этой заданной даты</summary>
        /// <param name="date">Дата для обновления</param>
        public void StartDateUpdatingTimetable(DateTime date)
	    {
	        _isUpdating = true;
	        _updatingDate = date;
	    }

        /// <summary>
        /// Обновляет данные в TimetableFactory для текущей даты, но не меняет текущих значений, в случае ошибки
        /// </summary>
        /// <param name="forceNet">true, если нужно загрузить данные из интернета принудительно</param>
        /// <returns></returns>
	    public async Task GetTimetable(bool forceNet = false)
        {
            var date = _isUpdating ? _updatingDate : Date;

	        if (!forceNet && _timetableFactory.TimetableExists(date.Date) && 
	            DateTime.UtcNow - _timetableFactory.GetCreationTime(date.Date) < _timetableLifeTime)
	        {
	            if (ShortTeam != null) UpdateSchedule(ShortTeam);
                return;
            }


	        IsBusy = true;

            try
            {
                var timetable = await _timetableProvider.RequestTimetable(date, null);
                var info = new TimetableInfo(timetable, date.Date);
                _timetableFactory.ForceAdd(date.Date, info);
            }
            finally
            {
	            IsBusy = false;
            }
            

            if (ShortTeam != null) UpdateSchedule(ShortTeam);
	    }

        /// <summary>
        /// Обновляет текущие данные для текущей даты или даты обновления для указнной команды
        /// </summary>
        /// <param name="team">Короткое имя команды</param>
        public void UpdateSchedule(string team)
		{
		    var date = _isUpdating ? _updatingDate : Date;
		    TimetableInfo = _timetableFactory.GetInfo(date.Date);
		    Date = date;

			//updating data
			var timetableAll = TimetableInfo.Timetable;
			var timetable = timetableAll[TimetableInfo.ShortLongTeamNameDictionary[team]];

			TeamName = TimetableInfo.ShortLongTeamNameDictionary[team];
			CurrentTimetable = new ObservableCollection<Event>(timetable);
			ShortTeam = team;

		    _isUpdating = false;
		}

		#endregion
	}
}