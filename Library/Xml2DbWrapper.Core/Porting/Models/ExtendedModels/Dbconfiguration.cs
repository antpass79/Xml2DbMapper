using System;

namespace Xml2DbMapper.Core.Models
{
	public partial class Dbconfiguration
	{
		public Dbconfiguration()
		{
			this.Id = -1;
		}

		// constructor
		public Dbconfiguration(Int32 _Compatibility, Int32 _Version)
		{
			Compatibility = _Compatibility;
			Version = _Version;
		}
	}
}
