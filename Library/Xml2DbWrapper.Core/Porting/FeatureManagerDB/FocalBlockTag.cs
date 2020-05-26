using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class FocalBlockTag : FocalizationTag
	{
		private String tagValue;
		public FocalBlockTag(String _tagName, String _focPath, String _tagValue) : base(_tagName, _focPath)
		{
			tagValue = _tagValue;
		}

		public override Boolean isPresentInFoc()
		{
			if (p_focFile.Descendants("Focal_Block").Attributes(tagName).FirstOrDefault() != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override Uirule generateRuleCore(Uirule baseRule, XDocument _doc)
		{
			if (p_focFile.Descendants("Focal_Block").Attributes(tagName).Any(x => x.Value == tagValue))
			{
				baseRule.Allow = AllowModes.Def;
			}
			else
			{
				baseRule.Allow = AllowModes.No;
			}
			return baseRule;
		}
	}
}
