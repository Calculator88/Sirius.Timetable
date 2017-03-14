using System;
using System.IO;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Droid.Services
{
	public class TimetableCacher : ITimetableCacher
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cachePath">Путь к кеш папке</param>
		public TimetableCacher(string cachePath)
		{
			_cacheLocation = cachePath;
		}
		/// <summary>
		/// Время, по истечении которого кеш считается устарешим
		/// </summary>
		public TimeSpan StalePeriod { get; set; } = TimeSpan.FromHours(0.01);
		/// <summary>
		/// Возвращает имя json файла
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns></returns>
		private string GetFileName(DateTime date)
		{
			return $"{_cacheLocation}/{date:yyyy-MM-dd}.json";
		}
		private readonly string _cacheLocation;

		/// <summary>
		/// Возвращает true, если кеш устарел, null, если файл не сущетсвует
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public bool? IsStale(DateTime date)
		{
			var fileName = GetFileName(date);
			if (!File.Exists(fileName) || String.IsNullOrEmpty(File.ReadAllText(fileName))) return null;
			var period = DateTime.UtcNow - File.GetCreationTimeUtc(fileName);
			return period >= StalePeriod;
		}


		public string Get(DateTime dateToGet)
		{
			var fileName = GetFileName(dateToGet);
			return File.Exists(fileName) ? File.ReadAllText(fileName) : null;
		}

		public void Cache(string timetableJsonText, DateTime dateToCache)
		{
			var fileName = GetFileName(dateToCache);
			File.WriteAllText(fileName, timetableJsonText);
		}
	}
}