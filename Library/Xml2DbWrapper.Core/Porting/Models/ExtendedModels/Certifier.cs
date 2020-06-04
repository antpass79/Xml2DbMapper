using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	/// Certifying bodies
	public partial class Certifier
	{
		public static List<Certifier> Import(Paths Paths, string log)
		{
			var fileName = "Certifiers.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<Certifier> result = new List<Certifier>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Certifier
					{
						Code = riga[0].Trim(),
						Name = riga[1].Trim()
					});
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex);
			}
		}
	}
}
