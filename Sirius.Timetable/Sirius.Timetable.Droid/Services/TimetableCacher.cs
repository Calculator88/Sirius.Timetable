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
		/// <param name="cachePath">���� � ��� �����</param>
		public TimetableCacher(string cachePath)
		{
			_cacheLocation = cachePath;
		}
		/// <summary>
		/// �����, �� ��������� �������� ��� ��������� ���������
		/// </summary>
		public TimeSpan StalePeriod { get; set; } = TimeSpan.FromHours(4);
		/// <summary>
		/// ���������� ��� json �����
		/// </summary>
		/// <param name="date">����</param>
		/// <returns></returns>
		private string GetFileName(DateTime date)
		{
			return $"{_cacheLocation}/{date:yyyy-MM-dd}.json";
		}
		private readonly string _cacheLocation;

		/// <summary>
		/// ���������� true, ���� ��� �������, null, ���� ���� �� ����������
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public bool? IsStale(DateTime date)
		{
			var fileName = GetFileName(date);
			if (!File.Exists(fileName)) return null;
			return File.GetCreationTimeUtc(fileName) - DateTime.UtcNow >= StalePeriod;
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