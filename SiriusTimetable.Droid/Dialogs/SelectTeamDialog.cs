using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Common.ViewModels;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Droid.Dialogs
{
	public class SelectTeamDialog : DialogFragment, View.IOnClickListener
	{
		#region Private fields

		private const string TeamTag = "TEAM";
		private const string DirectionTag = "DIRECTION";
		private TimetableInfo _info;
		private ISelectTeamDialogResultListener _listener;
		private List<TextView> _numbers;
		private string _selectedDirection;
		private string _selectedGroup;
		private TextView _directionName;
		private readonly ImageView[] _images = new ImageView[4];
		private TextView _groupName;
		private LinearLayout _groups;
		private Button _selectButton;

		#endregion

		#region Fragment lifecycle

		public override void OnAttach(Context context)
		{
			base.OnAttach(context);
			_listener = Activity as ISelectTeamDialogResultListener;
			if (_listener == null) throw new Exception("Activity must be inherited from SelectTeamDialog.ISelectTeamDialogResultListener");
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.SetTitle(Resource.String.ChooseTeam);
			Dialog.SetCanceledOnTouchOutside(true);

			var v = inflater.Inflate(Resource.Layout.SelectTeamDialog, null);
			_groups = v.FindViewById<LinearLayout>(Resource.Id.lay_groups);
			_images[0] = v.FindViewById<ImageView>(Resource.Id.img_science);
			_images[1] = v.FindViewById<ImageView>(Resource.Id.img_sport);
			_images[2] = v.FindViewById<ImageView>(Resource.Id.img_art);
			_images[3] = v.FindViewById<ImageView>(Resource.Id.img_literature);
			_groupName = v.FindViewById<TextView>(Resource.Id.group_name);
			_directionName = v.FindViewById<TextView>(Resource.Id.dir_name);
			_selectButton = v.FindViewById<Button>(Resource.Id.btn_select);
			_selectButton.SetOnClickListener(this);
			v.FindViewById<Button>(Resource.Id.btn_close).SetOnClickListener(this);
			_images[0].SetOnClickListener(this);
			_images[1].SetOnClickListener(this);
			_images[2].SetOnClickListener(this);
			_images[3].SetOnClickListener(this);

			_info = ServiceLocator.GetService<TimetableViewModel>().TimetableInfo;

			if(savedInstanceState == null) return v;

			var dir = savedInstanceState.GetInt(DirectionTag);
			OnClick(new View(Activity) { Id = dir });

			var team = savedInstanceState.GetInt(TeamTag);
			OnClick(new View(Activity) { Id = team });

			return v;
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutInt(DirectionTag, DirectionToId(_selectedDirection));
			outState.PutInt(TeamTag, TeamToId(_selectedGroup));
		}

		#endregion

		#region Private methods

		private void SetGroupsOpacity(int id)
		{
			if(_numbers == null) return;
			foreach(var number in _numbers)
				number.Alpha = number.Id == id ? 1 : 0.25f;
		}
		private string GetDirectionById(int id)
		{
			var el = _images.FirstOrDefault(view => view.Id == id);
			if(el != null) return (string)el.Tag;
			return null;
		}
		private string GetGroupById(int id)
		{
			return _numbers == null
				? null
				: (from number in _numbers where number.Id == id select (string)number.Tag).FirstOrDefault();
		}
		private void OnChooseGroup(int id)
		{
			var group = GetGroupById(id);
			if(_selectedGroup == group)
				return;

			SetGroupsOpacity(id);
			_selectedGroup = group;

			_groupName.Text = _info.KeywordDictionary[_selectedDirection[0] + _selectedGroup];
			_groupName.Visibility = ViewStates.Visible;
			_selectButton.Enabled = true;
		}
		private void OnChooseDirection(int imageId)
		{
			_selectButton.Enabled = false;
			_groupName.Visibility = ViewStates.Gone;
			if(_selectedDirection == GetDirectionById(imageId))
			{
				UpdateImageOpacity(-1);
				_directionName.Visibility = ViewStates.Gone;
				_groups.Visibility = ViewStates.Gone;
				_selectButton.Enabled = false;
				_selectedDirection = null;
				_selectedGroup = null;
				return;
			}

			UpdateImageOpacity(imageId);
			_selectedDirection = GetDirectionById(imageId);
			_directionName.Text = _selectedDirection;

			_numbers = _info.TeamsLiterPossibleNumbers[_selectedDirection[0].ToString()].Select(GetGroupSelector).ToList();
			_groups.RemoveAllViews();
			var lays = new List<LinearLayout>();
			for(var i = 0; i < _numbers.Count; ++i)
			{
				if(((i + 1) % 11 == 0) || (i == 0))
				{
					var lay = new LinearLayout(Application.Context) {Orientation = Orientation.Horizontal};
					lay.SetGravity(GravityFlags.Center);
					lays.Add(lay);
				}
				lays[(i + 1) / 11].AddView(_numbers[i],
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
			}
			foreach(var layout in lays)
				_groups.AddView(layout,
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
			_groups.Visibility = ViewStates.Visible;

			_directionName.Visibility = ViewStates.Visible;
		}
		private TextView GetGroupSelector(string text)
		{
			var selecter = new TextView(Application.Context)
			{
				Text = text,
				Tag = text,
				Id = text.GetHashCode(),
				TextSize = 18,
				Alpha = 0.25f
			};
			selecter.SetPadding(4, 0, 4, 0);
			selecter.SetTextColor(Color.Black);
			selecter.SetOnClickListener(this);
			return selecter;
		}
		private void UpdateImageOpacity(int id)
		{
			foreach(var image in _images)
				image.Alpha = image.Id == id ? 1f : 0.25f;
		}
		private void OnChoose()
		{
			_listener.SelectTeamOnChoose(_selectedDirection[0] + _selectedGroup);
			Dismiss();
		}
		private void OnClose()
		{
			Dismiss();
		}
		private int DirectionToId(string dir)
		{
			foreach(var image in _images)
				if((string)image.Tag == dir) return image.Id;
			return -1;
		}
		private int TeamToId(string team)
		{
			if(_numbers == null) return -1;
			foreach(var number in _numbers)
				if((string)number.Tag == team) return number.Id;
			return -1;
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
				case Resource.Id.img_sport:
				case Resource.Id.img_art:
				case Resource.Id.img_literature:
				case Resource.Id.img_science:
					OnChooseDirection(id);
					break;
				default:
					OnChooseGroup(id);
					break;
			}
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