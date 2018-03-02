using System;

namespace SiriusTool.Services
{
	public class LocalNotification
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string BigText { get; set; }
		public string Summary { get; set; }
		public DateTime NotifyTime { get; set; }
	}
}