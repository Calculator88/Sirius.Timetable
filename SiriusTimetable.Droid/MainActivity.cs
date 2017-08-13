using System;
using System.ComponentModel;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Droid.Dialogs;
using SiriusTimetable.Droid.Fragments;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Threading.Tasks;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;

namespace SiriusTimetable.Droid
{
	public class MainActivity : AppCompatActivity, 
		View.IOnClickListener, 
		TimetableFragment.IOnItemSelected, 
		ISelectTeamDialogService, 
		SelectTeamDialog.ISelectTeamDialogResultListener, 
		ILoadingDialogService, 
		Android.App.DatePickerDialog.IOnDateSetListener,
		DialogAlertService.IDialogAlertResultListener
	{
		#region Private fields

		private TextView _headerSelDate;
		private TextView _headerText;
		private TimetableViewModel _viewModel;
		private DrawerLayout _drawer;

		public const string IsTeamCached = "IsTeamCached";
		public const string CachedTeamName = "CachedTeam";
		public const string CachedShortTeam = "ShortTeam";
		public const string SelectedItem = "SelectedItem";

		#endregion

		#region Actvity lifecycle

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView(Resource.Layout.Main);
			RegisterComponents();
			RegisterServices();

			_viewModel = ServiceLocator.GetService<TimetableViewModel>();
			_viewModel.PropertyChanged += ViewModelOnPropertyChanged;

			UpdateVMLinks();

			if (!Intent.GetBooleanExtra(IsTeamCached, false)) return;
			var infoGot = await LoadTimetableInfo();
			var team = Intent.GetStringExtra(CachedTeamName);
			var shortTeam = Intent.GetStringExtra(CachedShortTeam);
			if(infoGot && _viewModel.TimetableInfo.Timetable.ContainsKey(team))
				SelectTeamOnChoose(shortTeam);
		}

		#region Helpers

		private void RegisterComponents()
		{
			_headerText = FindViewById<TextView>(Resource.Id.AppTitle);
			_headerText.Selected = true;
			_headerSelDate = FindViewById<TextView>(Resource.Id.header_date);
			_headerSelDate.SetOnClickListener(this);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			_drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

			FindViewById<NavigationView>(Resource.Id.nav_view).InflateMenu(Resource.Menu.drawerMenu);
			var toggle = new ActionBarDrawerToggle(this, _drawer, toolbar, Resource.String.TxtSport, Resource.String.TxtLiterature);
			_drawer.AddDrawerListener(toggle);	
			_drawer.SetStatusBarBackgroundColor(ContextCompat.GetColor(this, Resource.Color.PrimaryDark));
			_drawer.SetOnClickListener(this);
			toggle.SyncState();
		}
		private void RegisterServices()
		{
			ServiceLocator.RegisterService<ISelectTeamDialogService>(this);
			ServiceLocator.RegisterService<ILoadingDialogService>(this);
		}

		#endregion

