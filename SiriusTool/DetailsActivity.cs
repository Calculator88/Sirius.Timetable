using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using SiriusTool.Fragments;

namespace SiriusTool
{
	public class DetailsActivity : AppCompatActivity
	{
		#region Activiy lifecycle

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
			SetContentView(Resource.Layout.Details);

			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);


			var details = new DetailsFragment {Arguments = Intent.GetBundleExtra("ARGS")};

			SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.DetailsFragment, details,
					Resources.GetString(Resource.String.TagDetailsFragment))
				.Commit();
		}
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					OnBackPressed();
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		#endregion
	}
}