// FG 16112015
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
	[DataContract(Namespace = "")]
	public partial class CertifierVersion : IVersionAssociation
	{
		public int GeographyId
		{
			get
			{
				return CertifierId;
			}
			set
			{
				CertifierId = value;
			}
		}
	}
}
