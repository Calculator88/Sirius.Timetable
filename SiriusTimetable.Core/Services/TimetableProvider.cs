using System;
using System.Threading.Tasks;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Core.Timetable;
using System.IO;

namespace SiriusTimetable.Core.Services
{
	public class TimetableProvider : ITimetableProvider
	{
		public async Task<TimetableInfo> GetTimetableInfo(DateTime date)
		{
			var info = new TimetableInfo { Date = date };

			try
			{
				info.DownloadedJson = await ServiceLocator.GetService<ITimetableDownloader>().GetJsonString(date);
			}
			catch(Exception)
			{
				info.DownloadedJson = null;
			}

			var fileNameTimetable = ServiceLocator.GetService<ICacher>().CacheDirectory + $"{date:ddMMyyyy}.json";
			info.TimetableCacheInfo = new CacheInfo
			{
				Exists = ServiceLocator.GetService<ICacher>().Exists(fileNameTimetable),
				CacheFileName = fileNameTimetable,
				Content = ServiceLocator.GetService<ICacher>().Get(fileNameTimetable),
				CreationTime = ServiceLocator.GetService<ICacher>().GetCreationTime(fileNameTimetable)
			};

			return info;
		}
	}
}