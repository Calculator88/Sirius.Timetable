using Android.OS;
using Android.Views;

namespace SiriusTimetable.Droid.Dialogs
{
	public class LoadingDialog : Android.Support.V7.App.AppCompatDialogFragment
	{
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.loading, null);
			Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
			Cancelable = false;
			Dialog.SetCanceledOnTouchOutside(false);
			return v;
		}
	}
}