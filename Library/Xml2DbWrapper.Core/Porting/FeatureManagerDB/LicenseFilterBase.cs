using Xml2DbMapper.Core.Porting.Contract.Interfaces;
using Xml2DbMapper.Core.Porting.Contract.Enums;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    class LicenseFilterBase : ILicenseFilterBase, IInternalLicenseFilterBase
	{
		public LicenseFilterBase() { }

		public LicenseFilterBase(LicenseFilterBase Copy)
		{
			OptionName = Copy.OptionName;
			ApplicationType = Copy.ApplicationType;
			ProbeName = Copy.ProbeName;
			CountryName = Copy.CountryName;
			KitName = Copy.KitName;
			UserLevel = Copy.UserLevel;
			LogicalModelName = Copy.LogicalModelName;
			Version = Copy.Version;
		}
		public string OptionName
		{
			get;
			set;
		}
		public ApplicationType? ApplicationType
		{
			get;
			set;
		}
		public string ProbeName
		{
			get;
			set;
		}
		public string CountryName
		{
			get;
			set;
		}
		public string KitName
		{
			get;
			set;
		}
		public UserLevel? UserLevel
		{
			get;
			set;
		}
		public string LogicalModelName
		{
			get;
			set;
		}
		public string Version
		{
			get;
			set;
		}
	}
}
