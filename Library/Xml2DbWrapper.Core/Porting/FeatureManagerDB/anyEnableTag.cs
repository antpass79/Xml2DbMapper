using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class anyEnableTag : singleTag
	{
		public anyEnableTag(String _tagName) : base(_tagName) { }

		public override Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = _doc.Descendants(tagName).Any(x => Convert.ToBoolean(x.Value)) ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}

	}
}
