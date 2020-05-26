using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class containerTagEnable : IProbeXmlTags
	{
		protected String tagName;
		protected String containedStr;
		public containerTagEnable(String _tagName, String _containedStr)
		{
			tagName = _tagName;
			containedStr = _containedStr;
		}

		public Boolean isPresentInDoc(XDocument _doc)
		{
			return (_doc.Descendants(tagName).FirstOrDefault() != null)
				   && (_doc.Descendants(tagName).First().Descendants().Any(n => n.Name.ToString().Contains(containedStr)));
		}

		public Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = Convert.ToBoolean(_doc.Descendants(tagName).First().Descendants().First(n => n.Name.ToString().Contains(containedStr)).Value)
							 ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}

		public Uirule handleTagMiss(Uirule baseRule)
		{
			throw new ApplicationException("Error: string " + containedStr + " not found under " + tagName + " in a xml file");
		}
	}
}
