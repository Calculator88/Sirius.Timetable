using System.Threading.Tasks;

namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface IDialogAlertService
	{
		Task<DialogResult> ShowDialog(string title, string message, string positiveButton, string negativeButton);
	}

	public enum DialogResult
	{
		Positive,
		Negative
	}
}