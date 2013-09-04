using System;
using System.Runtime.Serialization;

namespace Edge
{

    [Serializable]
    public class EdgeAnalyzerException : Exception
    {

        public EdgeAnalyzerException() { }

        public EdgeAnalyzerException(string message) : base(message) { }

        public EdgeAnalyzerException(string message, Exception inner) : base(message, inner) { }

        protected EdgeAnalyzerException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }

}
