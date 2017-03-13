using Android.App;
using Android.Content.PM;
using Android.OS;
using SiriusTimetable.Common.Services;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
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
		 MainLauncher = true,
		 ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)
	]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{	
			ServiceLocator.RegisterService<IDatePickerDialogService>(new DatePickerDialogService(FragmentManager));
			ServiceLocator.RegisterService<IDateTimeService>(new DateTimeService());
			ServiceLocator.RegisterService<IDialogAlertService>(new DialogAlertService());
			ServiceLocator.RegisterService<IResourceService>(new ResourceService(Resources));
			ServiceLocator.RegisterService<ISelectedTeamCacher>(new SelectedTeamCacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimerService>(new TimerSerice());
			ServiceLocator.RegisterService<ITimetableCacher>(new TimetableCacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimetableDownloader>(new TimetableDownloader());
			ServiceLocator.RegisterService<ITimetableParser>(new TimetableParser());
			ServiceLocator.RegisterService<ITimetableProvider>(new TimetableProvider());
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;
			
			base.OnCreate(bundle);
			
			Forms.Init(this, bundle);
			
			LoadApplication(new App());
		}

		public override void OnBackPressed()
		{
			MoveTaskToBack(true);
		}
	}
}