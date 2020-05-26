using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Models;

//fg 20012015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	// class linking probe and probeInfo
	public class ProbeBind
	{
		public Probe probe;
		public List<probeInfo> probeInfoLst;

		public ProbeBind() { }
		public ProbeBind(Probe _probe, List<probeInfo> _probeInfoLst)
		{
			probe = _probe;
			probeInfoLst = _probeInfoLst.Select(x => new probeInfo(x)).ToList();
		}

		public probeInfo getProbeInfo(SettingFamily settings)
		{
			return this.probeInfoLst.Single(x => x.settingsFam.Id == settings.Id);
		}
	}
}
