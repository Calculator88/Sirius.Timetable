using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        public async Task<Dictionary<string, List<Event>>> RequestTimetable(DateTime? start, DateTime? end)
        {
            try
            {
                var response = await _downloader.GetJsonTimetable(start, end);
                var respString = Encoding.GetEncoding("windows-1251").GetString(response);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, List<Event>>>(respString);
                return dict;
            }
            catch (TaskCanceledException)
            {
                Log.Info("SiriusTool", "Json requesting has been canceled");
                throw;
            }
            catch (WebException ex)
            {
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
