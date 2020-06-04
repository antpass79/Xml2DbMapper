using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
	public partial class SettingFamily
	{
		public static List<SettingFamily> Import(List<Swpack> swpacks, Paths Paths, string log)
		{
			var importObject = "Settings Family";
			Provider.WriteImportLogFormat(log, importObject);
			try
			{
				List<SettingFamily> result = new List<SettingFamily>();
				foreach (var swp in swpacks)
				{
					var equipDescr = Provider.readEquipmentDescription(swp.Name, Paths);
					var validElements = equipDescr.Descendants("sSetting").First().Descendants("elemento")
										.Where(x => x.Element("eFamilyCode").Value.ToString() != "SETTINGS_FAMILY_INVALID_EDS");

					foreach (var setting in validElements)
					{
						result.Add(new SettingFamily
						{
							Name = setting.Element("eFamilyCode").Value.ToString(),
							ProbeListFile = Path.GetFileName(setting.Element("sProbeListFileName").Value.ToString()),
							SwpackId = swp.Id
						});
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new ParsingException(importObject, ex);
			}
		}
	}
}
