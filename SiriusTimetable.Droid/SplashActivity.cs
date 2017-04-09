using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Droid.Services;

namespace SiriusTimetable.Droid
{
	public class SplashActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RegisterServices();
			StartActivity(new Intent(Application.Context, typeof(MainActivity)));
			Finish();
		}
		private void RegisterServices()
		{
			ServiceLocator.RegisterService<IDateTimeService>(new DateTimeServiceFake());
			ServiceLocator.RegisterService<IResourceService>(new ResourceService(Resources));
			ServiceLocator.RegisterService<ISelectedTeamCacher>(new SelectedTeamCacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimerService>(new TimerSerice());
			ServiceLocator.RegisterService<ITimetableCacher>(new TimetableCacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimetableDownloader>(new TimetableDownloaderFake());
			ServiceLocator.RegisterService<ITimetableParser>(new TimetableParser());
			ServiceLocator.RegisterService<ITimetableProvider>(new TimetableProvider());
			ServiceLocator.RegisterService<ILocalNotificationService>(new LocalNotificationService());
			ServiceLocator.RegisterService(new TimetableViewModel());
		}
	}
}