using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	/// Option partial class
	public partial class Option
	{
		public static List<Option> Import(Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			var fileName = "Options.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			List<Option> result = new List<Option>();

			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Option
					{
						Name = riga[0].Trim(),
						FeatureId = dictFeat[riga[0].Trim()],
						IsFake = Convert.ToBoolean(riga[1].Trim().ToLowerInvariant()),
						IsPreset = Convert.ToBoolean(riga[2].Trim().ToLowerInvariant())
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
