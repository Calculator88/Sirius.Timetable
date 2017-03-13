using System;
using System.Collections.Generic;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Common.Services
{
	public static class TimetableService
	{
		/// <summary>
		///     Словарь с расписанием по дате. key - дата в формате ddmmyyyy, value - расписание
		/// </summary>
		public static Dictionary<string, Core.Timetable.Timetable> Timetables { get; set; }
		public static Dictionary<string, string> KeywordDictionary { get; set; }
		public static Dictionary<string, List<string>> TeamsLiterPossibleNumbers { get; set; }
		private static readonly ITimetableProvider Provider = ServiceLocator.GetService<ITimetableProvider>();
		public static async void RefreshTimetables(DateTime date)
		{
			var timetables = await Provider.GetTimetables(date);
			if (timetables == null)
				throw new Exception();

			Timetables = timetables;
			KeywordDictionary = new Dictionary<string, string>();
			TeamsLiterPossibleNumbers = new Dictionary<String, List<String>>();

			foreach (var pair in Timetables[$"{date:ddMMyyyy}"].Teams)
			{
				var shortTeamName = pair.Key.Split()[0];
				KeywordDictionary[shortTeamName] = pair.Key;

				var liter = shortTeamName[0].ToString();
				var number = shortTeamName.Substring(1);
				if (TeamsLiterPossibleNumbers.ContainsKey(liter))
					TeamsLiterPossibleNumbers[liter].Add(number);
				else TeamsLiterPossibleNumbers[liter] = new List<string> { number };
			}
		}
	}
}