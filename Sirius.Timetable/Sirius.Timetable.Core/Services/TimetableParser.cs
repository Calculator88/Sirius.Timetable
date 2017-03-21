using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Core.Services
{
	public class TimetableParser : ITimetableParser
	{
		public Dictionary<string, Timetable.Timetable> ParseTimetables(string jsonString)
		{
			return String.IsNullOrEmpty(jsonString)
				? null
				: JsonConvert.DeserializeObject<Dictionary<string, Timetable.Timetable>>(jsonString);
		}
	}
}