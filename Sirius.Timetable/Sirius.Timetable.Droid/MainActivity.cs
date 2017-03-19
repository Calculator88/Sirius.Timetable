using Android.App;
using Android.Content.PM;
using Android.OS;
using SiriusTimetable.Common.Services;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Droid.Dialogs;
using SiriusTimetable.Droid.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using App = SiriusTimetable.Common.App;
using DateTimeService = SiriusTimetable.Droid.Services.DateTimeService;

namespace SiriusTimetable.Droid
{
	[Activity(
		 Label = "Расписание Сириус",
		 Theme = "@style/MyTheme",
		 ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)
	]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{	
			RegisterServices();

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;
			
			base.OnCreate(bundle);
			
			Forms.Init(this, bundle);
			
			LoadApplication(new App());
		}

		private void RegisterServices()
		{
			ServiceLocator.RegisterService<IDatePickerDialogService>(new DatePickerDialogService(FragmentManager));
			ServiceLocator.RegisterService<IDateTimeService>(new DateTimeServiceFake());
			ServiceLocator.RegisterService<IDialogAlertService>(new DialogAlertService());
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
		public override void OnBackPressed()
		{
			MoveTaskToBack(true);
		}
	}
}