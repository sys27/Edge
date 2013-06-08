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

        public string Str
        {
            get
            {
                return str;
            }
        }

    }

}
