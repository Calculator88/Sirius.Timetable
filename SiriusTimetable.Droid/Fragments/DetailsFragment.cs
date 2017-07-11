using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Text;

namespace SiriusTimetable.Droid.Fragments
{
	public class DetailsFragment : Fragment
	{
		#region Private fields

		public const string TitleTextTag = "TITLE";
		public const string PlaceTextTag = "PLACE";
		public const string BusToTag = "BUSTO";
		public const string BusFromTag = "BUSFROM";
		public const string BeginTimeTag = "BEGINTIME";
		public const string EndTimeTag = "ENDTIME";

		private TextView _titleTextView;
		private TextView _placeTextView;
		private TextView _beginTimeTextView;
		private TextView _endTimeTextView;

		private string _title;
		private string _place;
		private string _beginTime;
		private string _endTime;

		#endregion

		#region Fragment lifecycle

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.DetailsFragment, container, false);
			_titleTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityTitle);
			_placeTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityPlace);
			_beginTimeTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityBeginTime);
			_endTimeTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityEndTime);
																		  
			_title = Arguments?.GetString(TitleTextTag) ?? savedInstanceState.GetString(TitleTextTag);
			_place = Arguments?.GetString(PlaceTextTag) ?? savedInstanceState.GetString(PlaceTextTag);
			_beginTime = Arguments?.GetString(BeginTimeTag) ?? savedInstanceState.GetString(BeginTimeTag);
			_endTime = Arguments?.GetString(EndTimeTag) ?? savedInstanceState.GetString(EndTimeTag);

			_titleTextView.SetText(Html.FromHtml(_title), TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_place))
				_placeTextView.Visibility = ViewStates.Gone;
			_placeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Place)} </i></b><span>{_place}</span>"), 
				TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_beginTime))
				_beginTimeTextView.Visibility = ViewStates.Gone;
			_beginTimeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Begin)} </i></b><span>{_beginTime}</span>"), 
				TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_endTime))
				_endTimeTextView.Visibility = ViewStates.Gone;
			_endTimeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.End)} </i></b><span>{_endTime}</span>"), 
				TextView.BufferType.Spannable);

			return v;
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutString(TitleTextTag, _title);
			outState.PutString(PlaceTextTag, _place);
			outState.PutString(BeginTimeTag, _beginTime);
			outState.PutString(EndTimeTag, _endTime);
		}

		#endregion
	}
}