using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	/// Deprecated features with substitutes: if the deprecated feature was bought, then it is available, otherwise it is invisible and not buyable any more.
	/// If the substitue feature was bought, the deprecated version is not available any longer.
	public partial class Deprecated
	{
		public static List<Deprecated> Import(Dictionary<String, int> dictFeat, Paths Paths, string log)
		{
			var fileName = "DeprecatedFeatures.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<Deprecated> result = new List<Deprecated>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Deprecated
					{
						DeprecatedFeatureId = dictFeat[riga[0].Trim()],
						SubstituteFeatureId = Provider.GetValueOrDefault_Stuct(dictFeat, riga[1].Trim())
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
