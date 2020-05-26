using System;

// fg15122015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public enum AllowModes
	{
		No,
		Def,
		A
	}

	/// interface of the allower
	public interface IAllower
	{
		Boolean getAllowance(AllowModes Allow);
	}

	/// abstract class of the allowance
	public abstract class Allower : IAllower
	{
		public abstract Boolean getAllowance(AllowModes Allow);
		public static AllowModes getMode(Boolean visible, Boolean enabled)
		{
			if (enabled)
			{
				if (visible)
				{
					return AllowModes.A;
				}
				else
				{
					return AllowModes.Def;
				}
			}
			return AllowModes.No;
		}
	}


	/// manager of the availability
	public class AvailableAllower : Allower
	{
		// ISAVAILABLE
		public override Boolean getAllowance(AllowModes Allow)
		{
			return (Allow != AllowModes.No);
		}
	}

	/// manager of the visibility
	public class VisibleAllower : Allower
	{
		// ISVISIBLE
		public override Boolean getAllowance(AllowModes Allow)
		{
			return (Allow == AllowModes.A);
		}
	}
}
