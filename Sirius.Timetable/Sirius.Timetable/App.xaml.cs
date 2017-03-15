using SiriusTimetable.Common.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TimetablePage = SiriusTimetable.Common.Views.TimetablePage;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SiriusTimetable.Common
{
	public partial class App
	{
		public enum Detail
		{
			Timetable
		}

		public App()
		{
			InitializeComponent();
			RegisterPages();
			SetMainPage();
		}

		private void RegisterPages()
		{
			MasterDetailsServices.DetailPages.Add(Detail.Timetable, new TimetablePage());
		}

		public static void SetMainPage()
		{
			Current.MainPage = new NavigationPage(MasterDetailsServices.DetailPages[Detail.Timetable]);
		}
	}
}