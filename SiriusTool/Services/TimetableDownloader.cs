using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Android.Util;
using SiriusTool.Services.Abstractions;

namespace SiriusTool.Services
{
	public class TimetableDownloader : ITimetableDownloader
	{
		private const string ApiUrl = "https://schedule-export.talantiuspeh.ru/meeting/json_app";
		private const string Key = "d2fc437c3f951b20fc7a6efa517a62f6";
	    private readonly WebClient _client;

	    public TimetableDownloader()
	    {
	        _client = new WebClient();
	    }

		public Task<byte[]> GetJsonTimetable(DateTime? start = null, DateTime? end = null)
		{
		    if (_client.IsBusy)
		    {
		        Log.Warn("SiriusTool", "Another web operation is in process. Canceling current operation...");
		        _client.CancelAsync();
		    }
            var pars = new NameValueCollection
            {
                { "key", Key }
            };

		    if (end != null && start != null)
		    {
		        Log.Info("SiriusTool", $"Request params: start:{start:yyyy-MM-dd}, end:{end:yyyy-MM-dd}");
		        pars.Add("start", $"{start:yyyy-MM-dd}");
                pars.Add("end", $"{end:yyyy-MM-dd}");
		    }
            else if (start != null)
		    {
		        Log.Info("SiriusTool", $"Request params: start:{start:yyyy-MM-dd}");
                pars.Add("start", $"{start:yyyy-MM-dd}");
		    }
		    else
		    {
		        Log.Info("SiriusTool", "Json requested with no parameters");
		    }
		    var resp = _client.UploadValuesTaskAsync(ApiUrl, pars);
		    return resp;
		}

	    public void Cancel()
	    {
	        _client.CancelAsync();
	    }
	}
}