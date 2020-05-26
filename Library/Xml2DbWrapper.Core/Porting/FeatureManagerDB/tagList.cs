using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class tagList : IProbeXmlTags
	{
		public List<String> tagLst;
		public tagList(List<String> _tagLst)
		{
			tagLst = _tagLst;
		}

		// if all tags are present in doc then returns true
		public Boolean isPresentInDoc(XDocument _doc)
		{
			return !tagLst.Any(t => _doc.Descendants(t).FirstOrDefault() == null);
		}


		public Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			baseRule.Allow = tagLst.Any(t => Convert.ToBoolean(_doc.Descendants(t).FirstOrDefault().Value)) ? AllowModes.Def : AllowModes.No;
			return baseRule;
		}

		public Uirule handleTagMiss(Uirule baseRule)
		{
			throw new ApplicationException("Error: tag " + this.tagLst.Aggregate((i, j) => i + ", " + j) + " not found in a xml file");
		}

	}
}
