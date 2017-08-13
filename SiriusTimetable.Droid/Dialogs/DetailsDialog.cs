using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.OS;
using Android.Support.V4.App;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DetailsDialog : DialogFragment
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
		private DialogAlertService.IDialogAlertResultListener _listener;

		private string _title;
		private string _place;
		private string _beginTime;
		private string _endTime;

		#endregion

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as DialogAlertService.IDialogAlertResultListener;
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.DetailsFragment, container, false);
			_titleTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityTitle);
			_placeTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityPlace);
			_beginTimeTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityBeginTime);
			_endTimeTextView = v.FindViewById<TextView>(Resource.Id.DetailsActivityEndTime);

			_title = Arguments?.GetString(TitleTextTag) ?? savedInstanceState?.GetString(TitleTextTag);
			_place = Arguments?.GetString(PlaceTextTag) ?? savedInstanceState?.GetString(PlaceTextTag);
			_beginTime = Arguments?.GetString(BeginTimeTag) ?? savedInstanceState?.GetString(BeginTimeTag);
			_endTime = Arguments?.GetString(EndTimeTag) ?? savedInstanceState?.GetString(EndTimeTag);

			_titleTextView.SetText(_title, TextView.BufferType.Normal);

			_placeTextView.SetText(
				String.IsNullOrEmpty(_place)
					? Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Place)}: </i></b><i>{Resources.GetString(Resource.String.PlaceNone)}</i>")
					: Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Place)}: </i></b><span>{_place}</span>"),
				TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_beginTime))
				_beginTimeTextView.Visibility = ViewStates.Gone;
			else _beginTimeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Begin)}: </i></b><span>{_beginTime}</span>"),
				TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_endTime))
				_endTimeTextView.Visibility = ViewStates.Gone;
			else _endTimeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.End)}: </i></b><span>{_endTime}</span>"),
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
		public void Update(Bundle item)
		{
			_title = item.GetString(TitleTextTag);
			_place = item.GetString(PlaceTextTag);
			_beginTime = item.GetString(BeginTimeTag);
			_endTime = item.GetString(EndTimeTag);

			_titleTextView.SetText(_title, TextView.BufferType.Normal);

			_placeTextView.SetText(
				String.IsNullOrEmpty(_place)
					? Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Place)} </i></b><i>Нет</i>")
					: Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Place)} </i></b><span>{_place}</span>"),
				TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_beginTime))
				_beginTimeTextView.Visibility = ViewStates.Gone;
			else _beginTimeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.Begin)} </i></b><span>{_beginTime}</span>"),
				TextView.BufferType.Spannable);

			if(String.IsNullOrEmpty(_endTime))
				_endTimeTextView.Visibility = ViewStates.Gone;
			else _endTimeTextView.SetText(Html.FromHtml($"<b><i>{Resources.GetString(Resource.String.End)} </i></b><span>{_endTime}</span>"),
				TextView.BufferType.Spannable);

		}
		public override void OnDismiss(IDialogInterface dialog)
		{
			_listener.OnAlertNegativeButtonClick(Tag);
			base.OnDismiss(dialog);
		}
	}
}