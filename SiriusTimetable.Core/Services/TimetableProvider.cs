using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Core.Services
{
	public class TimetableProvider : ITimetableProvider
	{
		private TaskCompletionSource<Dictionary<String, Timetable.Timetable>> _completion;
		private string _json;
		public async Task<Dictionary<String, Timetable.Timetable>> GetTimetables(DateTime date)
		{
			_completion = new TaskCompletionSource<Dictionary<String, Timetable.Timetable>>();
			var cacheState = ServiceLocator.GetService<ITimetableCacher>().IsStale(date);
			_json = ServiceLocator.GetService<ITimetableCacher>().Get(date);
			//Если кеш существует и актуален
			if (cacheState.HasValue && !cacheState.Value)
			{
				if (!String.IsNullOrEmpty(_json))
				{
					_completion.TrySetResult(ServiceLocator.GetService<ITimetableParser>().ParseTimetables(_json));
					return await _completion.Task;
				}
			}
			//Если кеш существует, но не актуален
			else if (cacheState.HasValue)
			{
				try
				{
					var jsonText = await ServiceLocator.GetService<ITimetableDownloader>().GetJsonString(date);
					ServiceLocator.GetService<ITimetableCacher>().Cache(jsonText, date);
					_completion.TrySetResult(ServiceLocator.GetService<ITimetableParser>().ParseTimetables(jsonText));
					return await _completion.Task;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					if (!String.IsNullOrEmpty(_json))
					{
						ServiceLocator.GetService<IDialogAlertService>().ShowDialog(
							ServiceLocator.GetService<IResourceService>().GetDialogTitleString(),
							ServiceLocator.GetService<IResourceService>().GetDialogCacheIsStaleString(),
							"Ок", "Отмена", "StaleCache");
						return await _completion.Task;
					}
				}
			}

			//Если нет кеша
			try
			{
				var jsonText = await ServiceLocator.GetService<ITimetableDownloader>().GetJsonString(date);
				ServiceLocator.GetService<ITimetableCacher>().Cache(jsonText, date);
				_completion.TrySetResult(ServiceLocator.GetService<ITimetableParser>().ParseTimetables(jsonText));
				return await _completion.Task;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				ServiceLocator.GetService<IDialogAlertService>().ShowDialog(
					ServiceLocator.GetService<IResourceService>().GetDialogTitleString(),
					ServiceLocator.GetService<IResourceService>().GetDialogNoInternetString(),
					"Ок", null, "NoInternet");
				_completion.TrySetResult(null);
				return await _completion.Task;
			}
		}

		public void StaleDialogSetOnPositive()
		{
			_completion.TrySetResult(ServiceLocator.GetService<ITimetableParser>().ParseTimetables(_json));
		}
		public void StaleDialogSetOnNegative()
		{
			_completion.TrySetResult(null);
		}
	}
}