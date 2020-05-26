using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Models;

// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class CountryVersionInput : AbsCountryVersionInput
	{
		public CountryVersionInput(int _gid, int ? _DistributorId, int ? _ModelId): base(_gid, _DistributorId, _ModelId) { }

		public override void AddToRegister(VersionAssociationLists _VersionAssocLists, int MajorVersion)
		{
			_VersionAssocLists.CountryVersions.Add(new CountryVersion
			{
				CountryId = this.GeographyId,
				MajorVersion = MajorVersion,
				DistributorId = this.DistributorId,
				LogicalModelId = this.LogicalModelId
			});
		}

		public override List<IVersionAssociation> GetVersionList(FeatureRegister _register)
		{
			return _register.CountryVersions.Select(x => x as IVersionAssociation).ToList();
		}
	}
}
