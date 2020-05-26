//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Xml.Linq;
//using Xml2DbMapper.Core.Models;
//using Xml2DbMapper.Reader.Models;

//namespace Xml2DbMapper.Reader.FileReaders
//{
//	public partial class LicenseReader
//	{
//		public static List<SettingFamily> Import(List<Swpack> swpacks, InputPaths inputPaths, string log)
//		{
//			var importObject = "Settings Family";

//			try
//			{
//				List<SettingFamily> result = new List<SettingFamily>();
//				foreach (var swp in swpacks)
//				{
//					var equipDescr = XDocument.Load(inputPaths.GetProbePath(swp.Name) + "\\..\\EquipmentDescr.xml");

//					var validElements = equipDescr
//						.Descendants("sSetting")
//						.First()
//						.Descendants("elemento")
//						.Where(x => x.Element("eFamilyCode").Value.ToString() != "SETTINGS_FAMILY_INVALID_EDS");

//					foreach (var setting in validElements)
//					{
//						result.Add(new SettingFamily
//						{
//							Name = setting.Element("eFamilyCode").Value,
//							ProbeListFile = Path.GetFileName(setting.Element("sProbeListFileName").Value),
//							SwpackId = swp.Id
//						});
//					}
//				}
//				return result;
//			}
//			catch (Exception ex)
//			{
//				throw new ParsingException(importObject, ex);
//			}
//		}
//	}
//}
