using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Core.Timetable;
using Button = Android.Widget.Button;
using View = Android.Views.View;

namespace SiriusTimetable.Droid.Dialogs
{
	public class SelectTeamDialog : AppCompatDialogFragment, View.IOnClickListener, ISelectTeamDialogService
	{
		public const string DTag = "SelectTeamDialog";
		public SelectTeamDialog(Android.Support.V4.App.FragmentManager fragmentManager)
		{
			_manager = fragmentManager;
		}
		private TaskCompletionSource<string> _completion = new TaskCompletionSource<String>();

		public async Task<string> SelectedTeam(TimetableInfo info)
		{
			_info = info;
			Show(_manager, DTag);
			return await _completion.Task;
		}
		private readonly Android.Support.V4.App.FragmentManager _manager;
		private TimetableInfo _info;
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
			var count = Groups.ChildCount;
			for (var i = 0; i < count; ++i)
			{
				var item = Groups.GetChildAt(i);
				item.Alpha = item.Id == id ? 1f : 0.25f;
			}
		}

		private string GetDirectionById(int id)
		{
			var el = Images.FirstOrDefault(view => view.Id == id);
			if (el != null) return (string)el.Tag;
			return null;
		}

		private string GetGroupById(int id)
		{
			var count = Groups.ChildCount;
			for (var i = 0; i < count; ++i)
				if (Groups.GetChildAt(i).Id == id)
					return (string)Groups.GetChildAt(i).Tag;
			return null;
		}
		private string SelectedDirection { get; set; }
		private string SelectedGroup { get; set; }
		private void OnChooseGroup(int id)
		{
			var group = GetGroupById(id);
			if(SelectedGroup == group)
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
			if(SelectedDirection == GetDirectionById(imageId))
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

			var numbers = _info.TeamsLiterPossibleNumbers[SelectedDirection[0].ToString()].Select(GetGroupSelector);
			Groups.RemoveAllViews();
			foreach(var item in numbers) Groups.AddView(item, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));

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

		private void UpdateImageOpacity(int id)
		{
			foreach (var image in Images)
				image.Alpha = image.Id == id ? 1f : 0.25f;
		}

		private void OnChoose()
		{
			_completion.TrySetResult(SelectedDirection[0] + SelectedGroup);
			_completion = new TaskCompletionSource<String>();
			Dismiss();
		}

		private void OnClose()
		{
			_completion.TrySetResult(null);
			_completion = new TaskCompletionSource<String>();
			Dismiss();
		}

		public override void OnDismiss(IDialogInterface dialog)
		{
			_completion.TrySetResult(null);
			_completion = new TaskCompletionSource<String>();
			base.OnDismiss(dialog);
		}

		public override void OnCancel(IDialogInterface dialog)
		{
			_completion.TrySetResult(null);
			_completion = new TaskCompletionSource<String>();
			base.OnCancel(dialog);
		}
	}
}