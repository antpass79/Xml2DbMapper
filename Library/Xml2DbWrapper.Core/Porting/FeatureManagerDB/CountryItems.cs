using System.Collections.Generic;
using System.Runtime.Serialization;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class CountryItems
	{
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<CountryItem> CountryItemList = new List<CountryItem>();
	}
}
