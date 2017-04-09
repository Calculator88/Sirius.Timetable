namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface IDialogAlertService
	{
		void ShowDialog(string title, string message, string positiveButton, string negativeButton, string tag);
	}
}