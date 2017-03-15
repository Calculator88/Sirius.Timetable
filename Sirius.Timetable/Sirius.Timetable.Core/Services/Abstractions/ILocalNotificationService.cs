namespace SiriusTimetable.Core.Services.Abstractions
{
	public interface ILocalNotificationService
	{
		void Notify(LocalNotification notification);

		void Cancel(int notificationId);
	}
}