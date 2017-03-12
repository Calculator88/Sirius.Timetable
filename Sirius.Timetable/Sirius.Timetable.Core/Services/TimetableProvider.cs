﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public Dictionary<String, Timetable.Timetable> GetTimetables(DateTime date)
		{
			var cacheState = Cacher.IsStale(date);
			var json = Cacher.Get(date);
			if (cacheState.HasValue && !cacheState.Value)
			{
				if (!String.IsNullOrEmpty(json))
					return Parser.ParseTimetables(json);
			}
			else if (cacheState.HasValue)
			{
				try
				{
					var jsonText = Downloader.GetJsonString(date);
					Cacher.Cache(jsonText, date);
					return Parser.ParseTimetables(jsonText);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					if (!String.IsNullOrEmpty(json))
					{
						var res = AlertService
							.ShowDialog(Resources.GetDialogTitleString(), Resources.GetDialogCacheIsStaleString(), "Ок", "Отмена").Result;
						return res == DialogResult.Positive ? Parser.ParseTimetables(json) : null;
					}
				}
			}

			try
			{
				var jsonText = Downloader.GetJsonString(date);
				if (String.IsNullOrEmpty(jsonText)) throw new Exception("JsonTest is empty");
				Cacher.Cache(jsonText, date);
				return Parser.ParseTimetables(jsonText);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				var dialogResult = AlertService.ShowDialog(Resources.GetDialogTitleString(), Resources.GetDialogNoInternetString(), "Ок", null).Result;
				return null;
			}
		}
	}
}