using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xml2DbMapper.Core.Porting.Util;

namespace Xml2DbMapper.Host
{
	class Program
	{
		static void Main(string[] args)
		{
			var OutpuLogPath = Environment.CurrentDirectory;
			try
			{
				// ANTO LOG
				var start = DateTime.Now;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				var deliverer = new Deliverer();
				int BuildNumber = 0;
				if (args.Count() > 0)
				{
					BuildNumber = Convert.ToInt32(args[0]);
				}
				else
				{
					if (!Debugger.IsAttached)
					{
						throw new Exception("Missing Build Number in input");
					}
				}

				// ANTO LOG
				var logPath = Deliverer.GetLogFullName(OutpuLogPath);
				if (File.Exists(logPath))
					File.Delete(logPath);
				using var logger = new FileLogger(logPath);
				Logger.AddLogger(logger);

				var exitCode = deliverer.DeliverAll(OutpuLogPath, BuildNumber);

				// ANTO LOG
				stopwatch.Stop();

				Logger.Log();
				Logger.Log("#########################################################");
				Logger.Log($"Process starts at {start}");
				Logger.Log("#########################################################");
				Logger.Log($"Process stops at {DateTime.Now} with duration of {stopwatch.Elapsed} - Exit Code {exitCode}");

				if (!bool.Parse(System.Configuration.ConfigurationManager.AppSettings["AutoCloseOnFinish"]))
					Console.ReadLine();
				else
					Environment.Exit(exitCode);
			}
			catch (Exception ex)
			{
				// ANTO LOG
				//using (StreamWriter logFileXml2DB = new StreamWriter(Deliverer.GetLogFullName(OutpuLogPath)))
				//{
				//	logFileXml2DB.WriteLine(ex.Message);
				//}
				Logger.Log(ex.Message);
			}
		}
	}
}
