using System;
using System.Runtime.Serialization;

namespace Edge
{

    [Serializable]
    public class EdgeParserException : Exception
    {

        public EdgeParserException() { }

        public EdgeParserException(string message) : base(message) { }

        public EdgeParserException(string message, Exception inner) : base(message, inner) { }

        protected EdgeParserException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }

}
