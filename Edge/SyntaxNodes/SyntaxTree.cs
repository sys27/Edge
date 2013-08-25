using Edge.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.SyntaxNodes
{

    public class SyntaxTree
    {

        private IEnumerable<NamespaceNode> namespaces;
        private RootNode root;

        private IDictionary<string, ObjectNode> ids;

        public SyntaxTree(RootNode root)
            : this(root, null, null)
        {
        }

        public SyntaxTree(RootNode root, IEnumerable<NamespaceNode> namespaces)
            : this(root, namespaces, null)
        {
        }

        public SyntaxTree(RootNode root, IDictionary<string, ObjectNode> ids)
            : this(root, null, ids)
        {
        }

        public SyntaxTree(RootNode root, IEnumerable<NamespaceNode> namespaces, IDictionary<string, ObjectNode> ids)
        {
            this.root = root;
            this.namespaces = namespaces;
            this.ids = ids;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var tree = obj as SyntaxTree;
            if (tree == null)
                return false;

            return root.Equals(tree.root) &&
                   ((namespaces == null && tree.namespaces == null) ||
                    (namespaces != null && tree.namespaces != null && namespaces.SequenceEqual(tree.namespaces))) &&
                   ((ids == null && tree.ids == null) ||
                    (ids != null && tree.ids != null && ids.SequenceEqual(tree.ids)));
        }

        public string Build(IBuilder builder)
        {
            return builder.Build(this);
        }

        public IEnumerable<NamespaceNode> Namespaces
        {
            get
            {
                return namespaces;
            }
        }

        public RootNode Root
        {
            get
            {
                return root;
            }
        }

        public IDictionary<string, ObjectNode> IDs
        {
            get
            {
                return ids;
            }
        }

    }

}
