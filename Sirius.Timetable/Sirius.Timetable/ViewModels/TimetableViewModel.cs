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
		#region Constructores

		public TimetableViewModel()
		{
			Init();
		}

		public TimetableViewModel(DateTime date, string team)
		{
			Init();
			Date = date;
			ShortTeam = team;
		}

		#endregion

		#region Commands

		public AsyncCommand SelectTeamCommand { get; set; }

		private async Task SelectTeamExecute()
		{
			_loading.Show();
			await Task.Delay(1000);
			if (!await UpdateInfo(Date))
			{
				_loading.Hide();
				return;
			}
			_loading.Hide();
			var team = await _selectTeam.SelectedTeam(TimetableInfo);
			if (String.IsNullOrEmpty(team))
				return;

			_loading.Show();
			await UpdateTeam(Date, team);
			_loading.Hide();
		}

		private async Task SelectDateExecute()
		{
			var date = await _datePicker.SelectedDate();
			if (date == null) return;

			await UpdateTeam(date.Value, ShortTeam);
		}

		#endregion

		#region Public Properties

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

		#region Private Methods

		private void Init()
		{
			SelectTeamCommand = new AsyncCommand(async () => await SelectTeamExecute());
			_model = new TimetableModel();
			Date = _dateTimeService.GetCurrentTime().Date;
		}

		private void UpdateCurrentAction()
		{
			var time = ServiceLocator.GetService<IDateTimeService>().GetCurrentTime();
			foreach (var item in Timetable)
			{
				var startTime = Date.AddHours(item.Parent.Start.Hour).AddMinutes(item.Parent.Start.Minute);
				var endTime = Date.AddHours(item.Parent.End.Hour).AddMinutes(item.Parent.End.Minute);
				if ((startTime <= time) && (time <= endTime))
					item.Color = 0x10ff007b;
				else if (endTime < time)
					item.Color = 0x79CBCBCB;
				else
					item.Color = 0x00000000;
			}
		}

		private async Task<bool> UpdateInfo(DateTime date)
		{
			try
			{
				if (TimetableInfo != null && TimetableInfo.Date == date.Date) return true;
				var info = await _model.GetTimetableInfo(date);
				TimetableInfo = info;
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				await _alertService.ShowDialog("Упс..", "На сервере произошла ошибка :(", "Ок", null);
				return false;
			}

		}
		private async Task UpdateTeam(DateTime date, string shortTeam)
		{
			if (!await UpdateInfo(date)) return;

			Date = date;

			if(!TimetableInfo.KeywordDictionary.ContainsKey(shortTeam))
			{
				await SelectTeamExecute();
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
				SelectDateCommand = new AsyncCommand(async () => await SelectDateExecute())
			};

			ShortTeam = shortTeam;

			_timer.SetHandler(UpdateCurrentAction);
			UpdateCurrentAction();
		}
		#endregion

		#region Private Fields
		private TimetableHeader _header;
		private bool _isBusy;
		private ObservableCollection<TimetableItem> _timetable;
		private readonly ITimerService _timer = ServiceLocator.GetService<ITimerService>();
		private readonly IDateTimeService _dateTimeService = ServiceLocator.GetService<IDateTimeService>();
		private readonly IDatePickerDialogService _datePicker = ServiceLocator.GetService<IDatePickerDialogService>();
		private readonly ISelectTeamDialogService _selectTeam = ServiceLocator.GetService<ISelectTeamDialogService>();
		private readonly IDialogAlertService _alertService = ServiceLocator.GetService<IDialogAlertService>();
		private readonly ILoadingDialogService _loading = ServiceLocator.GetService<ILoadingDialogService>();
		private TimetableModel _model;

		#endregion
	}
}