using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class Swpack
	{
		public static List<Swpack> Import(Paths Paths, string log)
		{
			var fileName = "SwPacks.txt";
			Provider.WriteImportLogFormat(log, fileName);
			var file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			var result = new List<Swpack>();
			try
			{
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Swpack
					{
						Name = riga[0].Trim()
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
