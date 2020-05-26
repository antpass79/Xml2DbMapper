using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    /// Features partial class: generalizes options and applications
    public partial class Feature
	{
		public static List<Feature> Import(Dictionary<string, int> dictLicenses, Paths Paths, string log)
		{
			var fileName = "Features.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<Feature> result = new List<Feature>();

			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					bool? alp = null;
					if (!string.IsNullOrEmpty(riga[3]))
					{
						alp = Convert.ToBoolean(riga[3].Trim().ToLowerInvariant());
					}
					result.Add(new Feature
					{
						Name = riga[0].Trim(),
						NameInCode = riga[1].Trim(),
						LicenseId = Provider.GetValueOrDefault_Stuct(dictLicenses, riga[2].Trim()),
						AlwaysPresent = alp
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
