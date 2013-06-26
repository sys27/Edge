using System;

namespace Edge.Tokens
{

    public class NamespaceToken : IToken
    {

        private string ns;

        public NamespaceToken(string ns)
        {
            this.ns = ns;
        }

        public override bool Equals(object obj)
        {
            var token = obj as NamespaceToken;
            if (token != null && token.ns == this.ns)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "Namespace: " + ns;
        }

        public string Namespace
        {
            get
            {
                return ns;
            }
        }

    }

}
