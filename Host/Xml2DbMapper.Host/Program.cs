using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xml2DbMapper.Core.Porting.Util;
using Xml2DbMapper.Host.Tests;

namespace Xml2DbMapper.Host
{
	class Program
	{
		static void Main(string[] args)
		{
			var OutpuLogPath = Environment.CurrentDirectory;

			// ANTO LOG
			var logPath = Deliverer.GetLogFullName(OutpuLogPath);
			if (File.Exists(logPath))
				File.Delete(logPath);
			using var logger = new FileLogger(logPath);
			Logger.AddLogger(logger);

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

				var exitCode = deliverer.DeliverAll(OutpuLogPath, BuildNumber);

				if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["RunTestsAfterXmlGeneration"]))
				{
					exitCode = RunTests();
				}

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

		private static int RunTests()
		{
			IList<string> differentFiles = new List<string>();

			string originalXmlDirectory = Environment.CurrentDirectory + System.Configuration.ConfigurationManager.AppSettings["OriginalXmlDirectory"];
			string generatedXmlDirectory = Environment.CurrentDirectory + System.Configuration.ConfigurationManager.AppSettings["GeneratedXmlDirectory"];

			IEnumerable<string> originalFiles = Directory.EnumerateFiles(originalXmlDirectory, "*.xml").Where(file => !file.ToLower().EndsWith("probepresets.xml")).OrderBy(file => file);
			IEnumerable<string> generatedFiles = Directory.EnumerateFiles(generatedXmlDirectory, "*.xml").Where(file => !file.ToLower().EndsWith("probepresets.xml")).OrderBy(file => file);

			if (originalFiles.Count() != generatedFiles.Count())
			{
				differentFiles.Add("Difference in number");
				return -100;
			}

			int count = originalFiles.Count();
			for (int i = 0; i < count; i++)
			{
				Logger.Log($"Test on {originalFiles.ElementAt(i)}");

				if (!FileComparer.CompareFileHashes(originalFiles.ElementAt(i), generatedFiles.ElementAt(i)))
				{
					Logger.Log($"Difference in {originalFiles.ElementAt(i)}");
					differentFiles.Add(originalFiles.ElementAt(i));
				}
			}

			if (differentFiles.Count() > 0)
			{
				return -100;
			}

			return 0;
		}
	}
}
