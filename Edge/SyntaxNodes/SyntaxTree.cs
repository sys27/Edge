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

        private IEnumerable<string> namespaces;
        private IEnumerable<ObjectNode> objects;

        public SyntaxTree(IEnumerable<ObjectNode> objects)
            : this(null, objects)
        {
        }

        public SyntaxTree(IEnumerable<string> namespaces, IEnumerable<ObjectNode> objects)
        {
            this.namespaces = namespaces;
            this.objects = objects;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var tree = obj as SyntaxTree;
            if (tree == null)
                return false;

            return ((namespaces == null && tree.namespaces == null) ||
                    (namespaces != null && tree.namespaces != null && namespaces.SequenceEqual(tree.namespaces))) &&
                   ((objects == null && tree.objects == null) ||
                    (objects != null && tree.objects != null && objects.SequenceEqual(tree.objects)));
        }

        public string Build(IBuilder builder)
        {
            return builder.Build(this);
        }

        public IEnumerable<string> Namespaces
        {
            get
            {
                return namespaces;
            }
        }

        public IEnumerable<ObjectNode> Objects
        {
            get
            {
                return objects;
            }
        }

    }

}
