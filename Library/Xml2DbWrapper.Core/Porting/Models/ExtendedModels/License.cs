using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class License
	{
		public static List<License> Import(Paths Paths, string log)
		{
			var fileName = "Licenses.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<License> result = new List<License>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new License
					{
						Name = riga[0].Trim(),
						Code = riga[1].Trim(),
						BuyableOnly = Convert.ToBoolean(riga[2].Trim().ToLowerInvariant()),
						Unremovable = Convert.ToBoolean(riga[3].Trim().ToLowerInvariant())
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
