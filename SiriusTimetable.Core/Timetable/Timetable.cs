using System.Collections.Generic;

namespace SiriusTimetable.Core.Timetable
{
	/// <summary>
	/// Расписание на один день для всех групп
	/// </summary>
	public class Timetable
	{
		/// <summary>
		/// Словарь расписаний для каждой команды
		/// </summary>
		public Dictionary<string, List<Activity>> Teams { get; set; }
	}
}