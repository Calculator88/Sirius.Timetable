using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiriusTool.Model;

namespace SiriusTool.Services.Abstractions
{
    public interface ITimetableProvider
	{
        /// <exception cref="System.Net.WebException"></exception>
        /// <exception cref="Newtonsoft.Json.JsonException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        Task<Dictionary<string, List<Event>>> RequestTimetable(DateTime? start, DateTime? end);

	    void Cancel();
	}
}