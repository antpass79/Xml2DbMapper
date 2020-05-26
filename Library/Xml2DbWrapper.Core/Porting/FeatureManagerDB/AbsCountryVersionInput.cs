using System.Collections.Generic;

// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public abstract class AbsCountryVersionInput: IVersionAssociationInput
	{
		public AbsCountryVersionInput (int _gid, int? _DistributorId, int? _ModelId)
		{
			GeographyId = _gid;
			DistributorId = _DistributorId;
			LogicalModelId = _ModelId;
		}


		private int _GeographyId;
		public int GeographyId
		{
			get
			{
				return _GeographyId;
			}
			set
			{
				_GeographyId = value;
			}
		}

		private int? _DistributorId;
		public int? DistributorId
		{
			get
			{
				return _DistributorId;
			}
			set
			{
				_DistributorId = value;
			}
		}

		private int? _LogicalModelId;
		public int? LogicalModelId
		{
			get
			{
				return _LogicalModelId;
			}
			set
			{
				_LogicalModelId = value;
			}
		}

		public abstract List<IVersionAssociation> GetVersionList(FeatureRegister _register);
		public abstract void AddToRegister(VersionAssociationLists _VersionAssocLists, int MahourVersion);
	}
}
