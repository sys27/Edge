using System;

namespace Edge.Tokens
{
    
    public class StringToken : IToken
    {

        private string str;

        public StringToken(string str)
        {
            this.str = str;
        }

        public override bool Equals(object obj)
        {
            var token = obj as StringToken;
            if (token != null && token.str == this.str)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "String: " + str;
        }

        public string Str
        {
            get
            {
                return str;
            }
        }

    }

}
