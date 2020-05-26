//fg 22022015

using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.Contract.Interfaces;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    class LicensePositionFilter: LicenseFilterBase, ILicensePositionFilter, IInternalLicensePositionFilter
	{
		public LicensePositionFilter() : base() { }
		//public LicensePositionFilter(LicenseFilterBase Copy) : base(Copy) { }

		public TransducerPosition? TransducerPos
		{
			get;
			set;
		}

		public ILicenseFilter ToILicenseFilter(ProbeType Transducer)
		{
			var basef = (LicenseFilterBase)this;
			var Lfilter = new LicenseFilter(basef);
			Lfilter.TransducerType = Transducer;
			return Lfilter;
		}
	}
}
