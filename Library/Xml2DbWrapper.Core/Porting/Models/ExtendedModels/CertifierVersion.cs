// FG 16112015
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
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
