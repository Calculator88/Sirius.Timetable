using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.Util;
using Newtonsoft.Json;
using SiriusTool.Model;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
{
	public class TimetableProvider : ITimetableProvider
	{
	    private readonly ITimetableDownloader _downloader;
	    public TimetableProvider()
	    {
	        _downloader = ServiceLocator.GetService<ITimetableDownloader>();
	    }
	    public void Cancel()
        {
            _downloader.Cancel();
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, List<Event>>> RequestTimetable(DateTime? start, DateTime? end)
        {
            try
            {
                Log.Info("SiriusTool", "Requesting json...");
                var response = await _downloader.GetJsonTimetable(start, end);
                Log.Info("SiriusTool", "Response recieved successfully");
                Log.Info("SiriusTool", "Parsing json...");
                var respString = Encoding.GetEncoding("windows-1251").GetString(response);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, List<Event>>>(respString);
                Log.Info("SiriusTool", "Json parsed successfully");
                return dict;
            }
            catch (WebException ex)
            {
                if (ex.Message == "Aborted.")
                {
                    Log.Warn("SiriusTool", "Requesting json has been canceled");
                    throw;
                }
                Log.Error("SiriusTool", $"An exception occured while loading json: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Log.Error("SiriusTool", $"An error occured while parsing json: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Log.Wtf("SiriusTool", $"An unknown error occured: {ex.Message}");
                throw;
            }
        }
	}
}
