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

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var namespaceNode = obj as NamespaceNode;
            if (namespaceNode == null)
                return false;

            return ns == namespaceNode.ns;
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
