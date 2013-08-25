﻿// Copyright 2013 Dmitry Kischenko
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
