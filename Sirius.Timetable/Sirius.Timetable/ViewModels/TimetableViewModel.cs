using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SiriusTimetable.Common.Helpers;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Common.Views;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using Xamarin.Forms;

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

		public Command SelectTeamCommand { get; set; }

		private async Task SelectTeamExecute()
		{
			IsBusy = true;
			SelectTeamCommand.ChangeCanExecute();

			if (!await UpdateInfo(Date))
			{
				IsBusy = false;
				SelectTeamCommand.ChangeCanExecute();
				return;
			}

			var team = await new TeamSelectPage().SelectTeamAsync(TimetableInfo);
			if (String.IsNullOrEmpty(team))
			{
				IsBusy = false;
				SelectTeamCommand.ChangeCanExecute();
				return;
			}

			await UpdateTeam(Date, team);

			IsBusy = false;
			SelectTeamCommand.ChangeCanExecute();
		}

		private async Task SelectDateExecute()
		{
			var date = await _datePicker.SelectedDate();
			if (date == null) return;

			await UpdateTeam(date.Value, ShortTeam);
		}

		private bool SelectTeamCanExecute()
		{
			return !_isBusy;
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
			SelectTeamCommand = new Command(async () => await SelectTeamExecute(), SelectTeamCanExecute);
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
					item.Color = Color.FromHex("#10ff007b");
				else if (endTime < time)
					item.Color = Color.FromHex("#88CBCBCB");
				else
					item.Color = Color.Transparent;
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
				await _alertService.ShowDialog("Упс..", "На сервере произошла ошибка :(", "Ок");
				return false;
			}

		}
		private async Task UpdateTeam(DateTime date, string shortTeam)
		{
			if (!await UpdateInfo(date)) return;

			var dateKey = date.ToString("ddMMyyyy");
			var timetable = TimetableInfo.Timetable[dateKey];
			var currentTimetable = timetable.Teams[TimetableInfo.KeywordDictionary[shortTeam]];
			var collection = currentTimetable.Select(activity => new TimetableItem(activity));
			Timetable = new ObservableCollection<TimetableItem>(collection);

			Header = new TimetableHeader
			{
				Date = $"{date:D}",
				Team = TimetableInfo.KeywordDictionary[shortTeam],
				IsLoaded = true,
				SelectDateCommand = new Command(async () => await SelectDateExecute())
			};

			Date = date;
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
		private readonly IDialogAlertService _alertService = ServiceLocator.GetService<IDialogAlertService>();
		private TimetableModel _model;

		#endregion
	}
}