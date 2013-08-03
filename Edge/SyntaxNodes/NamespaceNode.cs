using System;

namespace Edge.SyntaxNodes
{
    
    public class NamespaceNode : INode
    {

        private string ns;

        public NamespaceNode(string ns)
        {
            this.ns = ns;
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
