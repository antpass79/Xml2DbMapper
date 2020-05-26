using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class valueEnableTag : singleTag
	{
		private String tagValue;
		public valueEnableTag(String _tagName, String _tagValue) : base(_tagName)
		{
			tagValue = _tagValue;
		}

		public override Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = _doc.Descendants(tagName).FirstOrDefault().Value == tagValue ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}
	}
}
