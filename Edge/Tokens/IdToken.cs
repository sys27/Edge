using System;

namespace Edge.Tokens
{
    
    public class IdToken : IToken
    {

        private string id;

        public IdToken(string id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            var token = obj as IdToken;
            if (token != null || token.id == this.id)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Id: " + id;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

    }

}
