using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V7.App;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Dialogs
{
	public class DialogAlertService : IDialogAlertService
	{
		private readonly Context _context;
		private TaskCompletionSource<DialogResult> _completion;

		public DialogAlertService(Context context)
		{
			_context = context;
		}

		public async Task<DialogResult> ShowDialog(string title, string message, string positiveButton, string negativeButton)
		{
			_completion = new TaskCompletionSource<DialogResult>();
			BuildDialog(title, message, positiveButton, negativeButton).Show();
			return await _completion.Task;
		}

		private AlertDialog BuildDialog(string title, string message, string positiveButton, string negativeButton)
		{
			var builder = new AlertDialog.Builder(_context)
				.SetTitle(title)
				.SetMessage(message)
				.SetPositiveButton(positiveButton, PositiveButtonOnClick);
			if (!String.IsNullOrEmpty(negativeButton))
				builder.SetNegativeButton(negativeButton, NegativeButtonOnClick);
			var dialog = builder.Create();
			dialog.CancelEvent += (sender, args) => { _completion.TrySetResult(DialogResult.Negative); };
			dialog.DismissEvent += (sender, args) => { _completion.TrySetResult(DialogResult.Negative); };
			return dialog;
		}

		private void NegativeButtonOnClick(Object sender, DialogClickEventArgs dialogClickEventArgs)
		{
			_completion.TrySetResult(DialogResult.Negative);
		}

		private void PositiveButtonOnClick(Object sender, DialogClickEventArgs dialogClickEventArgs)
		{
			_completion.TrySetResult(DialogResult.Positive);
		}
	}
}