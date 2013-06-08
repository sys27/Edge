using System;

namespace Edge.Tokens
{

    public class TypeToken : IToken
    {

        private string type;

        public TypeToken(string type)
        {
            this.type = type;
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

    }

}
