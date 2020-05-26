using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class Application
	{
		public static List<Application> Import(Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			var fileName = "Applications.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<Application> result = new List<Application>();
			Dictionary<string, SystemEnvironment> dictAppTypes = new Dictionary<string, SystemEnvironment>
			{
				{ "HUMAN", SystemEnvironment.Human},
				{ "VET", SystemEnvironment.Veterinary}
			};

			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Application
					{
						Name = riga[0].Trim(),
						AppType = Provider.GetValueOrDefault_Stuct(dictAppTypes, riga[1].Trim()),
						ProbeDescrName = riga[2].Trim() == "" ? null : riga[2].Trim(),
						Abbreviation = riga[3].Trim(),
						Localization = riga[4].Trim(),
						FeatureId = dictFeat[riga[0].Trim()],
						IsFake = Convert.ToBoolean(riga[5].Trim().ToLowerInvariant())
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
