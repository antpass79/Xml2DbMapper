using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class FeatureRelation
	{
		public static List<FeatureRelation> Import(Dictionary<String, int> dictFeat, Paths Paths, string log, DBBuffer buffer)
		{
			var fileName = "FeatureRelations.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			var result = new List<FeatureRelation>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					var parentfeatureid = dictFeat[riga[0].Trim()];
					var parentfeature = buffer.p_features.Single(f => f.Id == parentfeatureid);
					if (parentfeature.LicenseId == null)
					{
						throw new ApplicationException(parentfeature.Name + " is not associated with any license.");
					}
					result.Add(new FeatureRelation
					{
						FeatureId = dictFeat[riga[1].Trim()],
						ParentFeatureId = parentfeatureid
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
