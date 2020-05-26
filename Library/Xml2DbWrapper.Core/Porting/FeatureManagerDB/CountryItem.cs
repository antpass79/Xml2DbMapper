using System;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class CountryItem
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string MagicNumber;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public String ComboLabel;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public string LicenseCode;

		public CountryItem(Country _country, Distributor _distributor, License _license)
		{
			MagicNumber = _license.Code;
			ComboLabel = FeatureRegister.GetCountryComboDisplay(_country, _distributor);
			LicenseCode = _license.Name;
		}
	}
}
