using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Reader.FileReaders
{
    [Serializable]
    internal class ParsingException : Exception
    {
        public ParsingException()
        {
        }

        public ParsingException(string fileName, Exception inner, List<string> LineText = null)
        {
            var msg = "Error while reading " + fileName;
            if (LineText != null)
            {
                msg += ", at line  '" + LineText.Aggregate((i, j) => i + " " + j) + "'";
            }
            msg += ": " + inner.Message;

            //var ErrorMessage = Provider.WriteErrorLogFormat(msg);
            //throw new Exception(ErrorMessage, inner);
        }

        public ParsingException(string message) : base(message)
        {
        }

        public ParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}