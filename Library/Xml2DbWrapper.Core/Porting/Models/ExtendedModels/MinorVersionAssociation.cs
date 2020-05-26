using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class MinorVersionAssociation
	{
		public static List<MinorVersionAssociation> Import(Paths Paths, string log)
		{
			List<MinorVersionAssociation> result = new List<MinorVersionAssociation>();
			var fileName = "MINOR-VERSION.txt";
			Provider.WriteImportLogFormat(log, fileName);
			try
			{
				List<List<String>> CountryFile = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
				foreach (List<string> riga in CountryFile.Skip(1))
				{
					string buildV = string.IsNullOrEmpty(riga[3]) ? null : riga[3].Trim();
					result.Add(new MinorVersionAssociation
					{
						Major = Convert.ToInt32(riga[0].Trim()),
						Minor = Convert.ToInt32(riga[1].Trim()),
						Patch = Convert.ToInt32(riga[2].Trim()),
						BuildVersion = buildV
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
