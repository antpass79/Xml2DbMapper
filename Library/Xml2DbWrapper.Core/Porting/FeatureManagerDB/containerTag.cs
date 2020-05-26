using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class containerTag : IProbeXmlTags
	{
		protected String tagName;
		protected String containedStr;
		public containerTag(String _tagName, String _containedStr)
		{
			tagName = _tagName;
			containedStr = _containedStr;
		}

		public Boolean isPresentInDoc(XDocument _doc)
		{
			return (_doc.Descendants(tagName).FirstOrDefault() != null) && (_doc.Descendants(tagName).First().Descendants().Any(n => n.Value.Contains(containedStr)));
		}

		public Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = isPresentInDoc(_doc) ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}

		public Uirule handleTagMiss(Uirule baseRule)
		{
			throw new ApplicationException("Error: string " + containedStr + " not found under " + tagName + " in a xml file");
		}
	}
}
