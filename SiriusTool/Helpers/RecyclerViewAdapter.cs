using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SiriusTool.Framework;
using SiriusTool.Model;

namespace SiriusTool.Helpers
{
	public class RecyclerViewAdapter : RecyclerView.Adapter
	{
		private readonly List<Event> _activities;

		public RecyclerViewAdapter(List<Event> activities)
		{
			_activities = activities;
		}

	    public override int ItemCount => _activities?.Count ?? 0;

	    public event ItemClickedEventHandler ItemClicked;


        public void OnClick(View v)
		{
			var tag = (Holder) v.Tag;
            
			var el = _activities[tag.LayoutPosition];
			ItemClicked?.Invoke(el);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var mHolder = (Holder) holder;

			mHolder.Title.Text = _activities[position].Title;
			mHolder.Title.SetTextSize(ComplexUnitType.Pt, 8);
			mHolder.Title.SetMaxLines(2);

			if(!_activities[position].Start.HasValue)
			{
				mHolder.Times.Visibility = ViewStates.Gone;
			}
			else
			{
				mHolder.Times.Visibility = ViewStates.Visible;

				mHolder.BeginTime.Text = _activities[position].Start.Value.ToString("HH:mm");
				mHolder.BeginTime.SetTextSize(ComplexUnitType.Pt, 8);

				mHolder.EndTime.Text = _activities[position].End.Value.ToString("HH:mm");
				mHolder.EndTime.SetTextSize(ComplexUnitType.Pt, 8);
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TimetableItem, parent, false);
			var holder = new Holder(view)
			{
				Title = view.FindViewById<TextView>(Resource.Id.TextTitle),
				BeginTime = view.FindViewById<TextView>(Resource.Id.TextStart),
				EndTime = view.FindViewById<TextView>(Resource.Id.TextEnd),
				Dash = view.FindViewById<TextView>(Resource.Id.TextDash),
				MainLayout = view.FindViewById<LinearLayout>(Resource.Id.Ground),
				Colored = view.FindViewById<LinearLayout>(Resource.Id.ColoredGround),
				Times = view.FindViewById<LinearLayout>(Resource.Id.Times)				
			};
			view.Tag = holder;
			return holder;
		}

		public class Holder : RecyclerView.ViewHolder
		{
			public Holder(View itemView) : base(itemView)
			{
				View = itemView;
				View.Click += (sender, args) => Click?.Invoke(sender, args);
			}

			public View View { get; set; }
			public LinearLayout Colored { get; set; }
			public TextView BeginTime { get; set; }
			public TextView EndTime { get; set; }
			public TextView Title { get; set; }
			public LinearLayout MainLayout { get; set; }
			public TextView Dash { get; set; }
			public LinearLayout Times { get; set; }

		    public event EventHandler Click;
		}
	}
}