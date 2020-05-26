using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public abstract class FocalizationTag : IProbeXmlTags
	{
		public String tagName;
		public String focPath;
		protected XDocument p_focFile = null;

		public FocalizationTag(String _tagName, String _focPath)
		{
			tagName = _tagName;
			focPath = _focPath;
		}

		public Uirule handleTagMiss(Uirule baseRule)
		{
			baseRule.Allow = AllowModes.No;
			return baseRule;
		}


		public Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			openFocFile(_doc);
			return generateRuleCore(baseRule, _doc);
		}

		public abstract Uirule generateRuleCore(Uirule baseRule, XDocument _doc);
		public abstract Boolean isPresentInFoc();

		// checks if the focFileName is present in the doc and then if the tag is present in the foc file
		public Boolean isPresentInDoc(XDocument _presetFile)
		{
			if (_presetFile.Descendants("cFocFileName").FirstOrDefault() != null)
			{
				openFocFile(_presetFile);
				return isPresentInFoc();
			}
			else
			{
				return false;
			}
		}

		protected void openFocFile(XDocument _presetFile)
		{
			if (p_focFile == null)
			{
				p_focFile = XDocument.Load(Provider.GetFocFileName(focPath, _presetFile));
			}
		}


	}
}
