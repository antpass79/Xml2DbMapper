//fg 22022015

using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.Contract.Interfaces;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class TransducerInfo : TransducerInformation
	{
		public TransducerInfo(ProbeTransducers tp)
		{
			Position = (TransducerPosition)tp.TransducerPosition;
			TransducerType = (ProbeType)tp.TransducerType;
		}
		public TransducerPosition Position
		{
			get;
			set;
		}

		public ProbeType TransducerType
		{
			get;
			set;
		}
	}
}
