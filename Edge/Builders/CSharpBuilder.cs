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
using Edge.SyntaxNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edge.Builders
{

    public class CSharpBuilder : IBuilder
    {

        private readonly string nl;

        public CSharpBuilder()
            : this(Environment.NewLine)
        {
        }

        public CSharpBuilder(string nl)
        {
            this.nl = nl;
        }

        public string CreateRoot(RootNode root)
        {
            string result = string.Empty;

            if (root.Namespaces != null)
                result += CreateNamespaces(root.Namespaces);

            throw new NotImplementedException();
        }

        public string CreateNamespaces(IEnumerable<NamespaceNode> namespaces)
        {
            if (namespaces == null)
                throw new ArgumentNullException("namespaces");

            StringBuilder sb = new StringBuilder();

            foreach (var ns in namespaces)
                sb.Append(CreateNamespace(ns)).Append(nl);

            return sb.ToString();
        }

        public string CreateNamespace(NamespaceNode ns)
        {
            return "using" + ns.Namespace + ';';
        }

        public string CreateObject(ObjectNode obj)
        {
            string result = string.Empty;



            throw new NotImplementedException();
        }

    }

}