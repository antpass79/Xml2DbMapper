using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class Country
	{
		public static List<Country> Import(DBBuffer _buffer, Paths paths, string LogXml2DB)
		{
			List<Country> countries = new List<Country>();
			var fileName = "Countries.txt";
			Provider.WriteImportLogFormat(LogXml2DB, fileName);
			try
			{
				List<List<String>> CountryFile = Provider.CreateFile(paths.importFilePath + "\\" + fileName);
				// import countries
				foreach (List<string> riga in CountryFile.Skip(1))
				{
					countries.Add(new Country
					{
						Code = riga[0],
						Name = riga[1],
						IsObsolete = Convert.ToBoolean(riga[2].Trim().ToLowerInvariant())
					});
				}

				SetCertifier(countries, "CE", paths.importFilePath + "\\CE.txt", _buffer, LogXml2DB);
				SetCertifier(countries, "APA", paths.importFilePath + "\\ANY-PRODUCT-ALLOWED.txt", _buffer, LogXml2DB);
				SetCertifier(countries, "STA", paths.importFilePath + "\\STALLING.txt", _buffer, LogXml2DB);

				return countries;
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex);
			}
		}

		private static void SetCertifier(List<Country> countries, string CertifierCode, string CertifierAssociatonFileName, DBBuffer _buffer, string LogXml2DB)
		{
			Provider.WriteImportLogFormat(LogXml2DB, "Certifier countries for " + CertifierCode);
			try
			{
				List<List<String>> CEFile = Provider.CreateFile(CertifierAssociatonFileName);
				var CECertifierID = _buffer.p_certifiers.Single(ce => ce.Code == CertifierCode).Id;
				foreach (List<string> CErow in CEFile)
				{
					countries.Single(c => c.Code == CErow[0].Trim() && c.Name == CErow[1].Trim()).CertifierId = CECertifierID;
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException("countries for certifier code " + CertifierCode, ex);
			}
		}
	}
}
