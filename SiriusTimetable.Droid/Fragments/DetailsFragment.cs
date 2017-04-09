using System;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace SiriusTimetable.Droid.Fragments
{
	public class DetailsFragment : Fragment
	{
		#region Private fields

		private const string TitleTextTag = "TITLE";
		private const string PlaceTextTag = "PLACE";
		private const string BusToTag = "BUSTO";
		private const string BusFromTag = "BUSFROM";

		private TextView _titleTextView;
		private TextView _placeTextView;
		private LinearLayout _busLayout;
		private TextView _busToTextView;
		private TextView _busFromTextView;

		private string _title;
		private string _place;
		private string _busTo;
		private string _busFrom;

		#endregion

		#region Fragment lifecycle

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.DetailsFragment, container, false);
			_titleTextView = v.FindViewById<TextView>(Resource.Id.ActivityTitleView);
			_placeTextView = v.FindViewById<TextView>(Resource.Id.ActivityPlaceView);
			_busLayout = v.FindViewById<LinearLayout>(Resource.Id.BusLayout);
			_busToTextView = v.FindViewById<TextView>(Resource.Id.ActivityBusTo);
			_busFromTextView = v.FindViewById<TextView>(Resource.Id.ActivityBusFrom);

			if (savedInstanceState == null)
			{
				_titleTextView.SetText(Html.FromHtml(_title), TextView.BufferType.Spannable);
				_placeTextView.SetText(Html.FromHtml(_place), TextView.BufferType.Spannable);
				_busToTextView.SetText(Html.FromHtml(_busTo ?? ""), TextView.BufferType.Spannable);
				_busFromTextView.SetText(Html.FromHtml(_busFrom ?? ""), TextView.BufferType.Spannable);
			}
			else
			{
				_titleTextView.SetText(Html.FromHtml(savedInstanceState.GetString(TitleTextTag)), TextView.BufferType.Spannable);
				_placeTextView.SetText(Html.FromHtml(savedInstanceState.GetString(PlaceTextTag)), TextView.BufferType.Spannable);
				_busToTextView.SetText(Html.FromHtml(savedInstanceState.GetString(BusToTag) ?? ""), TextView.BufferType.Spannable);
				_busFromTextView.SetText(Html.FromHtml(savedInstanceState.GetString(BusFromTag) ?? ""), TextView.BufferType.Spannable);

			}
			_busLayout.Visibility = String.IsNullOrEmpty(_busToTextView.Text) ? ViewStates.Gone : ViewStates.Visible;
			return v;
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutString(TitleTextTag, _titleTextView.Text);
			outState.PutString(PlaceTextTag, _placeTextView.Text);
			outState.PutString(BusToTag, _busToTextView.Text);
			outState.PutString(BusFromTag, _busFromTextView.Text);
		}

		#endregion

		#region Public methods

		public void SetData(string title, string place, string busTo, string busFrom)
		{
			_title = $"<b><i>Событие: </i></b>{title}";
			_place = $"<b><i>Место: </i></b>{place}";
			_busTo = String.IsNullOrEmpty(busTo) ? null : $"<b><i>Автобус на событие: </i></b>{busTo}";
			_busFrom = String.IsNullOrEmpty(busFrom) ? null : $"<b><i>Автобус с события: </i></b>{busFrom}";
		}

		#endregion
	}
}