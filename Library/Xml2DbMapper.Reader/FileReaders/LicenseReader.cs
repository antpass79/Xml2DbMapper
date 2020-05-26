using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Reader.Models;

namespace Xml2DbMapper.Reader.FileReaders
{
	public partial class LicenseReader
	{
		public static List<License> Import(InputPaths inputPaths, string log)
		{
			var fileName = "Licenses.txt";

			var file = File.ReadAllLines(inputPaths.importFilePath + "\\" + fileName).Select(a => a.Split('\t').ToList()).ToList();

			List<License> result = new List<License>();
			try
			{
				// first line contains column names
				foreach (List<string> line in file.Skip(1))
				{
					result.Add(new License
					{
						Name = line[0].Trim(),
						Code = line[1].Trim(),
						BuyableOnly = Convert.ToBoolean(line[2].Trim().ToLowerInvariant()),
						Unremovable = Convert.ToBoolean(line[3].Trim().ToLowerInvariant())
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
