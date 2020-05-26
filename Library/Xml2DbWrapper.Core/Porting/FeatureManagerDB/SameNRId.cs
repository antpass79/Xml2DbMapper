using System.Collections.Generic;
using Xml2DbMapper.Core.Models;

//fg01122015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class SameNRId : EqualityComparer<NormalRule>
	{
		public override bool Equals(NormalRule x, NormalRule y)
		{
			return x.Id == y.Id;
		}

		public override int GetHashCode(NormalRule obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
