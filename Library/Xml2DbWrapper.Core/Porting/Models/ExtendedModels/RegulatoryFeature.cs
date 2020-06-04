using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	[DataContract(Namespace = "")]
	public partial class RegulatoryFeature
	{
		public static List<RegulatoryFeature> Import(DBBuffer _buffer, Paths paths, string LogXml2DB)
		{
			var fileName = "REGULATORY-FEATURES.txt";
			Provider.WriteImportLogFormat(LogXml2DB, fileName);
			List<List<String>> file = Provider.CreateFile(paths.importFilePath + "\\" + fileName);
			var result = new List<RegulatoryFeature>();
			int RowIndex = -2, ColumnIndex = -2;
			try
			{
				var FeatureNames = file[0];
				for (RowIndex = 1; RowIndex < file.Count; RowIndex++)
				{
					for (ColumnIndex = 0; ColumnIndex < FeatureNames.Count; ColumnIndex++)
					{
						if (!string.IsNullOrEmpty(file[RowIndex][ColumnIndex]))
						{
							var Feature = _buffer.GetFeatureFromName(file[RowIndex][ColumnIndex]);
							result.Add(new RegulatoryFeature
							{
								Name = FeatureNames[ColumnIndex],
								FeatureId = Feature.Id
							});
						}
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName + " at row " + (RowIndex + 1).ToString() + " column " +
										   (ColumnIndex + 1).ToString(), ex);
			}
		}
	}
}
