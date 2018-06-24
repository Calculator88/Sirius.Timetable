using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace SiriusTool.Dialogs
{
	public class DialogAlertService : Android.Support.V7.App.AppCompatDialogFragment
	{
		#region Private fields

		private string _title;
		private string _message;
		private string _positiveButton;
		private string _negativeButton;
		private const string TitleTag = "com.sirius.timetable.dialogAlertService.TITLE";
		private const string MessageTag = "com.sirius.timetable.dialogAlertService.MESSAGE";
		private const string PositiveButtonTag = "com.sirius.timetable.dialogAlertService.POSITIVEBUTTON";
		private const string NegativeButtonTag = "com.sirius.timetable.dialogAlertService.NEGATIVEBUTTON";
		private IDialogAlertResultListener _listener;

		#endregion

		#region Constructors

		public DialogAlertService()
		{

		}

		public DialogAlertService(string title, string message, string positiveButton, string negativeButton)
		{
			_title = title;
			_message = message;
			_positiveButton = positiveButton;
			_negativeButton = negativeButton;
		}

		#endregion

		#region Fragment lifecycle

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_listener = Activity as IDialogAlertResultListener;
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			if(savedInstanceState != null)
			{
				_title = savedInstanceState.GetString(TitleTag);
				_message = savedInstanceState.GetString(MessageTag);
				_positiveButton = savedInstanceState.GetString(PositiveButtonTag);
				_negativeButton = savedInstanceState.GetString(NegativeButtonTag);
			}
			var builder = new Android.Support.V7.App.AlertDialog.Builder(Activity)
				.SetTitle(_title)
				.SetMessage(_message)
				.SetPositiveButton(_positiveButton, (sender, args) => { _listener.OnAlertPositiveButtonClick(Tag); });
			if(!String.IsNullOrEmpty(_negativeButton))
				builder.SetNegativeButton(_negativeButton, (sender, args) => { _listener.OnAlertNegativeButtonClick(Tag); });
			var dialog = builder.Create();
			dialog.SetCanceledOnTouchOutside(true);
			return dialog;
		}
		public override void OnCancel(IDialogInterface dialog)
		{
			base.OnCancel(dialog);
			_listener.OnAlertNegativeButtonClick(Tag);
		}
		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutString(TitleTag, _title);
			outState.PutString(MessageTag, _message);
			outState.PutString(PositiveButtonTag, _positiveButton);
			outState.PutString(NegativeButtonTag, _negativeButton);
		}

		#endregion

		#region Interaction interfaces

		public interface IDialogAlertResultListener
		{
			void OnAlertPositiveButtonClick(string tag);
			void OnAlertNegativeButtonClick(string tag);
		}

		#endregion
	}
}