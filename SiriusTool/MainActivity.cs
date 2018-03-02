using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using SiriusTool.Dialogs;
using SiriusTool.Fragments;
using SiriusTool.Helpers;
using SiriusTool.Model;
using SiriusTool.Services;
using SiriusTool.Services.Abstractions;
using SiriusTool.ViewModels;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace SiriusTool
{
	public class MainActivity : AppCompatActivity, View.IOnClickListener, TimetableFragment.IOnItemSelected, ISelectTeamDialogService, 
		SelectTeamDialog.ISelectTeamDialogResultListener, ILoadingDialogService, Android.App.DatePickerDialog.IOnDateSetListener,
		DialogAlertService.IDialogAlertResultListener
	{
		#region Private fields

		private TextView _headerSelDate;
		private TextView _headerText;
		private TimetableViewModel _viewModel;
		private TimetableInfo _tempInfo;

		public const string ISTEAMCACHED = "ISTEAMCACHED";
		public const string CACHEDTEAMNAME = "CACHEDTEAM";
		public const string CACHEDSHORTTEAM = "SHORTTEAM";

		#endregion

		#region Actvity lifecycle

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView(Resource.Layout.Main);


			_headerText = FindViewById<TextView>(Resource.Id.header_tmName);
			_headerSelDate = FindViewById<TextView>(Resource.Id.header_date);
			_headerSelDate.SetOnClickListener(this);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			RegisterServices();
			_viewModel = ServiceLocator.GetService<TimetableViewModel>();
			_viewModel.PropertyChanged += ViewModelOnPropertyChanged;

			UpdateVMLinks();

			if(Intent.GetBooleanExtra(ISTEAMCACHED, false))
			{
				var infoGot = await LoadTimetableInfo();
				var team = Intent.GetStringExtra(CACHEDTEAMNAME);
				var shortTeam = Intent.GetStringExtra(CACHEDSHORTTEAM);
				if(infoGot && _viewModel.TimetableInfo.Timetable.ContainsKey(team))
					SelectTeamOnChoose(shortTeam);
			}
		}
		protected override void OnDestroy()
		{
			_viewModel.PropertyChanged -= ViewModelOnPropertyChanged;

			base.OnDestroy();
		}
		protected override void OnPause()
		{
			base.OnPause();
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
					_tempInfo = null;
					break;
				default:
					break;
			}
		}
		public void OnAlertPositiveButtonClick(string tag)
		{
			switch(tag)
			{
				case "SC":
					_viewModel.TimetableInfo = _tempInfo;
					_tempInfo = null;
					ShowSelectTeamDialog();
					break;
				default:
					break;
			}
		}
		public void ItemSelected(Event item)
		{
			var intent = new Intent(this, typeof(DetailsActivity));
			var args = new Bundle();

			args.PutString(DetailsFragment.TitleTextTag, item.Title);
			args.PutString(DetailsFragment.BeginTimeTag, item.Start?.ToString("HH:mm"));
			args.PutString(DetailsFragment.EndTimeTag, item.End?.ToString("HH:mm"));
			intent.PutExtra("ARGS", args);

			StartActivity(intent);
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
		}

		#endregion

		#region Service methods

		public void ShowLoadingFragment()
		{
			var fragment = (LoadingDialog)SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagLoadingDialog));
			if (fragment != null) return;

			new LoadingDialog().Show(SupportFragmentManager, Resources.GetString(Resource.String.TagLoadingDialog));
			FragmentManager.ExecutePendingTransactions();
		}
		public void HideLoadingFragment()
		{
			var fragment = (LoadingDialog)SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagLoadingDialog));
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

		private void RegisterServices()
		{
			ServiceLocator.RegisterService<ISelectTeamDialogService>(this);
			ServiceLocator.RegisterService<ILoadingDialogService>(this);
		}
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
				var fragment = (TimetableFragmentFiller)SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagTimetableFragment));
				if(fragment == null) return;

				SupportFragmentManager.BeginTransaction()
					.Replace(Resource.Id.TimetableFragment, new TimetableFragmentFiller(),
					Resources.GetString(Resource.String.TagTimetableFragmentFiller))
					.Commit();
			}
			else
			{
				var fragment = (TimetableFragment)SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagTimetableFragment));
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
			_headerText.Visibility = _viewModel.TeamName == null ? ViewStates.Gone : ViewStates.Visible;
			_headerText.Text = _viewModel.TeamName;
		}
		private void VMOnShortNameChanged()
		{
			//TODO
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

		    var timetable = await ServiceLocator.GetService<ITimetableProvider>().RequestTimetable(null, null);

		    HideLoadingFragment();

            TimetableInfo info;


		    try
		    {
		        info = new TimetableInfo(timetable, _viewModel.Date);
		    }
		    catch (Exception ex)
		    {
		        Log.Error("SiriusTool", ex.Message);
		        new DialogAlertService(
		                Resources.GetString(Resource.String.AlertTitle),
		                Resources.GetString(Resource.String.AlertNoInternetMessage),
		                Resources.GetString(Android.Resource.String.Ok), null)
		            .Show(SupportFragmentManager, "ALERT");
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
			}
		}

		#endregion

	}
}