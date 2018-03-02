using System;
using System.Threading.Tasks;

namespace SiriusTool.Services.Abstractions
{
	public interface ITimetableDownloader
	{
		Task<byte[]> GetJsonTimetable(DateTime? start, DateTime? end);

        void Cancel();
	}
}