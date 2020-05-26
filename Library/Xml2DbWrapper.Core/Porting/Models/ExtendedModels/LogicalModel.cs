using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	/// Logical Models
	public partial class LogicalModel
	{
		private String _ModelID = String.Empty;
		public String ModelID
		{
			get
			{
				return _ModelID;
			}
			set
			{
				_ModelID = value;
			}
		}

		private String _SwPack = String.Empty;

		public String SwPack
		{
			get
			{
				return _SwPack;
			}
			set
			{
				_SwPack = value;
			}
		}

		//@PD Added default parameter for program ManualsZipper where Db is not necessary
		public static List<LogicalModel> Import(Dictionary<String, Int32> dictPhysMod, Dictionary<String, Int32> dictLicense,
												DBBuffer buffer, Paths Paths, string log, bool p_Db = true)
		{
			var fileName = "LogicalModel.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<List<String>> file = new List<List<string>>();

			if (p_Db)
			{
				file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
			}
			else
			{
				file = Provider.CreateFile(Paths.RootPath + "\\" + fileName);
			}

			List <LogicalModel> result = new List<LogicalModel>();
			Dictionary<string, SystemEnvironment> dictTipi = new Dictionary<string, SystemEnvironment>
			{
				{ "HUMAN", SystemEnvironment.Human},
				{ "VET", SystemEnvironment.Veterinary},
			};

			Dictionary<string, PhysicalSubModel> PhysModDice = new Dictionary<string, PhysicalSubModel>
			{
				{ "Model_00", PhysicalSubModel.Model_00},
				{ "Model_01", PhysicalSubModel.Model_01},
				{ "Model_02", PhysicalSubModel.Model_02},
				{ "Model_03", PhysicalSubModel.Model_03},
				{ "Model_04", PhysicalSubModel.Model_04}
			};

			try
			{

				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					int? settfamid = null;
					if (p_Db)
					{
						var swpk = buffer.p_swPacks.SingleOrDefault(sw => sw.Name == riga[10].Trim());
						if (swpk != null)
						{
							settfamid = buffer.p_settingsFamilies.Single(sf => sf.Name == riga[3].Trim() && sf.SwpackId == swpk.Id).Id;
						}
					}

					result.Add(new LogicalModel
					{
						Name = riga[0].Trim(),
						ModelFamily = riga[1].Trim(),
						PhModId = Provider.GetValueOrDefault_Stuct(dictPhysMod, riga[2].Trim()),
						SettingsFamilyId = settfamid,
						SubPhysModel = PhysModDice[riga[4].Trim()], // ANTO CAST
						Type = Provider.GetValueOrDefault_Stuct(dictTipi, riga[5].Trim()), // ANTO CAST
						IsDefault = Convert.ToBoolean(riga[6].Trim().ToLowerInvariant()),
						LicenseId = Provider.GetValueOrDefault_Stuct(dictLicense, riga[7].Trim()),
						StyleName = riga[8].Trim(),
						ContexVisionModule = Provider.FromStringToEnum<CVModule>(riga[9].Trim(), CVModule.DSPModule), // ANTO CAST
						ModelID = riga[7].Trim(),
						SwPack = riga[10].Trim()						
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
