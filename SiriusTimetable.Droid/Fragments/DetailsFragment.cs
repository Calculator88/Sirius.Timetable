using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

namespace SiriusTimetable.Droid.Fragments
{
	public class DetailsFragment : Fragment
	{
		#region Private fields

		private const string TitleTextTag = "TITLE";
		private const string PlaceTextTag = "PLACE";
		private const string BusToTag = "BUSTO";
		private const string BusFromTag = "BUSFROM";
		private const string BeginTimeTag = "BEGINTIME";
		private const string EndTimeTag = "ENDTIME";

		private TextView _titleTextView;
		private TextView _placeTextView;
		private TextView _beginTimeTextView;
		private TextView _endTimeTextView;
		private LinearLayout _busContainer;
		private LinearLayout _placeContainer;
		private TextView _busToTextView;
		private TextView _busFromTextView;

		private string _title;
		private string _place;
		private string _busTo;
		private string _busFrom;
		private string _beginTime;
		private string _endTime;

		#endregion

		#region Fragment lifecycle

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.DetailsFragment, container, false);
			_titleTextView = v.FindViewById<TextView>(Resource.Id.ActivityTitleView);
			_placeTextView = v.FindViewById<TextView>(Resource.Id.ActivityPlaceView);
			_busContainer = v.FindViewById<LinearLayout>(Resource.Id.BusLayout);
			_busToTextView = v.FindViewById<TextView>(Resource.Id.ActivityBusTo);
			_busFromTextView = v.FindViewById<TextView>(Resource.Id.ActivityBusFrom);
			_beginTimeTextView = v.FindViewById<TextView>(Resource.Id.ActivityBeginTime);
			_endTimeTextView = v.FindViewById<TextView>(Resource.Id.ActivityEndTime);
			_placeContainer = v.FindViewById<LinearLayout>(Resource.Id.ActivityPlaceContainer);
																		  
			_title = Arguments?.GetString(TitleTextTag) ?? savedInstanceState.GetString(TitleTextTag);
			_place = Arguments?.GetString(PlaceTextTag) ?? savedInstanceState.GetString(PlaceTextTag);
			_beginTime = Arguments?.GetString(BeginTimeTag) ?? savedInstanceState.GetString(BeginTimeTag);
			_endTime = Arguments?.GetString(EndTimeTag) ?? savedInstanceState.GetString(EndTimeTag);
			_busTo = Arguments?.GetString(BusToTag) ?? savedInstanceState.GetString(BusToTag);
			_busFrom = Arguments?.GetString(BusFromTag) ?? savedInstanceState.GetString(BusFromTag);

			_titleTextView.Text = _title;
			_placeTextView.Text = _place;
			_placeContainer.Visibility = String.IsNullOrEmpty(_place) ? ViewStates.Gone : ViewStates.Visible;
			_beginTimeTextView.Text = _beginTime;
			_endTimeTextView.Text = _endTime;
			_busToTextView.Text = _busTo;
			_busFromTextView.Text = _busFrom;
			_busContainer.Visibility = String.IsNullOrEmpty(_busTo) ? ViewStates.Gone : ViewStates.Visible;
			return v;
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutString(TitleTextTag, _title);
			outState.PutString(PlaceTextTag, _place);
			outState.PutString(BusToTag, _busTo);
			outState.PutString(BusFromTag, _busFrom);
			outState.PutString(BeginTimeTag, _beginTime);
			outState.PutString(EndTimeTag, _endTime);
		}

		#endregion
	}
}