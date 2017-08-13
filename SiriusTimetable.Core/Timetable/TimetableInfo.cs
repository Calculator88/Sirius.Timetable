using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SiriusTimetable.Core.Services;
using System.Text.RegularExpressions;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Core.Timetable
{
	public class TimetableInfo
	{
		public TimetableInfo()
		{
			_regx = new Regex(@"\b([a-zA-Zа-я-А-Я]+)(\d+)\b|\S+[\w\s\W]*$");
		}

		/// <summary>
		/// Date of timetable
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Info about cache of timetable
		/// </summary>
		public CacheInfo TimetableCacheInfo { get; set; }

		/// <summary>
		/// Json text which was downloaded from server
		/// </summary>
		public string DownloadedJson { get; set; }

		/// <summary>
		/// Timetable which parsed from DownloadedJson or cache
		/// </summary>
		public ReadOnlyDictionary<String, List<Activity>> Timetable { get; set; }

		/// <summary>
		/// Direction/Number groups existing for this direction
		/// </summary>
		public ReadOnlyDictionary<String, List<int>> DirectionPossibleNumbers { get; set; }

		/// <summary>
		/// Short name/full name collection
		/// </summary>
		public ReadOnlyDictionary<String, String> ShortLongTeamNameDictionary { get; set; }

		public ReadOnlyCollection<String> UnknownPossibleTeams { get; set; }

		public bool Build()
		{
			try
			{
				if (String.IsNullOrEmpty(DownloadedJson))
				{
					if(!TimetableCacheInfo.Exists) return false;

					var rawTimetable = ServiceLocator.GetService<IJsonParser>().ParseJson<Dictionary<String, Timetable>>(TimetableCacheInfo.Content);

					var date = Date.ToString("yyyyMMdd");
					if(!rawTimetable.ContainsKey(date)) return false;

					UpdateData(rawTimetable);
				}
				else
				{
					var rawTimetable = ServiceLocator.GetService<IJsonParser>().ParseJson<Dictionary<String, Timetable>>(DownloadedJson);

					var date = Date.ToString("yyyyMMdd");
					if(!rawTimetable.ContainsKey(date)) return false;

					ServiceLocator.GetService<ICacher>().Cache(TimetableCacheInfo.CacheFileName, DownloadedJson);

					UpdateData(rawTimetable);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}


		private void UpdateData(IReadOnlyDictionary<String, Timetable> rawTimetable)
		{
			var shortLongTeamNameDictionary = new Dictionary<String, String>();
			var timetable = new Dictionary<String, List<Activity>>();

			var directionPossibleNumbers = new Dictionary<String, List<int>>();
			var unknownPossibleTeams = new List<String>();
			
			var date = Date.ToString("yyyyMMdd");
			foreach(var timetableNode in rawTimetable[date].Teams)
			{
				timetableNode.Value.Sort(ComparingActivities);										  //Отсортировать ативности по вермени началу события

				var matches = _regx.Matches(timetableNode.Key);
				if(matches.Count == 0) return;												          //Если ни одного совпадения по шаблону нет,
																									  //то хрен знает, что делать. Расходимся
				if (matches.Count == 1 && String.IsNullOrEmpty(matches[0].Groups[1].Value))
				{                                                                                     
					timetable[timetableNode.Key] = timetableNode.Value;								  				
					unknownPossibleTeams.Add(timetableNode.Key);
					shortLongTeamNameDictionary[timetableNode.Key] = timetableNode.Key;
				}
				else
				{
					var lastMatch = matches[matches.Count - 1];
					if (String.IsNullOrEmpty(lastMatch.Groups[1].Value))
					{
						for (var i = 0; i < matches.Count - 1; ++i)									  //Если совпадений больше одного, то значит, что
						{
																									  //в ключе минимум одна команда, но может быть больше
							var teamName =
								matches[i].Value + ' ' +
								matches[matches.Count - 1].Value;									  //В таком случае нужно найти название для каждой команды
							var direction = matches[i].Groups[1].Value;				                  //и продублировать расписание для нее
							var group = Convert.ToInt32(matches[i].Groups[2].Value);				  //Пример такой ситуации - "Н16, Н17 Big Data"

							timetable[teamName] = timetableNode.Value;
							if (!directionPossibleNumbers.ContainsKey(direction))
								directionPossibleNumbers[direction] = new List<int>();
							directionPossibleNumbers[direction].Add(group);
							shortLongTeamNameDictionary[direction + group.ToString("00")] = teamName;
						}
					}
					else
					{
						for(var i = 0; i < matches.Count; ++i)
						{

							var teamName = matches[i].Value;
							var direction = matches[i].Groups[1].Value;
							var group = Convert.ToInt32(matches[i].Groups[2].Value);

							timetable[teamName] = timetableNode.Value;
							if(!directionPossibleNumbers.ContainsKey(direction))
								directionPossibleNumbers[direction] = new List<int>();
							directionPossibleNumbers[direction].Add(group);
							shortLongTeamNameDictionary[direction + group.ToString("00")] = teamName;
						}
					}
				}
			}

			foreach (var value in directionPossibleNumbers.Values) value.Sort();

			Timetable = new ReadOnlyDictionary<string, List<Activity>>(timetable);
			ShortLongTeamNameDictionary = new ReadOnlyDictionary<String, String>(shortLongTeamNameDictionary);
			DirectionPossibleNumbers = new ReadOnlyDictionary<String, List<int>>(directionPossibleNumbers);
			UnknownPossibleTeams = new ReadOnlyCollection<string>(unknownPossibleTeams);
		}
		private static int ComparingActivities(Activity act1, Activity act2)
		{
			if (act1.Start == act2.Start)
			{
				if(act1.End == act2.End) return 0;
				if(act1.End < act2.End) return -1;
				return 1;
			}
			if(act1.Start < act2.Start) return -1;
			return 1;
		}

		private readonly Regex _regx;
	}
	public enum TimetableDirection
	{
		Art,
		Sport,
		Science,
		Literature,
	}
}