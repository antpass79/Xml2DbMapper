using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class FocalModeTag : FocalizationTag
	{
		private String modeName;
		public FocalModeTag(String _tagName, String _focPath, String _modeName) : base(_tagName, _focPath)
		{
			modeName = _modeName;
		}

		public override Boolean isPresentInFoc()
		{
			if (p_focFile.Descendants("Mode").Any(n => n.Attributes("cModeId").FirstOrDefault().Value == modeName))
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
			if (p_focFile.Descendants("Mode").SingleOrDefault(n => n.Attributes("cModeId").FirstOrDefault().Value == modeName).Descendants(tagName).Count() > 0)
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
