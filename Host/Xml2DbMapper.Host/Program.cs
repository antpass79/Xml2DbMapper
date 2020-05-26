using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Xml2DbMapper.Host
{
	class Program
	{
		static void Main(string[] args)
		{
			var OutpuLogPath = Environment.CurrentDirectory;
			try
			{
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

				Environment.Exit(deliverer.DeliverAll(OutpuLogPath, BuildNumber));
			}
			catch (Exception ex)
			{
				using (StreamWriter logFileXml2DB = new StreamWriter(Deliverer.GetLogFullName(OutpuLogPath)))
				{
					logFileXml2DB.WriteLine(ex.Message);
				}
				Console.WriteLine(ex.Message);
			}
		}
	}
}
