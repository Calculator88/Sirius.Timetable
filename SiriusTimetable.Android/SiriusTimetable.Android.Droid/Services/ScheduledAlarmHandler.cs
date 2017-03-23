using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using SiriusTimetable.Core.Services;

namespace SiriusTimetable.Droid.Services
{
	[BroadcastReceiver]
	public class ScheduledAlarmHandler : BroadcastReceiver
	{
		public const string LocalNotificationKey = "LocalNotification";

		public override void OnReceive(Context context, Intent intent)
		{
			var extra = intent.GetStringExtra(LocalNotificationKey);
			var notification = SerializeFromString(extra);

			var nativeNotification = CreateNativeNotification(notification);
			nativeNotification.Flags |= NotificationFlags.AutoCancel;
			var manager = GetNotificationManager();

			manager.Notify(notification.Id, nativeNotification);
		}

		private static NotificationManager GetNotificationManager()
		{
			var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
			return notificationManager;
		}

		private static Notification CreateNativeNotification(LocalNotification notification)
		{
			var textStyle = new Notification.BigTextStyle()
				.BigText(notification.BigText)
				.SetSummaryText(notification.Summary);

			var builder = new Notification.Builder(Application.Context)
				.SetContentTitle(notification.Title)
				.SetContentText(notification.BigText)
				.SetDefaults(NotificationDefaults.All)
				.SetSmallIcon(Application.Context.ApplicationInfo.Icon)
				.SetStyle(textStyle);

			var nativeNotification = builder.Build();
			return nativeNotification;
		}

		private static LocalNotification SerializeFromString(string notificationString)
		{
			var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
			using (var stringReader = new StringReader(notificationString))
			{
				var notification = (LocalNotification) xmlSerializer.Deserialize(stringReader);
				return notification;
			}
		}
	}
}