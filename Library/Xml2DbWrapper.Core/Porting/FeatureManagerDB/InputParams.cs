using System;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class InputParams : ComparableFields
	{
		public InputParams() { }

		// constructor
		public InputParams(Int32? l_LogicalModelId,
							Int32? l_ApplicationId,
							Int32? l_OptionId,
							UserLevel? l_UserLevel,
							Int32? l_Version,
							Int32? l_ProbeId,
							ProbeType? l_TransducerType,
							Int32? l_CountryId,
							Int32? l_kitId)
		{
			Initialize(l_LogicalModelId: l_LogicalModelId,
					   l_ApplicationId: l_ApplicationId,
					   l_OptionId: l_OptionId,
					   l_UserLevel: l_UserLevel,
					   l_Version: l_Version,
					   l_ProbeId: l_ProbeId,
					   l_TransducerType: l_TransducerType,
					   l_CountryId: l_CountryId,
					   l_kitId: l_kitId);
		}

		public InputParams(InputParams copy)
		{
			Initialize(copy.LogicalModelId,
					   copy.ApplicationId,
					   copy.OptionId,
					   copy.UserLevel,
					   copy.Version,
					   copy.ProbeId,
					   copy.TransducerType,
					   copy.CountryId,
					   copy.KitId);
		}

		private void Initialize(Int32? l_LogicalModelId,
								Int32? l_ApplicationId,
								Int32? l_OptionId,
								UserLevel? l_UserLevel,
								Int32? l_Version,
								Int32? l_ProbeId,
								ProbeType? l_TransducerType,
								Int32? l_CountryId,
								Int32? l_kitId)
		{
			LogicalModelId = l_LogicalModelId;
			OptionId = l_OptionId;
			ApplicationId = l_ApplicationId;
			UserLevel = l_UserLevel;
			Version = l_Version;
			ProbeId = l_ProbeId;
			TransducerType = l_TransducerType;
			CountryId = l_CountryId;
			KitId = l_kitId;
		}


		private Int32? _LogicalModelId;
		public override Int32? LogicalModelId
		{
			get
			{
				return this._LogicalModelId;
			}
			set
			{
				this._LogicalModelId = value;
			}
		}

		private Int32? _OptionId;
		public override Int32? OptionId
		{
			get
			{
				return this._OptionId;
			}
			set
			{
				this._OptionId = value;
			}
		}

		private Int32? _ApplicationId;
		public override Int32? ApplicationId
		{
			get
			{
				return this._ApplicationId;
			}
			set
			{
				this._ApplicationId = value;
			}
		}


		private UserLevel? _UserLevel;
		public override UserLevel? UserLevel
		{
			get
			{
				return this._UserLevel;
			}
			set
			{
				this._UserLevel = value;
			}
		}


		private Int32? _Version;
		public override Int32? Version
		{
			get
			{
				return this._Version;
			}
			set
			{
				this._Version = value;
			}
		}

		private Int32? _ProbeId;
		public override Int32? ProbeId
		{
			get
			{
				return this._ProbeId;
			}
			set
			{
				this._ProbeId = value;
			}
		}

		private ProbeType? _TransducerType;
		public override ProbeType? TransducerType
		{
			get
			{
				return _TransducerType;
			}
			set
			{
				_TransducerType = value;
			}
		}


		private Int32? _KitId;
		public override Int32? KitId
		{
			get
			{
				return this._KitId;
			}
			set
			{
				this._KitId = value;
			}
		}


		private Int32? _CountryId;
		public override Int32? CountryId
		{
			get
			{
				return this._CountryId;
			}
			set
			{
				this._CountryId = value;
			}
		}

		private Int32? _DistributorId;
		public override Int32? DistributorId
		{
			get
			{
				return this._DistributorId;
			}
			set
			{
				this._DistributorId = value;
			}
		}
	}
}
