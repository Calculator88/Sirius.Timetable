using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DialogAlertService : DialogFragment
	{
		#region Private fields

		private string _title;
		private string _message;
		private string _positiveButton;
		private string _negativeButton;
		private const string TitleTag = "TITLE";
		private const string MessageTag = "MESSAGE";
		private const string PositiveButtonTag = "POSITIVEBUTTON";
		private const string NegativeButtonTag = "NEGATIVEBUTTON";
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
			if (_listener == null) throw new Exception($"{Activity} must implement {typeof(IDialogAlertResultListener)}");
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
			var builder = new AlertDialog.Builder(Activity)
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