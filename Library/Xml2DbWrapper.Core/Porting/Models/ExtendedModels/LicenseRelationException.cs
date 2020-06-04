using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class LicenseRelationException
	{
		public static List<LicenseRelationException> Import(DBBuffer buffer, Paths Paths, string log)
		{
			var fileName = "LicenseRelationExceptions.txt";
			Provider.WriteImportLogFormat(log, fileName);
			var file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			var result = new List<LicenseRelationException>();
			try
			{
				foreach (List<string> riga in file.Skip(1))   // first line contains column names
				{
					var l_LicenseRelation = LicenseRelation.ReadLicenseRelation(buffer, riga);
					var currentLr = buffer.p_LicenseRelation.Single(lr => lr.ParentFeatureId == l_LicenseRelation.ParentFeatureId
									&& lr.FeatureId == l_LicenseRelation.FeatureId
									&& lr.ParentType == l_LicenseRelation.ParentType
									&& lr.HiderType == l_LicenseRelation.HiderType);
					if (currentLr != null)
					{
						var lmodel = buffer.p_logicalModels.Single(lm => lm.Name == riga[4].Trim());
						if (lmodel != null)
						{
							result.Add(new LicenseRelationException
							{
								LicenseRelationId = currentLr.Id,
								LogicalModelId = buffer.p_logicalModels.Single(lm => lm.Name == riga[4].Trim()).Id
							});
						}
						else
						{
							throw new Exception(Provider.WriteErrorLogFormat("Model " + riga[4] + " was not recognized."));
						}
					}
					else
					{
						throw new Exception(Provider.WriteErrorLogFormat("License Relation " + riga[0] + " - " + riga[1] + " - " + riga[2] + " - " + riga[3] + " does not exist"));
					}
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
