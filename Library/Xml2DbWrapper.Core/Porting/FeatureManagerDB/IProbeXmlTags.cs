using System;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public interface IProbeXmlTags
	{
		Boolean isPresentInDoc(XDocument _doc);
		Uirule generateRule(Uirule baseRule, XDocument _doc);
		Uirule handleTagMiss(Uirule baseRule);
	}
}
