using System;

namespace Xml2DbMapper.Core.Porting.Util
{
	public abstract class BaseLogger
	{
		protected BaseLogger()
		{
		}
		public abstract void Log(string message);
	}

	public class TraceLogger : BaseLogger
	{
		public override void Log(string message)
		{
			System.Diagnostics.Trace.WriteLine(message);
		}
	}

	public static class Logger
	{
		static BaseLogger _logger;

		static Logger()
		{
			_logger = new TraceLogger();
		}

		static void SetLogger(BaseLogger logger)
		{
			_logger = logger;
		}

		public static void Log(string message)
		{
			_logger.Log(message);
		}
	}
}
