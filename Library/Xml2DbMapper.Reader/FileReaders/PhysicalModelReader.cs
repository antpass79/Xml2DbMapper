//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Xml2DbMapper.Core.Models;
//using Xml2DbMapper.Core.Porting.Contract.Enums;
//using Xml2DbMapper.Reader.Extensions;

//namespace Xml2DbMapper.Reader.FileReaders
//{
//	public partial class PhysicalModelReader
//	{
//		public static List<PhysicalModel> Import(InputPaths inputPaths, string log)
//		{
//			var fileName = "PhysicalModels.txt";

//			var file = File.ReadAllLines(inputPaths.importFilePath + "\\" + fileName).Select(a => a.Split('\t').ToList()).ToList();

//			List<Xml2DbMapper PhysicalModel> result = new List<PhysicalModel>();
//			List<string> LicenseNames = file[0].ToList();
//			try
//			{
//				// first line contains column names
//				foreach (List<string> riga in file.Skip(1))
//				{
//					result.Add(new PhysicalModel
//					{
//						Name = riga[0].Trim().FromStringToEnum<Enums.PhysicalModel>(Enums.PhysicalModel.HwModel_None),
//						Description = riga[1].Trim(),
//						Code = riga[2].Trim(),
//						SmallConnectorSupport = Convert.ToBoolean(riga[3].Trim().ToLowerInvariant()),
//						LargeConnectorSupport = Convert.ToBoolean(riga[4].Trim().ToLowerInvariant())
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
