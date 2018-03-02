using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using SiriusTool.Services;
using SiriusTool.Services.Abstractions;
using SiriusTool.ViewModels;

namespace SiriusTool
{
	public class SplashActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RegisterServices();

			var intent = new Intent(Application.Context, typeof(MainActivity));
			StartActivity(intent);
			Finish();
		}
		private void RegisterServices()
		{
			ServiceLocator.RegisterService<IDateTimeService>(new DateTimeService());
			ServiceLocator.RegisterService<IResourceService>(new ResourceService(Resources));
			ServiceLocator.RegisterService<ITimerService>(new TimerService());
			ServiceLocator.RegisterService<ICacher>(new Cacher(CacheDir.Path));
			ServiceLocator.RegisterService<ITimetableDownloader>(new TimetableDownloader());
			ServiceLocator.RegisterService<IJsonParser>(new JsonParser());
			ServiceLocator.RegisterService<ITimetableProvider>(new TimetableProvider());
			ServiceLocator.RegisterService<ILocalNotificationService>(new LocalNotificationService());
			ServiceLocator.RegisterService(new TimetableViewModel(ServiceLocator.GetService<IDateTimeService>().GetCurrentTime()));
		}
	}
}