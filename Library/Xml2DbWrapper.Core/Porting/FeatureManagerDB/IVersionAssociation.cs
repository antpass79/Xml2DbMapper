// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public interface IVersionAssociation : IVersionAssociationBox
	{
		int MajorVersion
		{
			get;
			set;
		}

	}
}
