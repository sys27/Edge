using System;

namespace Edge.Tokens
{

    public class UsingToken : IToken
    {

        public UsingToken()
        {

        }

        public override bool Equals(object obj)
        {
            var token = obj as UsingToken;
            if (token == null)
                return false;

            return true;
        }

        public override string ToString()
        {
            return "Using";
        }

    }

}
