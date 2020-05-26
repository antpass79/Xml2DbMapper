// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public interface IVersionAssociationBox
	{
		int GeographyId
		{
			get;
			set;
		}

		int? DistributorId
		{
			get;
			set;
		}

		int? LogicalModelId
		{
			get;
			set;
		}
	}
}
