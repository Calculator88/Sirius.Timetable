using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
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
	public class MainActivity : AppCompatActivity, TimetableFragment.IOnItemSelected, ISelectTeamDialogService, 
		SelectTeamDialog.ISelectTeamDialogResultListener, Android.App.DatePickerDialog.IOnDateSetListener,
		DialogAlertService.IDialogAlertResultListener
	{
		#region Private fields

		private TextView _headerSelDate;
		private TextView _headerText;
		private TimetableViewModel _viewModel;
		private TimetableInfo _tempInfo;
	    private LinearLayout _statusLoadingLayout;
	    private IMenuItem _chooseMenuItem;

		public const string IsTeamCached = "com.sirius.timetable.MainActivity.IsTeamCached";
		public const string CachedTeamName = "com.sirius.timetable.MainActivity.CachedTeamName";
		public const string CachedShortTeam = "com.sirius.timetable.MainActivity.CachedShortTeam";

		#endregion

		#region Actvity lifecycle

		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView(Resource.Layout.Main);

			_headerText = FindViewById<TextView>(Resource.Id.header_tmName);
			_headerSelDate = FindViewById<TextView>(Resource.Id.header_date);
            _headerSelDate.Click += ViewOnClick;
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
		    _statusLoadingLayout = FindViewById<LinearLayout>(Resource.Id.main_loading_status_layout);

			RegisterServices();
			_viewModel = ServiceLocator.GetService<TimetableViewModel>();
			_viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.ExceptionOccured += VMOnExceptionOccured;

			UpdateVMLinks();

		    await _viewModel.GetTimetable(_viewModel.Date);
		}

	    private void ViewOnClick(object sender, EventArgs eventArgs)
	    {
	        var id = ((View)sender).Id;
	        switch (id)
	        {
	            case Resource.Id.header_date:
	                ShowDatePicker();
	                break;
	        }
        }

        private void VMOnExceptionOccured(Exception exception)
	    {
	        new AlertDialog.Builder(this)
	            .SetMessage(exception.Message)
	            .SetTitle("Error")
	            .Show();
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
		    _chooseMenuItem = menu.FindItem(Resource.Id.btn_selectTeam);
		    if (_viewModel.IsBusy)
		        _chooseMenuItem.SetEnabled(false);
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
		public async void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
		{
			var selectedDate = new DateTime(year, month + 1, dayOfMonth);

		    await _viewModel.GetTimetable(selectedDate);

            if (_viewModel.Date == selectedDate)
				SelectTeamOnClick();
		}
		public void SelectTeamOnChoose(string result)
		{
			_viewModel.UpdateSchedule(result);
		}

		#endregion

		#region Service methods

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
		}
		private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs property)
		{
			var propName = property.PropertyName;
			switch(propName)
			{
				case nameof(_viewModel.CurrentTimetable):
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
                case nameof(_viewModel.IsBusy):
                    VMOnIsBusyChanged();
                    break;
			}
		}
		private void VMOnTimetableChanged()
		{
			if(_viewModel.CurrentTimetable == null)
			{
				var fragment = (TimetableFragmentFiller)SupportFragmentManager.FindFragmentByTag(Resources.GetString(Resource.String.TagTimetableFragmentFiller));
				if(fragment != null) return;

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
                    var ttfragment = new TimetableFragment();
                    ttfragment.PickDate += TimetableFragmentOnPickDate;
					SupportFragmentManager.BeginTransaction()
						.Replace(Resource.Id.TimetableFragment,
					        ttfragment,
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
            VMOnIsBusyChanged();
		}
	    private void VMOnIsBusyChanged()
	    {
	        if (_viewModel.IsBusy)
	        {
	            _statusLoadingLayout.Visibility = ViewStates.Visible;
	            _chooseMenuItem?.SetEnabled(false);
	            Title = null;
            }
	        else
	        {
	            _statusLoadingLayout.Visibility = ViewStates.Gone;
	            Title = GetString(Resource.String.MainActivityTitle);
	            _chooseMenuItem?.SetEnabled(true);
	        }
        }
	    private async void SelectTeamOnClick()
	    {
	        if (_viewModel.TimetableInfo == null)
	            await _viewModel.GetTimetable(_viewModel.Date);
            if (_viewModel.TimetableInfo != null)
			    ShowSelectTeamDialog();
		}

	    private void TimetableFragmentOnPickDate()
	    {
            ShowDatePicker();
	    }

        #endregion

        #region Public methods

        #endregion

    }
}