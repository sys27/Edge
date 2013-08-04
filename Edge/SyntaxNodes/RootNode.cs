using System;
using System.Collections.Generic;

namespace Edge.SyntaxNodes
{

    public class RootNode : INode
    {

        private ObjectNode rootObject;
        private IEnumerable<NamespaceNode> namespaces;

        public RootNode(ObjectNode rootObject)
            : this(rootObject, null)
        {

        }

        public RootNode(ObjectNode rootObject, IEnumerable<NamespaceNode> namespaces)
        {
            this.rootObject = rootObject;
            this.namespaces = namespaces;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var root = obj as RootNode;
            if (root == null)
                return false;
            
            return rootObject.Equals(root.rootObject) && namespaces.Equals(root.namespaces); // todo: !!!
        }

        public ObjectNode Root
        {
            get
            {
                return rootObject;
            }
        }

        public IEnumerable<NamespaceNode> Namespaces
        {
            get
            {
                return namespaces;
            }
        }

    }

}
