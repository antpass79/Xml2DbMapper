//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using Xml2DbMapper.Core.Models;
//using Xml2DbMapper.Reader.Models;

//namespace Xml2DbMapper.Reader.FileReaders
//{
//    class LogicalModelReader
//    {
//		//@PD Added default parameter for program ManualsZipper where Db is not necessary
//		public static List<LogicalModel> Import(Dictionary<String, Int32> dictPhysMod, Dictionary<String, Int32> dictLicense,
//												DBBuffer buffer, InputPaths inputPaths, string log, bool p_Db = true)
//		{
//			var fileName = "LogicalModel.txt";

//			List<List<String>> file = new List<List<string>>();

//			if (p_Db)
//			{
//				file = File.ReadAllLines(inputPaths.importFilePath + "\\" + fileName).Select(a => a.Split('\t').ToList()).ToList();
//			}
//			else
//			{
//				file = File.ReadAllLines(inputPaths.RootPath + "\\" + fileName).Select(a => a.Split('\t').ToList()).ToList();
//			}

//			List<LogicalModel> result = new List<LogicalModel>();
//			Dictionary<string, Enums.SystemEnvironment> dictTipi = new Dictionary<string, Enums.SystemEnvironment>
//			{
//				{ "HUMAN", Enums.SystemEnvironment.Human},
//				{ "VET", Enums.SystemEnvironment.Veterinary},
//			};

//			Dictionary<string, Enums.PhysicalSubModel> PhysModDice = new Dictionary<string, Enums.PhysicalSubModel>
//			{
//				{ "Model_00", Enums.PhysicalSubModel.Model_00},
//				{ "Model_01", Enums.PhysicalSubModel.Model_01},
//				{ "Model_02", Enums.PhysicalSubModel.Model_02},
//				{ "Model_03", Enums.PhysicalSubModel.Model_03},
//				{ "Model_04", Enums.PhysicalSubModel.Model_04}
//			};

//			try
//			{

//				// first line contains column names
//				foreach (List<string> riga in file.Skip(1))
//				{
//					int? settfamid = null;
//					if (p_Db)
//					{
//						var swpk = buffer.p_swPacks.SingleOrDefault(sw => sw.Name == riga[10].Trim());
//						if (swpk != null)
//						{
//							settfamid = buffer.p_settingsFamilies.Single(sf => sf.Name == riga[3].Trim() && sf.SWpackId == swpk.Id).Id;
//						}
//					}

//					result.Add(new LogicalModel
//					{
//						Name = riga[0].Trim(),
//						ModelFamily = riga[1].Trim(),
//						PhModId = Provider.GetValueOrDefault_Stuct(dictPhysMod, riga[2].Trim()),
//						SettingsFamilyId = settfamid,
//						SubPhysModel = PhysModDice[riga[4].Trim()],
//						Type = Provider.GetValueOrDefault_Stuct(dictTipi, riga[5].Trim()),
//						IsDefault = Convert.ToBoolean(riga[6].Trim().ToLowerInvariant()),
//						LicenseId = Provider.GetValueOrDefault_Stuct(dictLicense, riga[7].Trim()),
//						StyleName = riga[8].Trim(),
//						ContexVisionModule = Provider.FromStringToEnum<CVModule>(riga[9].Trim(), CVModule.DSPModule),
//						ModelID = riga[7].Trim(),
//						SwPack = riga[10].Trim()
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
