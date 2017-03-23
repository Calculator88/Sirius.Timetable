using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Core.Services
{
	public class TimetableProvider : ITimetableProvider
	{
		public ITimetableDownloader Downloader { get; set; } = ServiceLocator.GetService<ITimetableDownloader>();
		public ITimetableCacher Cacher { get; set; } = ServiceLocator.GetService<ITimetableCacher>();
		public ITimetableParser Parser { get; set; } = ServiceLocator.GetService<ITimetableParser>();
		public IDialogAlertService AlertService { get; set; } = ServiceLocator.GetService<IDialogAlertService>();
		public IResourceService Resources { get; set; } = ServiceLocator.GetService<IResourceService>();

		public async Task<Dictionary<String, Timetable.Timetable>> GetTimetables(DateTime date)
		{
			var cacheState = Cacher.IsStale(date);
			var json = Cacher.Get(date);
			//Если кеш существует и актуален
			if (cacheState.HasValue && !cacheState.Value)
			{
				if (!String.IsNullOrEmpty(json))
					return Parser.ParseTimetables(json);
			}
			//Если кеш существует, но не актуален
			else if (cacheState.HasValue)
			{
				try
				{
					var jsonText = await Downloader.GetJsonString(date);
					Cacher.Cache(jsonText, date);
					return Parser.ParseTimetables(jsonText);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					if (!String.IsNullOrEmpty(json))
					{
						var res = await AlertService.ShowDialog(
							Resources.GetDialogTitleString(),
							Resources.GetDialogCacheIsStaleString(),
							"Ок", "Отмена");
						return res == DialogResult.Positive ? Parser.ParseTimetables(json) : null;
					}
				}
			}

			//Если нет кеша
			try
			{
				var jsonText = await Downloader.GetJsonString(date);
				Cacher.Cache(jsonText, date);
				return Parser.ParseTimetables(jsonText);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				await AlertService.ShowDialog(
					Resources.GetDialogTitleString(),
					Resources.GetDialogNoInternetString(),
					"Ок", null);
				return null;
			}
		}
	}
}