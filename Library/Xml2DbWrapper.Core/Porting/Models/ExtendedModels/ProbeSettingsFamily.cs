using System;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    public partial class ProbeSettingsFamily
	{
		public ProbeSettingsFamily()
		{
		}

		// constructor from correstpondent probeInfo class
		public ProbeSettingsFamily(Int32 probeId, probeInfo probeInfo)
		{
			ProbeId = probeId;
			SettingsFamilyId = probeInfo.settingsFam.Id;
			ProbeFolder = probeInfo.ProbeFolder;
			ProbeDescFileName = probeInfo.ProbeDescFileName;
		}

		public String getProbeDataFile(TransducerPosition type)
		{
			if (type == TransducerPosition.Frontal)
			{
				return this.ProbeDataFileNameFrontal;
			}
			else
			{
				return this.ProbeDataFileNameLateral;
			}
		}
	}
}
