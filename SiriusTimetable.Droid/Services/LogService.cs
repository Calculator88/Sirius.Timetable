using NLog;
using NLog.Config;

namespace SiriusTimetable.Droid.Services
{
	public class LogService
	{

		public LogService()
		{
			
		}

		static LogService()
		{
			LogManager.Configuration = new XmlLoggingConfiguration("assets/nlog.config");
		}
	}
}