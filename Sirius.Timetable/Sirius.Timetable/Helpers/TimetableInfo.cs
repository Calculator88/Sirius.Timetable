using System;
using System.Collections.Generic;
using System.Diagnostics;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;
using SiriusTimetable.Core.Timetable;

namespace SiriusTimetable.Common.Helpers
{
	public class TimetableInfo
	{
		public TimetableInfo(Dictionary<string, Timetable> timetable, DateTime date)
		{
			Date = date;
			Timetable = timetable;
		}

		private void UpdateInfo()
		{
			KeywordDictionary = new Dictionary<string, string>();
			TeamsLiterPossibleNumbers = new Dictionary<String, List<String>>();

			foreach (var pair in Timetable[$"{Date:ddMMyyyy}"].Teams)
			{
				var shortTeamName = pair.Key.Split()[0];
				KeywordDictionary[shortTeamName] = pair.Key;

				var liter = shortTeamName[0].ToString();
				var number = shortTeamName.Substring(1);
				if (TeamsLiterPossibleNumbers.ContainsKey(liter))
					TeamsLiterPossibleNumbers[liter].Add(number);
				else TeamsLiterPossibleNumbers[liter] = new List<string> {number};
			}
		}

		private Dictionary<string, Timetable> _timetable;
		public Dictionary<String, List<String>> TeamsLiterPossibleNumbers { get; private set; }
		public Dictionary<string, Timetable> Timetable
		{
			get { return _timetable; }
			set
			{
				if (value == null) return;
				_timetable = value;
				UpdateInfo();
			}
		}
		public Dictionary<string, string> KeywordDictionary { get; private set; }
		public DateTime Date { get; }
	}
}