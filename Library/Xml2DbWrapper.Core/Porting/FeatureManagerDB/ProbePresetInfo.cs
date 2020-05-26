using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class ProbePresetInfo
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<ProbePreset> Items;

		public static string ProbePresetFileName = "ProbePresets.xml";

		public static ProbePresetInfo LoadFromFile(string path)
		{
			var FullFileName = path + "\\" + ProbePresetFileName;
			return Provider.LoadFromFile<ProbePresetInfo>(FullFileName);
		}
	}
}
