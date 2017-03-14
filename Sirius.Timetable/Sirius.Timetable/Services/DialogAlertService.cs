using System.Threading.Tasks;
using SiriusTimetable.Core.Services.Abstractions;
using Xamarin.Forms;

namespace SiriusTimetable.Common.Services
{
	public class DialogAlertService : IDialogAlertService
	{
		public async Task<DialogResult> ShowDialog(string title, string message, string positiveButton, string negativeButton)
		{
			var ans = await Application.Current.MainPage.DisplayAlert(title, message, positiveButton, negativeButton);
			return ans ? DialogResult.Positive : DialogResult.Negative;
		}

		public async Task ShowDialog(string title, string message, string cancel)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, cancel);
		}
	}
}