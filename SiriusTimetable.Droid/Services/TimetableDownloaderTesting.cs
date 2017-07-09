using System;
using System.Net;
using System.Threading.Tasks;
using SiriusTimetable.Core.Services.Abstractions;
using System.Text;

namespace SiriusTimetable.Droid.Services
{
	public class TimetableDownloaderTesting : ITimetableDownloader
	{
		private const string TestUrl = "http://144.76.84.206:13000/timetable/";

		private static string GetUrl(DateTime date)
		{
			return $"{TestUrl}{date:yyyy-MM-dd}";
		}
		public async Task<String> GetJsonString(DateTime date)
		{
			var str = GetUrl(date);
			var client = new WebClient();
			client.Encoding = Encoding.UTF8;
			var result = await client.DownloadStringTaskAsync(new Uri(str));
			return result;
		}
	}
}