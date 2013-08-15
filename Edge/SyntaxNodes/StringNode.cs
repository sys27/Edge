using System;

namespace Edge.SyntaxNodes
{
    
    public class StringNode : INode
    {

        private string str;

        public StringNode(string str)
        {
            this.str = str;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var sn = obj as StringNode;
            if (sn == null)
                return false;

            return str == sn.str;
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
