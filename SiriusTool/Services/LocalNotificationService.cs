using System;
using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
{
	public class LocalNotificationService : ILocalNotificationService
	{
		/// <summary>
		///     Notifies the specified notification.
		/// </summary>
		/// <param name="notification">The notification.</param>
		public void Notify(LocalNotification notification)
		{
			var intent = CreateIntent();

			var serializedNotification = SerializeNotification(notification);
			intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

			var pendingIntent = PendingIntent.GetBroadcast(Application.Context, notification.Id, intent,
				PendingIntentFlags.CancelCurrent);
			var triggerTime = NotifyTimeInMilliseconds(notification.NotifyTime);
			var alarmManager = GetAlarmManager();

			alarmManager.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + triggerTime, pendingIntent);
		}

		/// <summary>
		///     Cancels the specified notification identifier.
		/// </summary>
		/// <param name="notificationId">The notification identifier.</param>
		public void Cancel(int notificationId)
		{
			var intent = CreateIntent();
			var pendingIntent = PendingIntent.GetBroadcast(Application.Context, notificationId, intent,
				PendingIntentFlags.UpdateCurrent);

			var alarmManager = GetAlarmManager();
			alarmManager.Cancel(pendingIntent);

			var notificationManager = GetNotificationManager();
			notificationManager.Cancel(notificationId);
		}

		private static Intent CreateIntent()
		{
			return new Intent(Application.Context, typeof(ScheduledAlarmHandler));
		}

		private static NotificationManager GetNotificationManager()
		{
			var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
			return notificationManager;
		}

		private static AlarmManager GetAlarmManager()
		{
			var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
			return alarmManager;
		}

		private static string SerializeNotification(LocalNotification notification)
		{
			var xmlSerializer = new XmlSerializer(notification.GetType());
			using (var stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, notification);
				return stringWriter.ToString();
			}
		}

		private static long NotifyTimeInMilliseconds(DateTime notifyTime)
		{
			var utcAlarmTimeInMillis = (notifyTime.ToUniversalTime() - DateTime.UtcNow).TotalMilliseconds;
			return (long) utcAlarmTimeInMillis;
		}
	}
}