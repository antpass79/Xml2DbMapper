using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    public partial class CountryDistributor
	{
		public static List<CountryDistributor> Import(DBBuffer _buffer, Paths Paths, string log)
		{
			var fileName = "Country_Distributor.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			var result = new List<CountryDistributor>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new CountryDistributor
					{
						CountryId = _buffer.p_countries.Single(c => c.Code == riga[0].Trim()).Id,
						DistributorId = _buffer.p_Distributors.Single(c => c.Code == riga[1].Trim()).Id
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
