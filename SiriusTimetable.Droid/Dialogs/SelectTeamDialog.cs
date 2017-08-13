using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Timetable;
using Android.Support.V7.App;
using Android.Util;
using Android.Views.Animations;
using SiriusTimetable.Droid.Helpers;

namespace SiriusTimetable.Droid.Dialogs
{
	public class SelectTeamDialog : AppCompatDialogFragment, View.IOnClickListener , ViewSwitcher.IViewFactory
	{
		#region Private fields

		private string _selectedDirection;
		private string _selectedGroup;
		private TimetableInfo _info;
		private ISelectTeamDialogResultListener _listener;

		private List<TextView> _numbers;
		private List<FrameLayout> _directions;
		private TextSwitcher _groupName;
		private LinearLayout _groups;
		private Button _selectButton;
		private TextSwitcher _directionName;

		#endregion

		#region Fragment lifecycle

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as ISelectTeamDialogResultListener;
			_info = ServiceLocator.GetService<TimetableViewModel>().TimetableInfo;

			var dataExists = _info.ShortLongTeamNameDictionary != null && 
				(_info.DirectionPossibleNumbers != null || _info.UnknownPossibleTeams != null);
			if(!dataExists) Dismiss();
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.SetCanceledOnTouchOutside(true);

			var v = inflater.Inflate(Resource.Layout.SelectTeamDialog, container, false);
			
			_groups = v.FindViewById<LinearLayout>(Resource.Id.lay_groups);
			
			var directionsLayout = v.FindViewById<LinearLayout>(Resource.Id.directionContainer);
			_directions = new List<FrameLayout>();
			foreach (var direction in _info.DirectionPossibleNumbers.Keys)
			{
				var frame = new FrameLayout(Context)
				{
					Alpha = 0.25f,
					Clickable = true,
					Tag = new[] { direction, direction },	  //short tag, full tag
					Id = -3
				};
				frame.SetOnClickListener(this);
				frame.SetBackgroundDrawable(GetRippleBackground());

				var picture = new ImageView(Context);
				picture.SetImageResource(Resource.Drawable.img_background);
				var size = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 55, Resources.DisplayMetrics);
				frame.AddView(picture, new FrameLayout.LayoutParams(size, size));

				var text = new TextView(Context)
				{
					Text = direction,
					Gravity = GravityFlags.Center
				};
				text.SetTextSize(ComplexUnitType.Dip, 20);
				frame.AddView(text, new FrameLayout.LayoutParams(size, size));

				_directions.Add(frame);
				directionsLayout.AddView(frame,
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
					{
						LeftMargin = 5,
						RightMargin = 5
					});
			}
			if (_info.UnknownPossibleTeams.Count != 0)
			{
				var frame = new FrameLayout(Context)
				{
					Clickable = true,
					Alpha = 0.25f,
					Tag = new[] { "", "Неизвестно" },
					Id = -3
				};
				frame.SetOnClickListener(this);
				frame.SetBackgroundDrawable(GetRippleBackground());

				var dip = new DipConverter(Context);

				var picture = new ImageView(Context);
				picture.SetImageResource(Resource.Drawable.unknown);

				frame.AddView(picture, new FrameLayout.LayoutParams(dip.ToDip(55), dip.ToDip(55)));

				_directions.Add(frame);
				directionsLayout.AddView(frame,
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
					{
						LeftMargin = dip.ToDip(10),
						RightMargin = dip.ToDip(10)
					});
			}
			ObtainDirections();

			_groupName = v.FindViewById<TextSwitcher>(Resource.Id.group_name);
			_groupName.SetFactory(this);

			_directionName = v.FindViewById<TextSwitcher>(Resource.Id.dir_name);
			_directionName.SetFactory(this);

			_selectButton = v.FindViewById<Button>(Resource.Id.btn_select);
			_selectButton.SetOnClickListener(this);

			v.FindViewById<Button>(Resource.Id.btn_close).SetOnClickListener(this);

			var inSlideAnimation = AnimationUtils.LoadAnimation(Context, Android.Resource.Animation.SlideInLeft);
			var outSlideAnimation = AnimationUtils.LoadAnimation(Context, Android.Resource.Animation.SlideOutRight);
			var inFadeAnimation = AnimationUtils.LoadAnimation(Context, Android.Resource.Animation.FadeIn);
			var outFadeAnimation = AnimationUtils.LoadAnimation(Context, Android.Resource.Animation.FadeOut);

			inSlideAnimation.Duration = 80;
			outSlideAnimation.Duration = 80;
			inFadeAnimation.Duration = 150;
			outFadeAnimation.Duration = 150;

			_groupName.InAnimation = inSlideAnimation;
			_groupName.OutAnimation = outSlideAnimation;
			_directionName.InAnimation = inFadeAnimation;
			_directionName.OutAnimation = outFadeAnimation;
			
