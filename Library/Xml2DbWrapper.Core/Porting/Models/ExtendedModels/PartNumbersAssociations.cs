using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class PartNumbersAssociations
	{

		public String ToString(FeatureRegister register)
		{
			return PartNumber + " " +
				   register.Features.Single(f => f.Id == FeatureId).Name + " " +
				   FeatureBosname + " " +
				   register.ModelLicenses.Single(l => l.Model.Id == LogicalModelId).Model.Name + " " +
				   ToExport.ToString() + " " +
				   ModeTypeToExport;
		}

		public static List<PartNumbersAssociations> Import(Paths Paths, DBBuffer buffer, string log)
		{
			var result = new List<PartNumbersAssociations>();
			var fileName = "PartNumbers.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<string> currentrow = null;
			try
			{
				List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
				foreach (List<string> riga in file.Skip(1))
				{
					currentrow = riga;
					var LogModIndexex = new List < int? >();
					int? FeatureId = null;
					if (!string.IsNullOrEmpty(riga[1]))
					{
						FeatureId = buffer.p_features.Single(f => f.NameInCode == riga[1].Trim()).Id;
					}

					if (!string.IsNullOrEmpty(riga[3]) && !string.IsNullOrEmpty(riga[4].Trim()))
					{
						throw new Exception(riga[3] + " and " + riga[4] + " cannot be specified in the same line, pick only one.");
					}
					else
					{
						if (!string.IsNullOrEmpty(riga[3]))
						{
							LogModIndexex.Add(buffer.p_logicalModels.Single(l => l.Name == riga[3].Trim()).Id);
						}
						else if (!string.IsNullOrEmpty(riga[4]))
						{
							var pmid = buffer.p_physicalModels.Single(m => m.Description == riga[4].Trim()).Id;
							LogModIndexex = buffer.p_logicalModels.Where(l => l.PhModId == pmid).Select(l => (int? )l.Id).ToList();
						}
						else
						{
							LogModIndexex.Add(null);
						}
					}

					foreach (var modelindex in LogModIndexex)
					{
						result.Add(new PartNumbersAssociations
						{
							PartNumber = riga[0].Trim(),
							FeatureId = FeatureId,
							FeatureBosname = riga[2].Trim(),
							LogicalModelId = modelindex,
							ToExport = Convert.ToBoolean(riga[5].Trim().ToLowerInvariant()),
							ModeTypeToExport = riga[6].Trim() == "" ? null : riga[6].Trim()
						});
					}
				}

				var DuplicateAssociations = result.GroupBy(r => new
				{
					pn = r.PartNumber,
					fid = r.FeatureId,
					lmid = r.LogicalModelId,
					toe = r.ToExport
				}).Where(y => y.Count() > 1).ToList();
				if (DuplicateAssociations.Count > 1)
				{
					throw new ApplicationException("Found duplicates in PartNumbers: \r\n" +
												   DuplicateAssociations.Select(x => x.Key.pn
														   +  " " + buffer.p_features.Single(f => f.Id == x.Key.fid).Name
														   + " " + buffer.p_logicalModels.Single(l => l.Id == x.Key.lmid).Name)
												   .Aggregate((i, j) => i + "; \r\n" + j));
				}

				return result;
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex, currentrow);
			}
		}
	}
}
