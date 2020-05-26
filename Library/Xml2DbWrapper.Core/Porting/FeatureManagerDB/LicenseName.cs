using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class LicenseName
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Name;
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public License License;
		public LicenseName(string _Name, License _license)
		{
			License = _license;
			Name = _Name;
		}
	}
}
