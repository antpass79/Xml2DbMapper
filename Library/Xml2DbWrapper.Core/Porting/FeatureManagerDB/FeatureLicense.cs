using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class FeatureLicense
	{
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Feature Feature;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public License License;
		public FeatureLicense(Feature _feature, License _license)
		{
			License = _license;
			Feature = _feature;
		}
	}
}
