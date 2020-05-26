//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using Xml2DbMapper.Core.Models;
//using Xml2DbMapper.Reader.Models;

//namespace Xml2DbMapper.Reader.FileReaders
//{
//    class TwinLicensesReader
//    {
//		public static List<TwinLicenses> Import(Dictionary<String, int> dictLicenses, InputPaths inputPaths, string log)
//		{
//			var fileName = "Twins.txt";

//			var file = File.ReadAllLines(inputPaths.importFilePath + "\\" + fileName).Select(a => a.Split('\t').ToList()).ToList();

//			List<TwinLicenses> result = new List<TwinLicenses>();
//			try
//			{
//				// first line contains column names
//				foreach (List<string> riga in file.Skip(1))
//				{
//					result.Add(new TwinLicenses
//					{
//						LicenseId = dictLicenses[riga[0].Trim()],
//						TwinLicenseId = dictLicenses[riga[1].Trim()]
//					});
//				}

//				return result;
//			}
//			catch (Exception ex)
//			{
//				throw new ParsingException(fileName, ex);
//			}
//		}
//	}
//}
