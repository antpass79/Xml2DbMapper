using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.Contract.Interfaces;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    class LicenseFilter : LicenseFilterBase, ILicenseFilter, IInternalLicenseFilter
	{
		public LicenseFilter() : base() { }
		public LicenseFilter(LicenseFilterBase Copy) : base(Copy) { }

		public ProbeType? TransducerType
		{
			get;
			set;
		}
	}
}
