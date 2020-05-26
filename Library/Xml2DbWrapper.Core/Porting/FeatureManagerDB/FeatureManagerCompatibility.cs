using System;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    // this value makes sure that the DB imported from zip is compatible with the dll version
    public static class FeatureManagerCompatibility
	{
		public static Int32 Compatibility = 15;
		public static string ConfigurationFileName = "Configuration.xml";
	}
}
