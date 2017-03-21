using System;
using System.Net;
using System.Threading.Tasks;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class TimetableDownloader : ITimetableDownloader
	{
		private const string JsonUrl = "http://project2.sochisirius.ru/schedule/getforday/";
		private const string Key = "/ddd3a42ebd1ebce7f821d7d2fb04cac8";

		private static string GetFileUrl(DateTime date)
		{
			return JsonUrl + date.ToString("yyyy-MM-dd") + Key;
		}

		public async Task<string> GetJsonString(DateTime date)
		{
			var str = GetFileUrl(date);
			var result = await new WebClient().DownloadStringTaskAsync(new Uri(str));
			return result;
		}
	}
}