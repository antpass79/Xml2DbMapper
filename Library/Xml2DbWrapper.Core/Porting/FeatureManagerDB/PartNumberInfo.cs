using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    // [RTC 14796]
    [DataContract(Namespace = "")]
	public class PartNumberInfo
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<PartNumbersAssociations> Associations;

		public static string PartNumberFileName = "PartNumberAssociations.xml";

		public static PartNumberInfo LoadFromFile(string path)
		{
			var FullFileName = path + "\\" + PartNumberFileName;
			return Provider.LoadFromFile<PartNumberInfo>(FullFileName);
		}
	}
}
