using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SiriusTimetable.Common.Helpers;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Common.ViewModels
{
	public class TimetableViewModel : ObservableObject
	{
		#region Private fields

		private TimetableHeader _header;
		private bool _isBusy;
		private ObservableCollection<TimetableItem> _timetable;
		private TimetableModel _model;

		#endregion

		#region Constructors

		public TimetableViewModel()
		{
			Init();
		}
		#endregion

		#region Public properties

		public TimetableInfo TimetableInfo { get; private set; }
		public string ShortTeam { get; set; }
		public TimetableHeader Header
		{
			get { return _header; }
			set { SetProperty(ref _header, value); }
		}
		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}
		public DateTime Date { get; set; }
		public ObservableCollection<TimetableItem> Timetable
		{
			get { return _timetable; }
			set { SetProperty(ref _timetable, value); }
		}

		#endregion

		#region Private methods

		private void Init()
		{
			_model = new TimetableModel();
			Date = ServiceLocator.GetService<IDateTimeService>().GetCurrentTime().Date;
		}
		private void UpdateCurrentAction()
		{
			var time = ServiceLocator.GetService<IDateTimeService>().GetCurrentTime();
			foreach(var item in Timetable)
			{
				var startTime = Date.AddHours(item.Parent.Start.Hour).AddMinutes(item.Parent.Start.Minute);
				var endTime = Date.AddHours(item.Parent.End.Hour).AddMinutes(item.Parent.End.Minute);
				if((startTime <= time) && (time <= endTime))
					item.Color = 0x10ff007b;
				else if(endTime < time)
					item.Color = 0x79CBCBCB;
				else
					item.Color = 0x00000000;
			}
		}

		#endregion

		#region Public methods

		public void DialogOnPositiveButtonClick(string tag)
		{
			if(tag == "StaleCache")
			{
				ServiceLocator.GetService<ITimetableProvider>().StaleDialogSetOnPositive();
			}
		}

		public void DialogOnNegativeButtonClick(string tag)
		{
			if(tag == "StaleCache")
			{
				ServiceLocator.GetService<ITimetableProvider>().StaleDialogSetOnNegative();
			}
		}
		public async Task<bool> UpdateInfo(DateTime date)
		{
			try
			{
				if ((TimetableInfo != null) && (TimetableInfo.Date == date.Date)) return true;
				var info = await _model.GetTimetableInfo(date);
				TimetableInfo = info;
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				ServiceLocator.GetService<IDialogAlertService>().ShowDialog("Упс..", "На сервере произошла ошибка :(", "Ок", null, "SError");
				return false;
			}
		}

		public async Task UpdateTeam(DateTime date, string shortTeam)
		{
			if (!await UpdateInfo(date)) return;

			Date = date;

			if (!TimetableInfo.KeywordDictionary.ContainsKey(shortTeam))
			{
				ServiceLocator.GetService<ISelectTeamDialogService>().ShowDialog();
				return;
			}

			var dateKey = Date.ToString("ddMMyyyy");
			var timetable = TimetableInfo.Timetable[dateKey];
			var currentTimetable = timetable.Teams[TimetableInfo.KeywordDictionary[shortTeam]];
			var collection = currentTimetable.Select(activity => new TimetableItem(activity));
			Timetable = new ObservableCollection<TimetableItem>(collection);

			Header = new TimetableHeader
			{
				Date = $"{Date:D}",
				Team = TimetableInfo.KeywordDictionary[shortTeam],
				IsLoaded = true,
			};

			ShortTeam = shortTeam;

			ServiceLocator.GetService<ITimerService>().SetHandler(UpdateCurrentAction);
			UpdateCurrentAction();
		}

		#endregion
	}
}