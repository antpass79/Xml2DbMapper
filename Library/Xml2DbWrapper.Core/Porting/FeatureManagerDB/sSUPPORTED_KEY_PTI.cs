using System;
using System.Xml.Linq;

//fg 20012015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public partial class sSUPPORTED_KEY_PTI
	{
		private bool fHwKeyRSDeptField;
		private bool fHwKeyWIPField;
		private bool fHwKeyServiceField;
		private bool fHwKeyTestingField;
		private bool fHwKeyProductionField;

		public bool fHwKeyRSDept
		{
			get
			{
				return this.fHwKeyRSDeptField;
			}
			set
			{
				this.fHwKeyRSDeptField = value;
			}
		}

		public bool fHwKeyWIP
		{
			get
			{
				return this.fHwKeyWIPField;
			}
			set
			{
				this.fHwKeyWIPField = value;
			}
		}

		public bool fHwKeyService
		{
			get
			{
				return this.fHwKeyServiceField;
			}
			set
			{
				this.fHwKeyServiceField = value;
			}
		}

		public bool fHwKeyTesting
		{
			get
			{
				return this.fHwKeyTestingField;
			}
			set
			{
				this.fHwKeyTestingField = value;
			}
		}

		public bool fHwKeyProduction
		{
			get
			{
				return this.fHwKeyProductionField;
			}
			set
			{
				this.fHwKeyProductionField = value;
			}
		}

		public sSUPPORTED_KEY_PTI(XElement sk)
		{
			this.fHwKeyRSDept = Convert.ToBoolean(sk.Element("fHwKeyRSDept").Value);
			this.fHwKeyWIP = Convert.ToBoolean(sk.Element("fHwKeyWIP").Value);
			this.fHwKeyService = Convert.ToBoolean(sk.Element("fHwKeyService").Value);
			this.fHwKeyTesting = Convert.ToBoolean(sk.Element("fHwKeyTesting").Value);
			this.fHwKeyProduction = Convert.ToBoolean(sk.Element("fHwKeyProduction").Value);
		}

		public sSUPPORTED_KEY_PTI(sSUPPORTED_KEY_PTI sk)
		{
			this.fHwKeyRSDept = sk.fHwKeyRSDept;
			this.fHwKeyWIP = sk.fHwKeyWIP;
			this.fHwKeyService = sk.fHwKeyService;
			this.fHwKeyTesting = sk.fHwKeyTesting;
			this.fHwKeyProduction = sk.fHwKeyProduction;
		}

	}
}
