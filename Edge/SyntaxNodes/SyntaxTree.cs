// Copyright 2013 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
using Edge.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.SyntaxNodes
{

    public class SyntaxTree
    {

        private IEnumerable<NamespaceNode> namespaces;
        private RootObjectNode root;

        private IDictionary<string, ObjectNode> ids;

        public SyntaxTree(RootObjectNode root)
            : this(root, null, null)
        {
        }

        public SyntaxTree(RootObjectNode root, IEnumerable<NamespaceNode> namespaces)
            : this(root, namespaces, null)
        {
        }

        public SyntaxTree(RootObjectNode root, IDictionary<string, ObjectNode> ids)
            : this(root, null, ids)
        {
        }

        public SyntaxTree(RootObjectNode root, IEnumerable<NamespaceNode> namespaces, IDictionary<string, ObjectNode> ids)
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

        public RootObjectNode Root
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
