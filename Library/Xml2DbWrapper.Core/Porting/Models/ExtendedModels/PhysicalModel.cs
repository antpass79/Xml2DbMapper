using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    /// physical models
    public partial class PhysicalModel
	{
		public static List<PhysicalModel> Import(Paths Paths, string log)
		{
			var fileName = "PhysicalModels.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<PhysicalModel> result = new List<PhysicalModel>();
			List<string> LicenseNames = file[0].ToList();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new PhysicalModel
					{
						Name = Provider.FromStringToEnum<Porting.Contract.Enums.PhysicalModel>(riga[0].Trim(), Porting.Contract.Enums.PhysicalModel.HwModel_None), // ANTO CAST
						Description = riga[1].Trim(),
						Code = riga[2].Trim(),
						SmallConnectorSupport = Convert.ToBoolean(riga[3].Trim().ToLowerInvariant()),
						LargeConnectorSupport = Convert.ToBoolean(riga[4].Trim().ToLowerInvariant())
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
