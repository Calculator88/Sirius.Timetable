using System;

namespace SiriusTimetable.Core.Timetable
{
	/// <summary>
	///     Событие дня для одной группы
	/// </summary>
	public class Activity
	{
		/// <summary>
		///     Время начала события
		/// </summary>
		public DateTime? Start { get; set; }

		/// <summary>
		///     Время конца события
		/// </summary>
		public DateTime? End { get; set; }

		/// <summary>
		///     Название и описание события
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///     Место события
		/// </summary>
		public string Place { get; set; }

		/// <summary>
		///     Время отъезда автобуса на мероприятие, null - если нет автобуса
		/// </summary>
		public DateTime? BusTo { get; set; }

		/// <summary>
		///     Время отъезда автобуса с мероприятия, null - если нет автобуса
		/// </summary>
		public DateTime? BusFrom { get; set; }
	}
}