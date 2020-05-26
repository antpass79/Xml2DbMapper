using System;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class FeatureInfo
	{
		public FeatureInfo(Boolean _avail, Boolean _vis)
		{
			IsAvailable = _avail;
			IsVisible = _vis;
		}
		public Boolean IsAvailable;
		public Boolean IsVisible;
	}
}
