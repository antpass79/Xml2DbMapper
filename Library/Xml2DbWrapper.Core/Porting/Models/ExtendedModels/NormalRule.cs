using System;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

//fg01122015

namespace Xml2DbMapper.Core.Models
{
    public partial class NormalRule : ComparableFields, ICloneable
	{
		public NormalRule()
		{
		}

		// copy constructor
		public NormalRule(NormalRule copy)
		{
			Allow = copy.Allow;
			LogicalModelId = copy.LogicalModelId;
			ApplicationId = copy.ApplicationId;
			OptionId = copy.OptionId;
			UserLevel = copy.UserLevel;
			Version = copy.Version;
			CountryId = copy.CountryId;
			DistributorId = copy.DistributorId;
			ProbeId = copy.ProbeId;
			TransducerType = copy.TransducerType;
			KitId = copy.KitId;
			UiruleId = copy.UiruleId;
		}

		public object Clone()
		{
			var clone = new NormalRule(this);
			clone.Id = this.Id;
			return clone;
		}
	}
}