			return v;
		}
		private void ObtainDirections()
		{
			foreach (var frameLayout in _directions)
			{
				var tags = (string[]) frameLayout.Tag;
				switch (tags[0])
				{
					case "Н":
						tags[1] = "Наука";
						((ImageView) frameLayout.GetChildAt(0)).SetImageResource(Resource.Drawable.science);
						frameLayout.RemoveViewAt(1);
						frameLayout.Tag = tags;
						break;
					case "Л":
						tags[1] = "Литература";
						((ImageView)frameLayout.GetChildAt(0)).SetImageResource(Resource.Drawable.literature);
						frameLayout.RemoveViewAt(1);
						frameLayout.Tag = tags;
						break;
					case "И":
						tags[1] = "Искусство";
						((ImageView)frameLayout.GetChildAt(0)).SetImageResource(Resource.Drawable.art);
						frameLayout.RemoveViewAt(1);
						frameLayout.Tag = tags;
						break;
					case "С":
						tags[1] = "Спорт";
						((ImageView)frameLayout.GetChildAt(0)).SetImageResource(Resource.Drawable.sport);
						frameLayout.RemoveViewAt(1);
						frameLayout.Tag = tags;
						break;
				}
			}
		}
		#endregion

		#region Private methods

		private void SetGroupsOpacity(string tag)
		{
			if(_numbers == null) return;
			foreach(var number in _numbers)
				number.Alpha = (string)number.Tag == tag ? 1 : 0.25f;
		}
		private void OnChooseDirection(string[] tags)
		{
			if (tags == null) return;

			_selectButton.Enabled = false;
			_groupName.Visibility = ViewStates.Gone;

			if(_selectedDirection == tags[0])
			{
				UpdateImageOpacity(null);
				_directionName.Visibility = ViewStates.Gone;
				_groups.Visibility = ViewStates.Gone;
				_selectButton.Enabled = false;
				_selectedDirection = null;
				_selectedGroup = null;
				return;
			}

			UpdateImageOpacity(tags);
			_selectedDirection = tags[0];
			_directionName.SetText(tags[1]);
			_selectedGroup = null;

			if(_selectedDirection == "")
				_numbers = _info.UnknownPossibleTeams.Select(GetGroupSelector).ToList();
			else
				_numbers = _info.DirectionPossibleNumbers[_selectedDirection]
					.Select(item => item.ToString("00"))
					.Select(GetGroupSelector).ToList();

			_groups.RemoveAllViews();

			for(var it = 0; it < _numbers.Count; )
			{
				var lay = new LinearLayout(Context);
				lay.SetGravity(GravityFlags.CenterHorizontal);				
				for(var summSumb = 0; summSumb < 19 && it < _numbers.Count; ++it)
				{
					lay.AddView(_numbers[it], new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
					summSumb += _numbers[it].Text.Length;
				}
				_groups.AddView(lay, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			}

			_groups.Visibility = ViewStates.Visible;
			_directionName.Visibility = ViewStates.Visible;
		}
		private void OnChooseGroup(string group)
		{
			if(_selectedGroup == group)
				return;

			SetGroupsOpacity(group);
			_selectedGroup = group;

			_groupName.SetText(_info.ShortLongTeamNameDictionary[_selectedDirection + _selectedGroup]);

			_groupName.Visibility = ViewStates.Visible;
			_selectButton.Enabled = true;
		}
		private TextView GetGroupSelector(string text)
		{
			var selector = new TextView(Context)
			{
				Text = text,
				Tag = text,
				Id = -2,
				TextSize = 18,
				Alpha = 0.25f
			};
			selector.SetPadding(4, 0, 4, 0);
			selector.SetTextColor(Color.Black);
			selector.SetOnClickListener(this);

			selector.SetBackgroundDrawable(GetRippleBackground());
			return selector;
		}

		private Drawable GetRippleBackground()
		{
			var attrs = new[]{ Android.Resource.Attribute.SelectableItemBackground};
			var ta = Activity.ObtainStyledAttributes(attrs);
			var selectedItemDrawable = ta.GetDrawable(0);
			ta.Recycle();
			return selectedItemDrawable;
		}
		private void UpdateImageOpacity(string[] tag)
		{
			foreach (var image in _directions)
			{
				var tag2 = (string[])image.Tag;
				image.Alpha = tag != null && tag[0] == tag2[0] && tag[1] == tag2[1] ? 1f : 0.25f;
			}
		}
		private void OnChoose()
		{
			_listener.SelectTeamOnChoose(_selectedDirection + _selectedGroup);
			Dismiss();
		}
		private void OnClose()
		{
			Dismiss();
		}

		#endregion

		#region Public methods

		public void OnClick(View v)
		{
			var id = v.Id;
			if(id == -1) return;
			switch(id)
			{
				case Resource.Id.btn_select:
					OnChoose();
					break;
				case Resource.Id.btn_close:
					OnClose();
					break;
				case -3:
					OnChooseDirection((string[])v.Tag);
					break;
				case -2:
					OnChooseGroup((string)v.Tag);
					break;
			}
		}

		public View MakeView()
		{
			var textView = new TextView(Context)
			{
				Gravity = GravityFlags.CenterHorizontal | GravityFlags.Center
			};
			var dip = new DipConverter(Context);
			textView.SetPadding(dip.ToDip(10), 0, dip.ToDip(10), 0);
			return textView;
		}

		#endregion

		#region Interaction interfaces

		public interface ISelectTeamDialogResultListener
		{
			void SelectTeamOnChoose(string result);
		}

		#endregion
	}
}