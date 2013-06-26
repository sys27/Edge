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

        public override bool Equals(object obj)
        {
            var token = obj as TypeToken;
            if (token != null && token.type == this.type)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Type: " + type;
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
