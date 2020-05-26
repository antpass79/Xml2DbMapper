using System;
using System.Linq;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;

//fg01122015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public abstract class FeaturesFields
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public abstract Int32? OptionId
		{
			get;
			set;
		}
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public abstract Int32? ApplicationId
		{
			get;
			set;
		}
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public abstract Int32? ProbeId
		{
			get;
			set;
		}
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public abstract Int32? KitId
		{
			get;
			set;
		}

		public Feature GetFeature(DBBuffer buffer, out FeatureRegister.FeatureType type)
		{
			type = FeatureRegister.FeatureType.Nothing;

			if (OptionId != null)
			{
				type = FeatureRegister.FeatureType.Option;
				return buffer.p_features.Single(f => f.Id == (buffer.p_option.Single(o => o.Id == OptionId).FeatureId));
			}

			if (ApplicationId != null)
			{
				type = FeatureRegister.FeatureType.Application;
				return buffer.p_features.Single(f => f.Id == (buffer.p_applications.Single(o => o.Id == ApplicationId).FeatureId));
			}

			if (ProbeId != null)
			{
				type = FeatureRegister.FeatureType.Probe;
				return buffer.p_features.Single(f => f.Id == (buffer.p_probes.Single(o => o.Id == ProbeId).FeatureId));
			}

			if (KitId != null)
			{
				type = FeatureRegister.FeatureType.Kit;
				return buffer.p_features.Single(f => f.Id == (buffer.p_biospyKits.Single(o => o.Id == KitId).FeatureId));
			}

			return null;
		}

		public void SetFieldFromType(FeatureRegister.FeatureType type, FeaturesFields fields)
		{
			switch (type)
			{
				case FeatureRegister.FeatureType.Application:
					ApplicationId = fields.ApplicationId;
					break;
				case FeatureRegister.FeatureType.Option:
					OptionId = fields.OptionId;
					break;
				case FeatureRegister.FeatureType.Probe:
					ProbeId = fields.ProbeId;
					break;
				case FeatureRegister.FeatureType.Kit:
					KitId = fields.KitId;
					break;
				default:
					break;
			}
		}

	}
}
