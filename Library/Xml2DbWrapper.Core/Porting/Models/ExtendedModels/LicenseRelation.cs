using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class LicenseRelation
	{
		public static List<LicenseRelation> Import(DBBuffer buffer, Paths Paths, string log)
		{
			var fileName = "LicenseRelations.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<LicenseRelation> result = new List<LicenseRelation>();
			var LicensedFeatures = buffer.p_features.Join(buffer.p_licenses, f => f.LicenseId, l => l.Id, (f, l) => f).ToList();

			try
			{
				foreach (List<string> riga in file.Skip(1))   // first line contains column names
				{
					result.Add(ReadLicenseRelation(buffer, riga));
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex);
			}
		}

		public static Dictionary<string, ParentTypes> dictParenTypes = new Dictionary<string, ParentTypes>
		{
			{ "MACRO", ParentTypes.Macro},
			{ "MASTER", ParentTypes.Master},
		};

		public static Dictionary<string, HiderTypes> dictHiderTypes = new Dictionary<string, HiderTypes>
		{
			{ "NoHide", HiderTypes.NoHide},
			{ "Hider", HiderTypes.Hider},
			{ "Collapser", HiderTypes.Collapser},
		};

		public static LicenseRelation ReadLicenseRelation(DBBuffer buffer, List<string> riga)
		{
			var ParentFeature = buffer.p_features.SingleOrDefault(fl => fl.Name == riga[0].Trim());
			var Feature = buffer.p_features.SingleOrDefault(fl => fl.Name == riga[1].Trim());
			if (ParentFeature != null && Feature != null && ParentFeature.LicenseId != null && Feature.LicenseId != null)
			{
				return new LicenseRelation
				{
					FeatureId = Feature.Id,
					ParentFeatureId = ParentFeature.Id,
					ParentType = dictParenTypes[riga[2].Trim()],
					HiderType = dictHiderTypes[riga[3].Trim()]
				};
			}
			else
			{
				throw new Exception(Provider.WriteErrorLogFormat("Check License Relation " + riga[0] + " - " + riga[1] + ": all features should be under license."));
			}
		}

	}
}
