using System;
using System.Linq;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class userEnableTag : singleTag
	{
		private UserLevel p_user;
		public userEnableTag(String _tagName, UserLevel _user) : base(_tagName)
		{
			p_user = _user;
		}

		public override Boolean isPresentInDoc(XDocument _doc)
		{
			var headTag = _doc.Descendants(tagName).FirstOrDefault();
			Boolean IsUserTagPresent = false;
			if (headTag != null)
			{
				switch (p_user)
				{
					case UserLevel.RS:
						IsUserTagPresent = headTag.Descendants("fHwKeyRSDept").FirstOrDefault() != null;
						break;
					case UserLevel.WIP:
						IsUserTagPresent = headTag.Descendants("fHwKeyWIP").FirstOrDefault() != null;
						break;
					case UserLevel.Service:
						IsUserTagPresent = headTag.Descendants("fHwKeyService").FirstOrDefault() != null;
						break;
					case UserLevel.Test:
						IsUserTagPresent = headTag.Descendants("fHwKeyTesting").FirstOrDefault() != null;
						break;
					case UserLevel.Standard:
						IsUserTagPresent = headTag.Descendants("fHwKeyProduction").FirstOrDefault() != null;
						break;
					default:
						throw new ApplicationException("Unhandled user level " + p_user.ToString());
				}
			}

			return base.isPresentInDoc(_doc) && IsUserTagPresent;
		}


		public override Uirule generateRule(Uirule baseRule, XDocument _doc)
		{
			var headTag = _doc.Descendants(tagName).FirstOrDefault();

			switch (p_user)
			{
				case UserLevel.RS:
					return addUserToRule(baseRule, UserLevel.RS, headTag, "fHwKeyRSDept");
				case UserLevel.WIP:
					return addUserToRule(baseRule, UserLevel.WIP, headTag, "fHwKeyWIP");
				case UserLevel.Service:
					return addUserToRule(baseRule, UserLevel.Service, headTag, "fHwKeyService");
				case UserLevel.Test:
					return addUserToRule(baseRule, UserLevel.Test, headTag, "fHwKeyTesting");
				case UserLevel.Standard:
					return addUserToRule(baseRule, UserLevel.Standard, headTag, "fHwKeyProduction");
				default:
					throw new ApplicationException("Unhandled user level " + p_user.ToString());
			}
		}

		private Uirule addUserToRule(Uirule _rl, UserLevel _user, XElement _headTag, String _tagName)
		{
			Uirule rlRS = new Uirule(_rl);
			rlRS.UserLevel = _user;
			rlRS.Allow = (Convert.ToBoolean(_headTag.Descendants(_tagName).FirstOrDefault().Value) ? AllowModes.Def : AllowModes.No);
			return rlRS;
		}
	}
}
