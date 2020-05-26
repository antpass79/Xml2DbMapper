using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class thresholdEnableTag : singleTag
	{
		public thresholdEnableTag(String _tagName) : base(_tagName) { }

		public override Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = Convert.ToInt32(_doc.Descendants(tagName).FirstOrDefault().Value) > 0 ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}
	}
}
