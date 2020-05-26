using System;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Reader.Extensions
{
	static class StringExtensions
    {
		public static T FromStringToEnum<T>(this string @string, T value) where T : struct
		{
			if (typeof(T) == typeof(ApplicationType))
			{
				throw new Exception("Use FromStringToAppType() for Application Type, do not use FromStringToEnum(): default value of ApplicationType is not NoType!");
			}
			T result;
			if (Enum.TryParse(@string, out result))
			{
				return result;
			}
			else
			{
				return value;
			}
		}

	}
}
