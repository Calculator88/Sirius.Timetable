using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Common.Models;

namespace SiriusTimetable.Droid.Helpers
{
	public class RecyclerViewAdapter : RecyclerView.Adapter, View.IOnClickListener
	{ 
		private readonly List<TimetableItem> _activities;
		public RecyclerViewAdapter(List<TimetableItem> activities)
		{
			_activities = activities;
		}
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var mHolder = (Holder) holder;
			_activities[position].PropertyChanged += OnPropertyChanged;


			mHolder.Title.Text = _activities[position].Title;
			mHolder.Title.SetTextSize(ComplexUnitType.Pt, 11);
			mHolder.Title.SetMaxLines(_activities[position].IsSelected ? 100 : 2);

			mHolder.BeginTime.Text = _activities[position].Start;
			mHolder.BeginTime.SetTextSize(ComplexUnitType.Pt, 11);

			mHolder.EndTime.Text = _activities[position].End;
			mHolder.EndTime.SetTextSize(ComplexUnitType.Pt, 11);

			mHolder.Bus.Visibility = _activities[position].IsBus ? ViewStates.Visible : ViewStates.Gone;
			
			mHolder.BusFrom.Text = _activities[position].BusFrom;
			mHolder.BusFrom.SetTextSize(ComplexUnitType.Pt, 9f);

			mHolder.BusTo.Text = _activities[position].BusTo;
			mHolder.BusTo.SetTextSize(ComplexUnitType.Pt, 9f);

			mHolder.Place.Visibility = _activities[position].IsPlace ? ViewStates.Visible : ViewStates.Gone;
			mHolder.Place.Text = _activities[position].Place;
			mHolder.Place.SetTextSize(ComplexUnitType.Pt, 9f);

			mHolder.Colored.SetBackgroundColor(new Color(_activities[position].Color));
			mHolder.DetaiLayout.Visibility = _activities[position].IsSelected ? ViewStates.Visible : ViewStates.Gone;
		}

		private void OnPropertyChanged(Object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			var el = (TimetableItem) sender;
			NotifyItemChanged(_activities.IndexOf(el));
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, Int32 viewType)
		{
			var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TimetableItem, parent, false);
			var holder = new Holder(view, this)
			{
				Title = view.FindViewById<TextView>(Resource.Id.TextTitle),
				BeginTime = view.FindViewById<TextView>(Resource.Id.TextStart),
				EndTime = view.FindViewById<TextView>(Resource.Id.TextEnd),
				BusFrom = view.FindViewById<TextView>(Resource.Id.TextBusFrom),
				BusTo = view.FindViewById<TextView>(Resource.Id.TextBusTo),
				DetaiLayout = view.FindViewById<LinearLayout>(Resource.Id.DetailLine),
				Place = view.FindViewById<TextView>(Resource.Id.TextPlace),
				Dash = view.FindViewById<TextView>(Resource.Id.TextDash),
				Bus = view.FindViewById<LinearLayout>(Resource.Id.Bus),
				MainLayout = view.FindViewById<LinearLayout>(Resource.Id.Ground),
				Colored = view.FindViewById<LinearLayout>(Resource.Id.ColoredGround)
			};
			view.Tag = holder;
			return holder;
		}

		public override int ItemCount => _activities?.Count ?? 0;

		public class Holder : RecyclerView.ViewHolder
		{
			public View View { get; set; }
			public LinearLayout DetaiLayout { get; set; }
			public LinearLayout Colored { get; set; }
			public TextView BeginTime { get; set; }
			public TextView EndTime { get; set; }
			public TextView Title { get; set; }
			public TextView Place { get; set; }
			public TextView BusTo { get; set; }
			public TextView BusFrom { get; set; }
			public LinearLayout MainLayout { get; set; }
			public TextView Dash { get; set; }
			public LinearLayout Bus { get; set; }
			public Holder(View itemView, View.IOnClickListener instance) : base(itemView)
			{
				View = itemView;
				View.SetOnClickListener(instance);
			}
		}

		public void OnClick(View v)
		{
			var tag = (Holder) v.Tag;
			_activities[tag.LayoutPosition].IsSelected = !_activities[tag.LayoutPosition].IsSelected;
		}
	}
}