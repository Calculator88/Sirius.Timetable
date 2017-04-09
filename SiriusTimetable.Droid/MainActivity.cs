using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

namespace SiriusTimetable.Droid
{
	public class MainActivity : AppCompatActivity, View.IOnClickListener, TimetableFragment.IOnItemSelected, ISelectTeamDialogService, 
		SelectTeamDialog.ISelectTeamDialogResultListener, ILoadingDialogService, Android.App.DatePickerDialog.IOnDateSetListener, IDialogAlertService,
		DialogAlertService.IDialogAlertResultListener
	{
		#region Private fields

		private LinearLayout _headerLayout;
		private TextView _headerSelDate;
		private TextView _headerText;
		private TimetableViewModel _viewModel;
		private LoadingDialog _loading;

		#endregion

		#region Actvity lifecycle

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView(Resource.Layout.Main);

			FragmentManager.BeginTransaction()
				.Replace(Resource.Id.TimetableFragment, new TimetableFragment(),
					Resources.GetString(Resource.String.TagTimetableFragment))
				.Commit();

			_headerLayout = FindViewById<LinearLayout>(Resource.Id.header);
			_headerText = FindViewById<TextView>(Resource.Id.header_tmName);
			_headerSelDate = FindViewById<TextView>(Resource.Id.header_date);
			_headerSelDate.SetOnClickListener(this);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			RegisterServices();
			_viewModel = ServiceLocator.GetService<TimetableViewModel>();
			_viewModel.PropertyChanged += ViewModelOnPropertyChanged;
			_loading = new LoadingDialog();
			UpdateHeader();
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			_viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
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
			_viewModel.DialogOnNegativeButtonClick(tag);
		}
		public void OnAlertPositiveButtonClick(string tag)
		{
			_viewModel.DialogOnPositiveButtonClick(tag);
		}
		public void ItemSelected(TimetableItem item)
		{
			var intent = new Intent(this, typeof(DetailsActivity));
			intent.PutExtra("TITLE", item.Title);
			intent.PutExtra("PLACE", item.Place);
			intent.PutExtra("BUSTO", item.BusTo);
			intent.PutExtra("BUSFROM", item.BusFrom);
			StartActivity(intent);
		}
		public async void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			var selectedDate = new DateTime(year, month + 1, dayOfMonth);
			await _viewModel.UpdateTeam(selectedDate, _viewModel.ShortTeam);
		}
		public async void SelectTeamOnChoose(string result)
		{
			ServiceLocator.GetService<ILoadingDialogService>().ShowLoadingFragment();
			await _viewModel.UpdateTeam(_viewModel.Date, result);
			ServiceLocator.GetService<ILoadingDialogService>().HideLoadingFragment();
		}

		#endregion

		#region Service methods

		public void ShowLoadingFragment()
		{
			_loading.Show(FragmentManager, Resources.GetString(Resource.String.TagLoadingDialog));
		}
		public void HideLoadingFragment()
		{
			_loading.Dismiss();
		}
		public void ShowSelectTeamDialog()
		{
			new SelectTeamDialog()
				.Show(FragmentManager, Resources.GetString(Resource.String.TagSelectTeamDialog));
		}
		public void ShowAlert(string title, string message, string positiveButton, string negativeButton, string tag)
		{
			new DialogAlertService(title, message, positiveButton, negativeButton)
				.Show(FragmentManager, tag);
		}

		#endregion

		#region Private methods

		private void HeaderSelDateOnClick()
		{
			new DatePickerDialog().Show(FragmentManager, Resources.GetString(Resource.String.TagDatePickerDialog));
		}
		private void RegisterServices()
		{
			ServiceLocator.RegisterService<ISelectTeamDialogService>(this);
			ServiceLocator.RegisterService<ILoadingDialogService>(this);
			ServiceLocator.RegisterService<IDialogAlertService>(this);
		}
		private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs property)
		{
			var propName = property.PropertyName;
			switch(propName)
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
			if(_viewModel.Header == null)
			{
				_headerLayout.Visibility = ViewStates.Gone;
				return;
			}

			_headerLayout.Visibility = ViewStates.Visible;
			_viewModel.Header.PropertyChanged += HeaderOnPropertyChanged;
			_headerText.Text = _viewModel.Header.Team;
			_headerSelDate.Text = _viewModel.Header.Date;
		}
		private void HeaderOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			var propName = propertyChangedEventArgs.PropertyName;
			switch(propName)
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
			var fragment = FragmentManager.FindFragmentByTag<TimetableFragment>(Resources.GetString(Resource.String.TagTimetableFragment));
			fragment?.SetItems(items);
		}
		private async void SelectTeam()
		{
			ServiceLocator.GetService<ILoadingDialogService>().ShowLoadingFragment();
			await Task.Delay(1000);
			if(!await _viewModel.UpdateInfo(_viewModel.Date))
			{
				ServiceLocator.GetService<ILoadingDialogService>().HideLoadingFragment();
				return;
			}
			ServiceLocator.GetService<ILoadingDialogService>().HideLoadingFragment();
			ServiceLocator.GetService<ISelectTeamDialogService>().ShowSelectTeamDialog();
		}

		#endregion

		#region Public methods

		public void OnClick(View v)
		{
			var id = v.Id;
			switch(id)
			{
				case Resource.Id.header_date:
					HeaderSelDateOnClick();
					break;
			}
		}

		#endregion

	}
}