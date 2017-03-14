using System;
using System.Threading.Tasks;
using SiriusTimetable.Common.Helpers;
using SiriusTimetable.Core.Services;
using SiriusTimetable.Core.Services.Abstractions;

namespace SiriusTimetable.Common.Models
{
	public class TimetableModel
	{
		public TimetableModel(ITimetableProvider provider = null)
		{
			_provider = provider ?? ServiceLocator.GetService<ITimetableProvider>();
		}

		public async Task<TimetableInfo> GetTimetableInfo(DateTime date)
		{
			var dict = await _provider.GetTimetables(date);
			if (dict == null) return null;
			var info = new TimetableInfo(dict, date);
			return info;
		}
		private readonly ITimetableProvider _provider;
	}
}