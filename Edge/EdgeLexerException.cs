using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Edge
{

    [Serializable]
    public class EdgeLexerException : Exception
    {

        public EdgeLexerException() { }

        public EdgeLexerException(string message) : base(message) { }

        public EdgeLexerException(string message, Exception inner) : base(message, inner) { }

        protected EdgeLexerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

}
