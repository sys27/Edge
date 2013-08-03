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
