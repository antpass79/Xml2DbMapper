using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public abstract class singleTag : IProbeXmlTags
	{
		public abstract Uirule generateRule(Uirule baseRule, XDocument _doc);

		public String tagName;
		public singleTag(String _tagName)
		{
			tagName = _tagName;
		}

		public virtual Boolean isPresentInDoc(XDocument _doc)
		{
			return (_doc.Descendants(tagName).FirstOrDefault() != null);
		}

		public Uirule handleTagMiss(Uirule baseRule)
		{
			throw new ApplicationException("Error: tag " + tagName + " was not found in a xml file");
		}

	}
}
