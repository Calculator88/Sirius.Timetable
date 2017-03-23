using Android.App;
using Android.OS;
using Android.Views;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Dialogs
{
	public class LoadingDialog : DialogFragment, ILoadingDialogService
	{
		public const string LoadingTag = "LoadingDialog";
		private readonly FragmentManager _manager;

		public LoadingDialog(FragmentManager manager)
		{
			_manager = manager;
		}

		public void Show()
		{
			Show(_manager, LoadingTag);
		}

		public void Hide()
		{
			Dismiss();
		}

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