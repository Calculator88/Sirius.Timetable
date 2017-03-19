using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Common.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace SiriusTimetable.Droid.Controls
{
	public sealed class AdvancedCellControler : LinearLayout, INativeElementView
	{
		public AdvancedCellControler(Context context, AdvancedCell advancedCell) : base(context)
		{
			AdvancedCell = advancedCell;
			var view = ((Activity) context).LayoutInflater.Inflate(Resource.Layout.AdvancedCellView, null);
			StartTextView = view.FindViewById<TextView>(Resource.Id.TextStart);
			
			EndTextView = view.FindViewById<TextView>(Resource.Id.TextEnd);
			TitleTextView = view.FindViewById<TextView>(Resource.Id.TextTitle);
			BusToTextView = view.FindViewById<TextView>(Resource.Id.TextBusTo);
			BusFromTextView = view.FindViewById<TextView>(Resource.Id.TextBusFrom);
			DashTextView = view.FindViewById<TextView>(Resource.Id.TextDash);
			BackLayout = view.FindViewById<LinearLayout>(Resource.Id.Ground);
			PlaceTextView = view.FindViewById<TextView>(Resource.Id.TextPlace);
			Details = view.FindViewById<LinearLayout>(Resource.Id.DetailLine);
			Bus = view.FindViewById<LinearLayout>(Resource.Id.Bus);
			BusHelper1 = view.FindViewById<ImageView>(Resource.Id.DetailHelp1);
			BusHelper2 = view.FindViewById<ImageView>(Resource.Id.DetailHelp2);
			Button = view.FindViewById<Android.Widget.Button>(Resource.Id.ManNot);
			AddView(view, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
		}

		public TextView StartTextView { get; set; }
		public LinearLayout BackLayout { get; set; }
		public TextView DashTextView { get; set; }
		public TextView EndTextView { get; set; }
		public TextView TitleTextView { get; set; }
		public TextView BusToTextView { get; set; }
		public ImageView BusHelper1 { get; set; }
		public ImageView BusHelper2 { get; set; }
		public TextView BusFromTextView { get; set; }
		public TextView PlaceTextView { get; set; }
		public LinearLayout Details { get; set; }
		public LinearLayout Bus { get; set; }
		public AdvancedCell AdvancedCell { get; set; }
		public Element Element => AdvancedCell;

		public Android.Widget.Button Button { get; set; }

		public void UpdateCell(AdvancedCell cell)
		{
			StartTextView.Text = cell.Start;
			EndTextView.Text = cell.End;
			TitleTextView.Text = cell.Title;
			BusToTextView.Text = cell.BusTo;
			BusFromTextView.Text = cell.BusFrom;
			PlaceTextView.Text = cell.Place;

			Details.Visibility = cell.IsSelected ? ViewStates.Visible : ViewStates.Gone;
			PlaceTextView.Visibility = cell.IsPlace ? ViewStates.Visible : ViewStates.Gone;
			Bus.Visibility = cell.IsBus ? ViewStates.Visible : ViewStates.Gone;


			if (Device.Idiom == TargetIdiom.Phone)
			{
				StartTextView.TextSize =
					EndTextView.TextSize = DashTextView.TextSize = TitleTextView.TextSize = cell.PhoneMainTextSize;
				BusToTextView.TextSize = BusFromTextView.TextSize = PlaceTextView.TextSize = Button.TextSize = cell.PhoneDetailTextSize;
				TitleTextView.SetMaxLines(cell.PhoneMaxLines);
			}
			else
			{
				StartTextView.TextSize =
					EndTextView.TextSize = DashTextView.TextSize = TitleTextView.TextSize = cell.TabletMainTextSize;
				BusToTextView.TextSize = BusFromTextView.TextSize = PlaceTextView.TextSize = Button.TextSize = cell.TabletDetailTextSize;
				TitleTextView.SetMaxLines(cell.TabletMaxLines);
			}
			
			BackLayout.SetBackgroundColor(cell.BackgroundColor.ToAndroid());
		}
	}
}