using Android.App;
using Android.OS;
using Android.Views;

namespace SiriusTimetable.Droid.Fragments
{
	public class TimetableFragmentFiller : Fragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.TimetableFragmentFiller, container, false);
		}
	}
}