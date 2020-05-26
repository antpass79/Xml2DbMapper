using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

//fg 20012015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    // class representing the information associated to the probe per settings family
    public class probeInfo
	{
		public SettingFamily settingsFam;
		public sSUPPORTED_KEY_PTI SupportedKeys;
		public String ProbeFolder;
		public String ProbeDescFileName;
		public List<XElement> appNodes;

		public string wHwCode;
		public string ProbeStringCode;

		public bool IsVNAVEnabled       // Gibiino [RTC 14690]
		{
			get;
			set;
		}

		public probeInfo(probeInfo pi)
		{
			settingsFam = pi.settingsFam;
			SupportedKeys = pi.SupportedKeys;
			ProbeFolder = pi.ProbeFolder;
			ProbeDescFileName = pi.ProbeDescFileName;
			appNodes = pi.appNodes;
			IsVNAVEnabled = pi.IsVNAVEnabled;
			wHwCode = pi.wHwCode;
			ProbeStringCode = pi.ProbeStringCode;
		}

		public probeInfo(XElement elemento, SettingFamily settigs)
		{
			settingsFam = settigs;
			SupportedKeys = new sSUPPORTED_KEY_PTI(elemento.Element("sSupportedKey"));

			IsVNAVEnabled = elemento.Element("wNavigatorProbeCode").Value != "0";
			wHwCode = elemento.Element("wHwCode").Value;
			var prbstrcode = elemento.Element("sProbeStringCode").Value.ToString();
			ProbeStringCode = prbstrcode == "NULL" ? null : prbstrcode;

			String[] probeDescSplit = elemento.Element("sProbeDescFileName").Value.ToString().Split('\\');
			ProbeFolder = probeDescSplit[0].ToString();
			ProbeDescFileName = probeDescSplit[1].ToString();
		}

		public probeInfo(SettingFamily _settingsFam, ProbeBind probeBind)
		{
			settingsFam = _settingsFam;
			var currentProbeInfo = probeBind.getProbeInfo(_settingsFam);

			SupportedKeys = new sSUPPORTED_KEY_PTI(currentProbeInfo.SupportedKeys);
			ProbeFolder = currentProbeInfo.ProbeFolder;
			ProbeDescFileName = currentProbeInfo.ProbeDescFileName;
			IsVNAVEnabled = currentProbeInfo.IsVNAVEnabled;
			wHwCode = currentProbeInfo.wHwCode;
			ProbeStringCode = currentProbeInfo.ProbeStringCode;
		}
	}
}
