using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	/// twin licenses
	public partial class TwinLicenses
	{
		public TwinLicenses()
		{
			this.Id = -1;
		}

		public TwinLicenses(Int32 _LicenseId, Int32 _TwinLicenseId)
		{
			LicenseId = _LicenseId;
			TwinLicenseId = _TwinLicenseId;
		}

		public static List<TwinLicenses> Import(Dictionary<String, int> dictLicenses, Paths Paths, string log)
		{
			var fileName = "Twins.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<TwinLicenses> result = new List<TwinLicenses>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new TwinLicenses(dictLicenses[riga[0].Trim()], dictLicenses[riga[1].Trim()]));
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
