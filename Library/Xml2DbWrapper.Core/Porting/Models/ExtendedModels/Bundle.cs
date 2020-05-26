using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    /// bundle table partial definition
    public partial class Bundle
	{
		public static List<Bundle> Import(Dictionary<String, int> dictFeat, Paths Paths, string log)
		{
			var fileName = "Bundle.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<Bundle> result = new List<Bundle>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Bundle
					{
						FeatureId = dictFeat[riga[1].Trim()],
						ParentFeatureId = dictFeat[riga[0].Trim()]
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
