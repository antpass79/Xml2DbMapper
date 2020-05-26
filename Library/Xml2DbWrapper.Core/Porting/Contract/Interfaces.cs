using System;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Porting.Contract.Interfaces
{
	/// <summary>
	///  gibiino 22022015: Interface to query the Feature database
	/// </summary>
	public interface ILicenseFilter : ILicenseFilterBase
	{
		ProbeType? TransducerType
		{
			set;
		}
	}

	// Gibiino
	public interface ILicensePositionFilter : ILicenseFilterBase
	{
		TransducerPosition? TransducerPos
		{
			set;
		}

		ILicenseFilter ToILicenseFilter(ProbeType Transducer);
	}


	public interface ILicenseFilterBase
	{
		String OptionName
		{
			set;
		}
		ApplicationType? ApplicationType
		{
			set;
		}
		String ProbeName
		{
			set;
		}
		String KitName
		{
			set;
		}
	}

	public interface IInternalLicenseFilterBase
	{
		String OptionName
		{
			get;
		}
		ApplicationType? ApplicationType
		{
			get;
		}
		String ProbeName
		{
			get;
		}
		String KitName
		{
			get;
		}
		String CountryName
		{
			get;
			set;
		}
		Enums.UserLevel? UserLevel
		{
			get;
			set;
		}
		String LogicalModelName
		{
			get;
			set;
		}
	}

	public interface IInternalLicenseFilter : IInternalLicenseFilterBase
	{
		ProbeType? TransducerType
		{
			get;
		}
	}

	public interface IInternalLicensePositionFilter : IInternalLicenseFilterBase
	{
		Enums.TransducerPosition? TransducerPos
		{
			get;
		}
	}

	public interface TransducerInformation
	{
		TransducerPosition Position
		{
			get;
			set;
		}
		ProbeType TransducerType
		{
			get;
			set;
		}
	}
}