//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public partial class FeatureMain
	{
        public enum DeliveryResult
        {
            Ok,
            InvalidCompatibility,
            InvalidVersion,
            InvalidFiles,
            InvalidFormat,
            UnzipUnsuccessful
        }
	}
}
