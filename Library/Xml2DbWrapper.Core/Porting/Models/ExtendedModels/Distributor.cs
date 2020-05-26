using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    public partial class Distributor
	{
		public static List<Distributor> Import(Paths Paths, string log)
		{
			var fileName = "Distributors.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			var result = new List<Distributor>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Distributor
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
