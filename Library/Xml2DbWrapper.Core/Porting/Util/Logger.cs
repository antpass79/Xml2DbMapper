using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

	public class ConsoleLogger : BaseLogger
	{
		public override void Log(string message)
		{
			Console.WriteLine(message);
		}
	}

	public class FileLogger : BaseLogger, IDisposable
	{
		private readonly StreamWriter _streamWriter;

		public FileLogger(string filePath)
			: this(new StreamWriter(filePath, true))
		{
		}

		public FileLogger(StreamWriter streamWriter)
		{
			_streamWriter = streamWriter;
		}

		public void Dispose()
		{
			_streamWriter.Dispose();
		}

		public override void Log(string message)
		{
			_streamWriter.WriteLine(message);
			_streamWriter.Flush();
		}
	}

	public static class Logger
	{
		static List<BaseLogger> _loggers;

		static Logger()
		{
			_loggers = new List<BaseLogger>();
			_loggers.Add(new TraceLogger());
			_loggers.Add(new ConsoleLogger());
		}

		public static void AddLogger(BaseLogger logger)
		{
			_loggers.Add(logger);
		}

		public static void Error(string message = "")
		{
			Parallel.ForEach(_loggers, logger =>
			{
				logger.Log($"{Environment.NewLine}***************** ERROR *****************{Environment.NewLine} {message}");
			});
		}

		public static void Log(string message = "")
		{
			Parallel.ForEach(_loggers, logger =>
			{
				logger.Log($" >>> {DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture)} - {message}");
			});
		}
	}
}
