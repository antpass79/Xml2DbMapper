using System;
using System.IO;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Host
{
	class Deliverer
	{
		private Paths Paths;
		private static string LogName = "Xml2DBLog.txt";
		public static string GetLogFullName(string path) => path + "\\" + LogName;

		public int DeliverAll(string OutputLogPath, int BuildNumber)
		{
			var RootPath = Environment.CurrentDirectory + @"\..\..\..\..\..";
			Paths = new Paths(RootPath);
			var logFileXml2DB = GetLogFullName(OutputLogPath);
			using (StreamWriter _LogFileXml2DB = new StreamWriter(logFileXml2DB)) { }

			try
			{
				FeatureMain.CreateDatabase(Paths, logFileXml2DB, BuildNumber);      // generate the database
				var register = FeatureMain.CreateDBFiles(Paths.DBfile, Paths.DBxmlPath, logFileXml2DB);          // xml files
																												 //GenerateLicenseEnums(register, Paths.RootPath, logFileXml2DB);
				Provider.WriteLogFormat(logFileXml2DB, "Generation Finished.");
				return 0;
			}
			catch (Exception ex)
			{
				Provider.WriteErrorHeader(logFileXml2DB, ex);
				return -1;
			}
		}
	}
}
