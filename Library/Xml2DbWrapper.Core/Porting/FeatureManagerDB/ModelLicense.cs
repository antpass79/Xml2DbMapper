using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class ModelLicense
	{
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public LogicalModel Model;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public License License;
		public ModelLicense(LogicalModel _model, License _license)
		{
			License = _license;
			Model = _model;
		}
	}
}
