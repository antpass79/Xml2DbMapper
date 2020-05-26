using System;
using System.Collections.Generic;
using System.Linq;

// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    public class ParsingException : Exception
	{
		public ParsingException(string FileName, Exception inner, List<string> LineText = null)
		{
			var msg = "Error while reading " + FileName;
			if (LineText != null)
			{
				msg += ", at line  '" + LineText.Aggregate((i, j) => i + " " + j) +  "'";
			}
			msg += ": " + inner.Message;

			var ErrorMessage = Provider.WriteErrorLogFormat(msg);
			throw new Exception(ErrorMessage, inner);
		}
	}
}
