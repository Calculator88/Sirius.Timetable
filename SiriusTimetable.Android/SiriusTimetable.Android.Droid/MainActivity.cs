using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Common.Models;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Droid.Dialogs;
using SiriusTimetable.Droid.Helpers;
using SiriusTimetable.Droid.Services;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace SiriusTimetable.Droid
{
	[Activity (Label = "Раписание", Theme = "@style/MyTheme", Icon = "@drawable/logo", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : AppCompatActivity, View.IOnClickListener
	{
		private LinearLayoutManager _manager;
		private LinearLayout _headerLayout;
		private TextView _headerText;
		private TextView _headerSelDate;
		private void RegisterServices()
		{
			ServiceLocator.RegisterService<IDatePickerDialogService>(new Dialogs.DatePickerDialog(FragmentManager));
			ServiceLocator.RegisterService<IDateTimeService>(new DateTimeServiceFake());
			ServiceLocator.RegisterService<IDialogAlertService>(new DialogAlertService(this));
			ServiceLocator.RegisterService<IResourceService>(new ResourceService(Resources));
			ServiceLocator.RegisterService<ISelectedTeamCacher>(new SelectedTeamCacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimerService>(new TimerSerice());
			ServiceLocator.RegisterService<ITimetableCacher>(new TimetableCacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimetableDownloader>(new TimetableDownloaderFake());
			ServiceLocator.RegisterService<ITimetableParser>(new TimetableParser());
			ServiceLocator.RegisterService<ITimetableProvider>(new TimetableProvider());
			ServiceLocator.RegisterService<ILocalNotificationService>(new LocalNotificationService());
			ServiceLocator.RegisterService<ISelectTeamDialogService>(new SelectTeamDialog(SupportFragmentManager));
			ServiceLocator.RegisterService<ILoadingDialogService>(new LoadingDialog(FragmentManager));
		}
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView (Resource.Layout.Main);
			_headerLayout = FindViewById<LinearLayout>(Resource.Id.header);
			_headerText = FindViewById<TextView>(Resource.Id.header_tmName);
			_headerSelDate = FindViewById<TextView>(Resource.Id.header_date);
			_headerSelDate.SetOnClickListener(this);

			RegisterServices();
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			_recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
			_manager = new LinearLayoutManager(this);
			_recyclerView.SetLayoutManager(_manager);
			_adapter = new RecyclerViewAdapter(null);
			_recyclerView.SetAdapter(_adapter);
			_recyclerView.AddItemDecoration(new DividerItemDecoration(_recyclerView.Context, _manager.Orientation));

			_viewModel = new TimetableViewModel();
			_viewModel.PropertyChanged += ViewModelOnPropertyChanged;
			_viewModel.SelectTeamCommand.CanExecuteChanged += SelectTeamCommandOnCanExecuteChanged;
		}

		private void HeaderSelDateOnClick()
		{
			_viewModel.Header.SelectDateCommand.Execute(null);
		}

		private void SelectTeamCommandOnCanExecuteChanged(Object sender, EventArgs eventArgs)
		{
			_selectTeamMenuItem.SetEnabled(_viewModel.SelectTeamCommand.CanExecute(null));
		}

		private RecyclerViewAdapter _adapter;
		private void ViewModelOnPropertyChanged(Object sender, PropertyChangedEventArgs property)
		{
			var propName = property.PropertyName;
			switch (propName)
			{
				case nameof(_viewModel.Timetable):
					UpdateAdapter(_viewModel.Timetable.ToList());
					break;
				case nameof(_viewModel.Header):
					UpdateHeader();
					break;
			}
		}

		private void UpdateHeader()
		{
			if (_viewModel.Header == null)
			{
				_headerLayout.Visibility = ViewStates.Gone;
				return;
			}
			_headerLayout.Visibility = ViewStates.Visible;
			_viewModel.Header.PropertyChanged += HeaderOnPropertyChanged;
			_headerText.Text = _viewModel.Header.Team;
			_headerSelDate.Text = _viewModel.Header.Date;
		}

		private void HeaderOnPropertyChanged(Object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			var propName = propertyChangedEventArgs.PropertyName;
			switch (propName)
			{
				case nameof(_viewModel.Header.Team):
					_headerText.Text = _viewModel.Header.Team;
					break;
				case nameof(_viewModel.Header.Date):
					_headerSelDate.Text = _viewModel.Header.Date;
					break;
			}
		}

		private void UpdateAdapter(List<TimetableItem> items)
		{
			_adapter = new RecyclerViewAdapter(items);
			_recyclerView.SetAdapter(_adapter);
		}

		private void SelectTeam()
		{
			_viewModel.SelectTeamCommand.Execute(null);
		}
		private RecyclerView _recyclerView;
		private IMenuItem _selectTeamMenuItem;
		private TimetableViewModel _viewModel;
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.btn_selectTeam:
					SelectTeam();
					break;
			}
			return true;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
			_selectTeamMenuItem = menu.FindItem(Resource.Id.btn_selectTeam);
			return true;
		}

		public void OnClick(View v)
		{
			var id = v.Id;
			switch (id)
			{
				case Resource.Id.header_date:
					HeaderSelDateOnClick();
					break;
			}
		}
	}
}