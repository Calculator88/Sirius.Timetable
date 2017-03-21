using System;
using System.Threading.Tasks;
using Android.Content;
using SiriusTimetable.Core.Services.Abstractions;
using AlertDialog = Android.App.AlertDialog;

namespace SiriusTimetable.Droid.Services
{
	public class DialogAlertService : IDialogAlertService
	{
		public DialogAlertService(Context context)
		{
			_context = context;
		}
		private TaskCompletionSource<DialogResult> _completion;
		private readonly Context _context;
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