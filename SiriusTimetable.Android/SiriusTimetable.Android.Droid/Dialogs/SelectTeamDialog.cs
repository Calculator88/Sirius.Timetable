using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Droid.Dialogs
{
	public class SelectTeamDialog : DialogFragment, View.IOnClickListener, ISelectTeamDialogService
	{
		public const string DTag = "SelectTeamDialog";
		private readonly FragmentManager _manager;
		private TaskCompletionSource<string> _completion;
		private TimetableInfo _info;

		private List<TextView> _numbers;

		public SelectTeamDialog(FragmentManager fragmentManager)
		{
			_manager = fragmentManager;
		}

		private string SelectedDirection { get; set; }
		private string SelectedGroup { get; set; }


		private TextView DirectionName { get; set; }
		private ImageView[] Images { get; } = new ImageView[4];
		private TextView GroupName { get; set; }
		private LinearLayout Groups { get; set; }
		private Button SelectButton { get; set; }

		public void OnClick(View v)
		{
			var id = v.Id;
			switch (id)
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

		public async Task<string> SelectedTeam(TimetableInfo info)
		{
			_info = info;
			_completion = new TaskCompletionSource<string>();
			Show(_manager, DTag);
			return await _completion.Task;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.SetTitle(Resource.String.ChooseTeam);
			var v = inflater.Inflate(Resource.Layout.SelectTeamDialog, null);
			Groups = v.FindViewById<LinearLayout>(Resource.Id.lay_groups);
			Images[0] = v.FindViewById<ImageView>(Resource.Id.img_science);
			Images[1] = v.FindViewById<ImageView>(Resource.Id.img_sport);
			Images[2] = v.FindViewById<ImageView>(Resource.Id.img_art);
			Images[3] = v.FindViewById<ImageView>(Resource.Id.img_literature);
			GroupName = v.FindViewById<TextView>(Resource.Id.group_name);
			DirectionName = v.FindViewById<TextView>(Resource.Id.dir_name);
			SelectButton = v.FindViewById<Button>(Resource.Id.btn_select);
			SelectButton.SetOnClickListener(this);
			v.FindViewById<Button>(Resource.Id.btn_close).SetOnClickListener(this);
			Images[0].SetOnClickListener(this);
			Images[1].SetOnClickListener(this);
			Images[2].SetOnClickListener(this);
			Images[3].SetOnClickListener(this);
			return v;
		}

		private void SetGroupsOpacity(int id)
		{
			if (_numbers == null) return;
			foreach (var number in _numbers)
				number.Alpha = number.Id == id ? 1 : 0.25f;
		}

		private string GetDirectionById(int id)
		{
			var el = Images.FirstOrDefault(view => view.Id == id);
			if (el != null) return (string) el.Tag;
			return null;
		}

		private string GetGroupById(int id)
		{
			return _numbers == null
				? null
				: (from number in _numbers where number.Id == id select (string) number.Tag).FirstOrDefault();
		}

		private void OnChooseGroup(int id)
		{
			var group = GetGroupById(id);
			if (SelectedGroup == group)
				return;

			SetGroupsOpacity(id);
			SelectedGroup = group;

			GroupName.Text = _info.KeywordDictionary[SelectedDirection[0] + SelectedGroup];
			GroupName.Visibility = ViewStates.Visible;
			SelectButton.Enabled = true;
		}

		private void OnChooseDirection(int imageId)
		{
			SelectButton.Enabled = false;
			GroupName.Visibility = ViewStates.Gone;
			if (SelectedDirection == GetDirectionById(imageId))
			{
				UpdateImageOpacity(-1);
				DirectionName.Visibility = ViewStates.Gone;
				Groups.Visibility = ViewStates.Gone;
				SelectButton.Enabled = false;
				SelectedDirection = null;
				SelectedGroup = null;
				return;
			}

			UpdateImageOpacity(imageId);
			SelectedDirection = GetDirectionById(imageId);
			DirectionName.Text = SelectedDirection;

			_numbers = _info.TeamsLiterPossibleNumbers[SelectedDirection[0].ToString()].Select(GetGroupSelector).ToList();
			Groups.RemoveAllViews();
			var lays = new List<LinearLayout>();
			for (var i = 0; i < _numbers.Count; ++i)
			{
				if (((i + 1)%11 == 0) || (i == 0))
				{
					var lay = new LinearLayout(Application.Context) {Orientation = Orientation.Horizontal};
					lay.SetGravity(GravityFlags.Center);
					lays.Add(lay);
				}
				lays[(i + 1)/11].AddView(_numbers[i],
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
			}
			foreach (var layout in lays)
				Groups.AddView(layout,
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
			Groups.Visibility = ViewStates.Visible;

			DirectionName.Visibility = ViewStates.Visible;
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
			foreach (var image in Images)
				image.Alpha = image.Id == id ? 1f : 0.25f;
		}

		private void OnChoose()
		{
			_completion.TrySetResult(SelectedDirection[0] + SelectedGroup);
			Dismiss();
		}

		private void OnClose()
		{
			Dismiss();
		}

		public override void OnDismiss(IDialogInterface dialog)
		{
			_completion.TrySetResult(null);
			base.OnDismiss(dialog);
		}

		public override void OnCancel(IDialogInterface dialog)
		{
			_completion.TrySetResult(null);
			base.OnCancel(dialog);
		}
	}
}