﻿using System;
using System.IO;
using Xml2DbMapper.Core;
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
			// ANTO CONFIG
			//var RootPath = Environment.CurrentDirectory + @"\..\..\..\..\..\..\..";
			var RootPath = Environment.CurrentDirectory + System.Configuration.ConfigurationManager.AppSettings["RelativePathToCurrentDirectory"];
			Paths = new Paths(RootPath);
			var logFileXml2DB = GetLogFullName(OutputLogPath);

			// ANTO LOG
			//using (StreamWriter _LogFileXml2DB = new StreamWriter(logFileXml2DB)) { }

			try
			{
				DatabaseType databaseType = System.Configuration.ConfigurationManager.AppSettings["DatabaseType"] switch
				{
					"SQLite" => DatabaseType.SQLite,
					"SQLiteInMemory" => DatabaseType.SQLiteInMemory,
					"SQLServer" => DatabaseType.SQLServer,
					_ => DatabaseType.SQLite
				};

				FeatureMain.CreateDatabase(
					Paths,
					logFileXml2DB,
					BuildNumber,
					databaseType,
					System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);      // generate the database
			
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
