using System;
using System.Collections.ObjectModel;
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

			if (TimetableInfo == null)
			{
				var info = await _model.GetTimetableInfo(Date);
				if (info == null)
				{
					IsBusy = false;
					SelectTeamCommand.ChangeCanExecute();
					return;
				}
				TimetableInfo = info;
			}

			var team = await new TeamSelectPage().SelectTeamAsync(TimetableInfo);
			if (String.IsNullOrEmpty(team))
			{
				IsBusy = false;
				SelectTeamCommand.ChangeCanExecute();
				return;
			}

			ShortTeam = team;
			UpdateTeam(Date);

			IsBusy = false;
			SelectTeamCommand.ChangeCanExecute();
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
		public string Team => TimetableInfo.KeywordDictionary[ShortTeam];
		#endregion

		#region Private Methods
		private void Init()
		{
			SelectTeamCommand = new Command(async () => await SelectTeamExecute(), SelectTeamCanExecute);
			_model = new TimetableModel();
			Date = _dateTimeService.GetCurrentTime();
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

		private void UpdateTeam(DateTime date)
		{
			Date = date;
			var dateKey = Date.ToString("ddMMyyyy");
			var timetable = TimetableInfo.Timetable[dateKey];
			var currentTimetable = timetable.Teams[TimetableInfo.KeywordDictionary[ShortTeam]];
			var collection = currentTimetable.Select(activity => new TimetableItem(activity));
			Timetable = new ObservableCollection<TimetableItem>(collection);

			Header = new TimetableHeader
			{
				Date = $"{Date:D}",
				Team = TimetableInfo.KeywordDictionary[ShortTeam],
				IsLoaded = true
			};

			UpdateCurrentAction();
		}
		#endregion

		#region Private Fields
		private TimetableHeader _header;
		private bool _isBusy;
		private ObservableCollection<TimetableItem> _timetable;
		private readonly ITimerService _timer = ServiceLocator.GetService<ITimerService>();
		private readonly IDateTimeService _dateTimeService = ServiceLocator.GetService<IDateTimeService>();
		private TimetableModel _model;

		#endregion
	}
}