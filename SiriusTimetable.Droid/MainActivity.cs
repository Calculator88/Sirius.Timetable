using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
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
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Droid
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

		#endregion

		#region Actvity lifecycle

		protected override void OnCreate(Bundle bundle)
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
		}
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
					SelectTeam();
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
		public void ItemSelected(TimetableItem item)
		{
			//var intent = new Intent(this, typeof(DetailsActivity));
			//var args = new Bundle();
			//TODO
			//args.PutString("TITLE", item.Title);
			//args.PutString("PLACE", item.Place);
			//args.PutString("BUSTO", item.BusTo);
			//args.PutString("BUSFROM", item.BusFrom);
			//args.PutString("BEGINTIME", item.Start);
			//args.PutString("ENDTIME", item.End);
			//intent.PutExtra("ARGS", args);
			//StartActivity(intent);
		}
		public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			var selectedDate = new DateTime(year, month + 1, dayOfMonth);
			_viewModel.Date = selectedDate;
		}
		public void SelectTeamOnChoose(string result)
		{
			_viewModel.UpdateSchedule(result);
		}

		#endregion

		#region Service methods

		public void ShowLoadingFragment()
		{
			var fragment = FragmentManager.FindFragmentByTag<LoadingDialog>(Resources.GetString(Resource.String.TagLoadingDialog));
			if (fragment != null) return;

			new LoadingDialog().Show(FragmentManager, Resources.GetString(Resource.String.TagLoadingDialog));
			FragmentManager.ExecutePendingTransactions();
		}
		public void HideLoadingFragment()
		{
			var fragment = FragmentManager.FindFragmentByTag<LoadingDialog>(Resources.GetString(Resource.String.TagLoadingDialog));
			fragment?.Dismiss();
			FragmentManager.ExecutePendingTransactions();
		}
		public void ShowSelectTeamDialog()
		{
			new SelectTeamDialog()
				.Show(FragmentManager, Resources.GetString(Resource.String.TagSelectTeamDialog));
		}

		#endregion

		#region Private methods

		private void HeaderSelectDateOnClick()
		{
			new DatePickerDialog().Show(FragmentManager, Resources.GetString(Resource.String.TagDatePickerDialog));
		}
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
				var fragment = FragmentManager.FindFragmentByTag<TimetableFragmentFiller>(Resources.GetString(Resource.String.TagTimetableFragment));
				if(fragment == null) return;

				FragmentManager.BeginTransaction()
					.Replace(Resource.Id.TimetableFragment, new TimetableFragmentFiller(),
					Resources.GetString(Resource.String.TagTimetableFragmentFiller))
					.Commit();
			}
			else
			{
				var fragment = FragmentManager.FindFragmentByTag<TimetableFragment>(Resources.GetString(Resource.String.TagTimetableFragment));
				if(fragment == null)
				{
					FragmentManager.BeginTransaction()
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
		private async void SelectTeam()
		{
			ShowLoadingFragment();

			var info = await ServiceLocator.GetService<ITimetableProvider>().GetTimetableInfo(_viewModel.Date);
			var infoOk = info.Build();

			HideLoadingFragment();

			if (!infoOk)
			{
				new DialogAlertService(
					Resources.GetString(Resource.String.AlertTitle),
					Resources.GetString(Resource.String.AlertNoInternetMessage),
					Resources.GetString(Android.Resource.String.Ok), null)
					.Show(FragmentManager, "ALERT");
				return;
			}

			var dateNow = ServiceLocator.GetService<IDateTimeService>().GetCurrentTime();
			if (String.IsNullOrEmpty(info.DownloadedJson) && 
				info.TimetableCacheInfo.Exists && 
				(dateNow - info.TimetableCacheInfo.CreationTime > TimeSpan.FromHours(4)))
			{
				_tempInfo = info;
				new DialogAlertService(
					Resources.GetString(Resource.String.AlertTitle),
					Resources.GetString(Resource.String.AlertStaleCacheMessage),
					Resources.GetString(Android.Resource.String.Yes),
					Resources.GetString(Android.Resource.String.No))
					.Show(FragmentManager, "SC");
				return;
			}

			_viewModel.TimetableInfo = info;

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
					HeaderSelectDateOnClick();
					break;
			}
		}

		#endregion

	}
}