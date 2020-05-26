using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class directEnableTag : singleTag
	{
		public directEnableTag(String _tagName) : base(_tagName) { }

		public override Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = Convert.ToBoolean(_doc.Descendants(tagName).FirstOrDefault().Value) ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}

	}
}
