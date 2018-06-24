using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SiriusTool.Dialogs;
using SiriusTool.Fragments;
using SiriusTool.Helpers;
using SiriusTool.Model;
using SiriusTool.Services;
using SiriusTool.ViewModels;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace SiriusTool
{
	public class MainActivity : AppCompatActivity, SelectTeamDialog.ISelectTeamDialogResultListener, Android.App.DatePickerDialog.IOnDateSetListener,
		DialogAlertService.IDialogAlertResultListener, RecyclerViewAdapter.IItemClickListener, RecyclerViewAdapter.IItemLongClickListener
    {
        #region Private fields

	    private RecyclerView _recyclerView;
	    private FloatingActionButton _fabSelectDate;
	    private LinearLayoutManager _manager;
	    private RecyclerViewAdapter _adapter;
        private TimetableViewModel _viewModel;
		private TimetableInfo _tempInfo;
	    private LinearLayout _statusLoadingLayout;
	    private IMenuItem _chooseMenuItem;

		public const string CachedShortTeam = "com.sirius.timetable.MainActivity.CachedShortTeam";

		#endregion

		#region Actvity lifecycle

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView(Resource.Layout.Main);

			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.main_toolbar));
		    _statusLoadingLayout = FindViewById<LinearLayout>(Resource.Id.main_loading_status_layout);
		    _recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler);
		    _fabSelectDate = FindViewById<FloatingActionButton>(Resource.Id.fab_select_date);

		    _manager = new LinearLayoutManager(this);
		    _recyclerView.SetAdapter(null);
		    _recyclerView.SetLayoutManager(_manager);
		    _recyclerView.AddItemDecoration(new DividerItemDecoration(_recyclerView.Context, _manager.Orientation));

            RegisterServices();
			_viewModel = ServiceLocator.GetService<TimetableViewModel>();
			_viewModel.PropertyChanged += ViewModelOnPropertyChanged;
		    _fabSelectDate.Click += ViewOnClick;

			UpdateVMLinks();

            SelectTeamOnClick();
		}

	    private void ViewOnClick(object sender, EventArgs eventArgs)
	    {
	        var id = ((View)sender).Id;
	        switch (id)
	        {
	            case Resource.Id.fab_select_date:
	                ShowDatePicker();
	                break;
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

        public void ItemClick(Event item)
        {
            ItemSelected(item);
        }
        public void ItemLongClick(Event item)
        {
        }

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

            _viewModel.StartDateUpdatingTimetable(selectedDate);
		    await ValidateTimetableInfo();
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

        private async Task<bool> ValidateTimetableInfo()
        {
            try
            {
                await _viewModel.GetTimetable();
            }
            catch (WebException)
            {
                new AlertDialog.Builder(this)
                    .SetMessage(
                        "Произошла ошибка при загрузке данных. Проверте ваше подключение и повторите попытку")
                    .SetTitle("Error")
                    .Show();
                return false;
            }
            catch (Exception)
            {
                new AlertDialog.Builder(this)
                    .SetMessage("Непредвиденная ошибка")
                    .SetTitle("Error")
                    .Show();
                return false;
            }

            return true;
        }
        private void VMOnTimetableChanged()
		{
		    SetItems(_viewModel.CurrentTimetable?.ToList());
		}
        private void VMOnDateChanged()
        {
            if (!String.IsNullOrWhiteSpace(_viewModel.ShortTeam))
                Title = $"{_viewModel.ShortTeam} - {_viewModel.Date:M}";
        }
		private void VMOnTeamNameChanged()
		{
		    if (!String.IsNullOrWhiteSpace(_viewModel.ShortTeam))
		        Title = $"{_viewModel.ShortTeam} - {_viewModel.Date:M}";
        }
        private void VMOnShortNameChanged()
		{
		    if (!String.IsNullOrWhiteSpace(_viewModel.ShortTeam))
		        Title = $"{_viewModel.ShortTeam} - {_viewModel.Date:M}";
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
	            _fabSelectDate.Enabled = false;
	            Title = null;
            }
	        else
	        {
	            _statusLoadingLayout.Visibility = ViewStates.Gone;
                Title = 
                    !String.IsNullOrWhiteSpace(_viewModel.ShortTeam) ? 
                        $"{_viewModel.ShortTeam} - {_viewModel.Date:M}" : 
                        GetString(Resource.String.MainActivityTitle);

	            _chooseMenuItem?.SetEnabled(true);
	            _fabSelectDate.Enabled = true;
	        }
        }
	    private async void SelectTeamOnClick()
	    {
	        if (await ValidateTimetableInfo())
			    ShowSelectTeamDialog();
		}

        #endregion

        #region Public methods

        public void SetItems(List<Event> items)
        {
            _adapter = new RecyclerViewAdapter(items, this, this);
            _recyclerView.SetAdapter(_adapter);
        }

        #endregion

    }
}