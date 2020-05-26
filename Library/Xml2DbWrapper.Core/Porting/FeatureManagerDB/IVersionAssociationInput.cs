using System.Collections.Generic;

// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public interface IVersionAssociationInput : IVersionAssociationBox
	{
		List<IVersionAssociation> GetVersionList(FeatureRegister _register);
		void AddToRegister(VersionAssociationLists _VersionAssocLists, int MajorVersion);
	}
}