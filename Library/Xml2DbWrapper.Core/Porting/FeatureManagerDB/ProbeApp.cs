using System;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class ProbeApp
	{
		public Int32 ProbeId;
		public Int32 SettingsId;
		public Application app;

		public ProbeApp(Int32 _ProbeId, Int32 _SettingsId, Application _app)
		{
			ProbeId = _ProbeId;
			SettingsId = _SettingsId;
			app = _app;
		}
	}
}