		protected override void OnDestroy()
		{
			_viewModel.PropertyChanged -= ViewModelOnPropertyChanged;

			base.OnDestroy();
		}
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
			return true;
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch(item.ItemId)
			{
				case Resource.Id.btn_selectTeam:
					SelectTeamOnClick();
					break;
			}
			return true;
		}

		#endregion

		#region Fragment listeners

		public void OnAlertNegativeButtonClick(string tag)
		{
			switch(tag)
			{
				case "SC":
					_viewModel.TempTimetableInfo = null;
					if(_viewModel.TimetableInfo != null && _viewModel.Date != _viewModel?.TimetableInfo.Date)
						_viewModel.Date = _viewModel.TimetableInfo.Date;
					break;
			}
		}
		public void OnAlertPositiveButtonClick(string tag)
		{
			switch(tag)
			{
				case "SC":
					_viewModel.TimetableInfo = _viewModel.TempTimetableInfo;
					_viewModel.TempTimetableInfo = null;
					ShowSelectTeamDialog();
					break;
			}
		}
		public void ItemSelected(TimetableItem item)
		{
			var args = new Bundle();
			args.PutString(DetailsDialog.TitleTextTag, item.Title);
			args.PutString(DetailsDialog.PlaceTextTag, item.Place);
			args.PutString(DetailsDialog.BeginTimeTag, item.Start?.ToString("HH:mm"));
			args.PutString(DetailsDialog.EndTimeTag, item.End?.ToString("HH:mm"));

			var bottomDialog = new DetailsDialog { Arguments = args };
			bottomDialog.Show(SupportFragmentManager, "DT");
		}
		public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			var selectedDate = new DateTime(year, month + 1, dayOfMonth);
			_viewModel.Date = selectedDate;
			if(_viewModel.Date.Date != _viewModel.TimetableInfo?.Date.Date)
				SelectTeamOnClick();
		}
		public void SelectTeamOnChoose(string result)
		{
			_viewModel.UpdateSchedule(result);
			if (String.IsNullOrEmpty(_viewModel.TeamName) || _viewModel.Date.Date !=
			    ServiceLocator.GetService<IDateTimeService>().GetCurrentTime().Date) return;
			ServiceLocator.GetService<ICacher>().Cache(ServiceLocator.GetService<ICacher>().CacheDirectory + CachedTeamName,
				_viewModel.TeamName);
			ServiceLocator.GetService<ICacher>().Cache(ServiceLocator.GetService<ICacher>().CacheDirectory + CachedShortTeam,
				_viewModel.ShortTeam);
		}

		#endregion

		#region Service methods

		public void ShowLoadingFragment()
		{
			var fragment = SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagLoadingDialog)) as LoadingDialog;
			if (fragment != null) return;

			new LoadingDialog().Show(SupportFragmentManager, Resources.GetString(Resource.String.TagLoadingDialog));
			FragmentManager.ExecutePendingTransactions();
		}
		public void HideLoadingFragment()
		{
			var fragment = SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagLoadingDialog)) as LoadingDialog;
			fragment?.Dismiss();
			FragmentManager.ExecutePendingTransactions();
		}
		public void ShowSelectTeamDialog()
		{
			new SelectTeamDialog()
				.Show(SupportFragmentManager, Resources.GetString(Resource.String.TagSelectTeamDialog));
		}
		private void ShowDatePicker()
		{
			new DatePickerDialog(_viewModel.Date).Show(SupportFragmentManager, Resources.GetString(Resource.String.TagDatePickerDialog));
		}

		#endregion

		#region Private methods

		private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs property)
		{
			var propName = property.PropertyName;
			switch(propName)
			{
				case nameof(_viewModel.Timetable):
					VMOnTimetableChanged();
					break;
				case nameof(_viewModel.Date):
					VMOnDateChanged();
					break;
				case nameof(_viewModel.TeamName):
					VMOnTeamNameChanged();
					break;
				case nameof(_viewModel.ShortTeam):
					VMOnShortNameChanged();
					break;
			}
		}
		private void VMOnTimetableChanged()
		{
			if(_viewModel.Timetable == null)
			{
				SupportFragmentManager.BeginTransaction()
					.Replace(Resource.Id.TimetableFragment, new TimetableFragmentFiller(),
					Resources.GetString(Resource.String.TagTimetableFragmentFiller))
					.Commit();
			}
			else
			{
				var fragment = SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagTimetableFragment)) as TimetableFragment;
				if(fragment == null)
				{
					SupportFragmentManager.BeginTransaction()
						.Replace(Resource.Id.TimetableFragment,
							new TimetableFragment(),
							Resources.GetString(Resource.String.TagTimetableFragment))
						.Commit();
				}
			}
		}
		private void VMOnDateChanged()
		{
			_headerSelDate.Text = $"{_viewModel.Date:D}";
		}
		private void VMOnTeamNameChanged()
		{
			if(String.IsNullOrEmpty(_viewModel.TeamName))
			{
				_headerText.Text = null;
				Title = Resources.GetString(Resource.String.main_title);
			}
			else
			{
				_headerText.Text = _viewModel.TeamName;
				Title = "";
			}
		}
		private void VMOnShortNameChanged()
		{
		}
		private void UpdateVMLinks()
		{
			VMOnDateChanged();
			VMOnShortNameChanged();
			VMOnTeamNameChanged();
			VMOnTimetableChanged();
		}
		private async Task<bool> LoadTimetableInfo()
		{
			if(_viewModel.Date.Date == _viewModel.TimetableInfo?.Date.Date)
				return true;

			ShowLoadingFragment();

			var info = await ServiceLocator.GetService<ITimetableProvider>().GetTimetableInfo(_viewModel.Date);
			var infoOk = info.Build();

			HideLoadingFragment();

			if(!infoOk)
			{
				new DialogAlertService(
					Resources.GetString(Resource.String.AlertTitle),
					Resources.GetString(Resource.String.AlertNoInternetMessage),
					Resources.GetString(Android.Resource.String.Ok), null)
					.Show(SupportFragmentManager, "ALERT");
				if(_viewModel.TimetableInfo != null && _viewModel.Date != _viewModel?.TimetableInfo.Date)
					_viewModel.Date = _viewModel.TimetableInfo.Date;
				return false;
			}

			_viewModel.Timetable = null;
			_viewModel.TeamName = null;

			var dateNow = ServiceLocator.GetService<IDateTimeService>().GetCurrentTime();
			if(String.IsNullOrEmpty(info.DownloadedJson) &&
				info.TimetableCacheInfo.Exists &&
				(dateNow - info.TimetableCacheInfo.CreationTime > TimeSpan.FromHours(4)))
			{
				_viewModel.TempTimetableInfo = info;
				new DialogAlertService(
					Resources.GetString(Resource.String.AlertTitle),
					Resources.GetString(Resource.String.AlertStaleCacheMessage),
					Resources.GetString(Android.Resource.String.Yes),
					Resources.GetString(Android.Resource.String.No))
					.Show(SupportFragmentManager, "SC");
				return false;
			}

			_viewModel.TimetableInfo = info;
			return true;
		}
		private async void SelectTeamOnClick()
		{
			var infoGot = await LoadTimetableInfo();
			if (infoGot)
				ShowSelectTeamDialog();
		}
		
		#endregion

		#region Public methods

		public void OnClick(View v)
		{
			var id = v.Id;
			switch(id)
			{
				case Resource.Id.header_date:
					ShowDatePicker();
					break;
				case Resource.Id.nav_schedule:
					
					break;
				case Resource.Id.nav_settings:
					break;
			}
		}

		#endregion

	}
}