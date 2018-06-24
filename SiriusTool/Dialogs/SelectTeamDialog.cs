using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SiriusTool.Helpers;
using SiriusTool.Services;
using SiriusTool.ViewModels;

namespace SiriusTool.Dialogs
{
	public class SelectTeamDialog : AppCompatDialogFragment, View.IOnClickListener
	{
		#region Private fields

		private const string TeamTag = "TEAM";
		private const string DirectionTag = "DIRECTION";

		private string _selectedDirection;
		private string _selectedGroup;
		private TimetableInfo _info;
		private ISelectTeamDialogResultListener _listener;

		private List<TextView> _numbers;
		private readonly ImageView[] _images = new ImageView[5];
		private TextView _groupName;
		private LinearLayout _groups;
		private Button _selectButton;
		private TextView _directionName;

		#endregion

		#region Fragment lifecycle

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as ISelectTeamDialogResultListener;
			_info = ServiceLocator.GetService<TimetableViewModel>().GetTemporaryTimetableInfo();

			var dataExists = _info.ShortLongTeamNameDictionary != null && 
				(_info.DirectionPossibleNumbers != null || _info.UnknownPossibleTeams != null);
			if(!dataExists) Dismiss();
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.SetCanceledOnTouchOutside(true);

			var v = inflater.Inflate(Resource.Layout.SelectTeamDialog, null, false);
			
			_groups = v.FindViewById<LinearLayout>(Resource.Id.lay_groups);

			_images[0] = v.FindViewById<ImageView>(Resource.Id.img_science);
			if(!_info.DirectionPossibleNumbers.ContainsKey("Í") || _info.DirectionPossibleNumbers["Í"].Count == 0)
				_images[0].Visibility = ViewStates.Gone;

			_images[1] = v.FindViewById<ImageView>(Resource.Id.img_sport);
			if(!_info.DirectionPossibleNumbers.ContainsKey("Ñ") || _info.DirectionPossibleNumbers["Ñ"].Count == 0)
				_images[1].Visibility = ViewStates.Gone;

			_images[2] = v.FindViewById<ImageView>(Resource.Id.img_art);
			if(!_info.DirectionPossibleNumbers.ContainsKey("È") || _info.DirectionPossibleNumbers["È"].Count == 0)
				_images[2].Visibility = ViewStates.Gone;

			_images[3] = v.FindViewById<ImageView>(Resource.Id.img_literature);
			if(!_info.DirectionPossibleNumbers.ContainsKey("Ë") || _info.DirectionPossibleNumbers["Ë"].Count == 0)
				_images[3].Visibility = ViewStates.Gone;

			_images[4] = v.FindViewById<ImageView>(Resource.Id.img_unknown);
			if(_info.UnknownPossibleTeams == null || _info.UnknownPossibleTeams.Count == 0)
				_images[4].Visibility = ViewStates.Gone;

			_groupName = v.FindViewById<TextView>(Resource.Id.group_name);
			_directionName = v.FindViewById<TextView>(Resource.Id.dir_name);
			_selectButton = v.FindViewById<Button>(Resource.Id.btn_select);
			_selectButton.SetOnClickListener(this);
			v.FindViewById<Button>(Resource.Id.btn_close).SetOnClickListener(this);

			_images[0].SetOnClickListener(this);   
			_images[1].SetOnClickListener(this);
			_images[2].SetOnClickListener(this);
			_images[3].SetOnClickListener(this);
			_images[4].SetOnClickListener(this);
			


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

			if(_selectedDirection == Resources.GetString(Resource.String.TxtUnknown))
				_groupName.Text = _info.ShortLongTeamNameDictionary[group];
			else
				_groupName.Text = _info.ShortLongTeamNameDictionary[_selectedDirection[0] + _selectedGroup];

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
			_selectedGroup = null;

			if(_selectedDirection == Resources.GetString(Resource.String.TxtUnknown))
				_numbers = _info.UnknownPossibleTeams.Select(GetGroupSelector).ToList();
			else
				_numbers = _info.DirectionPossibleNumbers[_selectedDirection[0].ToString()]
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
		private TextView GetGroupSelector(string text)
		{
			var selecter = new TextView(Context)
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

			var attrs = new[]{ Android.Resource.Attribute.SelectableItemBackground};
			var ta = Activity.ObtainStyledAttributes(attrs);
			var selectedItemDrawable = ta.GetDrawable(0);
			ta.Recycle();
			selecter.Background = selectedItemDrawable;
			return selecter;
		}
		private void UpdateImageOpacity(int id)
		{
			foreach(var image in _images)
				image.Alpha = image.Id == id ? 1f : 0.25f;
		}
		private void OnChoose()
		{
			if(_selectedDirection == Resources.GetString(Resource.String.TxtUnknown))
				_listener.SelectTeamOnChoose(_selectedGroup);
			else
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
				case Resource.Id.img_unknown:
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