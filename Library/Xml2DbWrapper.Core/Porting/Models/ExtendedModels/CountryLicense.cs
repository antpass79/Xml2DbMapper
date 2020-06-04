using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class CountryLicense
	{
		public static List<CountryLicense> Import(DBBuffer _buffer, Paths Paths, string log)
		{
			var fileName = "CountryLicenses.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			var result = new List<CountryLicense>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					var country = _buffer.p_countries.Single(c => c.Code == riga[0].Trim());
					int? distributorId = null;
					if (!string.IsNullOrEmpty(riga[1].Trim()))
					{
						var distributor = _buffer.p_Distributors.Single(d => d.Code == riga[1].Trim());
						distributorId = distributor.Id;
						if (!_buffer.p_Country_Distributors.Any(cd => cd.CountryId == country.Id && cd.DistributorId == distributor.Id))
						{
							throw new Exception("Countruy_Distributor table does not contain associatio between " + country.Name + " and " + distributor.Name);
						}
					}
					result.Add(new CountryLicense
					{
						CountryId = country.Id,
						DistributorId = distributorId,
						LicenseId = _buffer.p_licenses.Single(l => l.Name == riga[2].Trim()).Id
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
