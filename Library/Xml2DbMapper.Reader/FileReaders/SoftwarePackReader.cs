//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Xml2DbMapper.Core.Models;
//using Xml2DbMapper.Reader.Models;

//namespace Xml2DbMapper.Reader.FileReaders
//{
//	class SoftwarePackReader
//    {
//		const string FILE_NAME = "SwPacks.txt";

//		public SoftwarePackReader()
//		{
//		}

//		public List<Swpack> Read(InputPaths inputPaths)
//		{
//			var fileName = "SwPacks.txt";
//			var file = File
//				.ReadAllLines(Path.Combine(inputPaths.importFilePath, fileName))
//				.Select(a => a.Split('\t').ToList());

//			var result = new List<Swpack>();
//			try
//			{
//				// first line contains column names => skip
//				foreach (List<string> line in file.Skip(1))
//				{
//					result.Add(new Swpack
//					{
//						Name = line[0].Trim()
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
