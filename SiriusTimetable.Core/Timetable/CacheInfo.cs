using System;

namespace SiriusTimetable.Core.Timetable
{
	/// <summary>
	/// Provides information about cache
	/// </summary>
	public class CacheInfo
	{
		/// <summary>
		/// Returns name of file associated with cache
		/// </summary>
		public string CacheFileName { get; set; }

		/// <summary>
		/// Content of cache
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Creation date of cache
		/// </summary>
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Whether cache exists or not
		/// </summary>
		public bool Exists { get; set; }
	}
}