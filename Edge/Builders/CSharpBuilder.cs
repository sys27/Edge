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
using Edge.SyntaxNodes;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string Build(SyntaxTree tree)
        {
            string result = string.Empty;

            if (tree.Namespaces != null)
                result = CreateNamespaces(tree.Namespaces) + nl;

            result += CreateRootObject(tree.Root, tree.IDs);

            return result;
        }

        private string CreateNamespaces(IEnumerable<NamespaceNode> namespaces)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var ns in namespaces)
                sb.Append("using ").Append(ns.Namespace).Append(';').Append(nl);

            return sb.ToString();
        }

        private string CreateRootObject(RootObjectNode root, IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("namespace ").Append(nl)
              .Append('{').Append(nl)
              .Append("public partial class ").Append(nl)
              .Append('{').Append(nl);

            if (ids != null)
                sb.Append(CreateMembers(ids)).Append(nl);

            sb.Append(CreateInitMethod(root, ids));

            sb.Append('}').Append(nl)
              .Append('}');

            return sb.ToString();
        }

        private string CreateMembers(IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var obj in ids)
                sb.Append("internal ").Append(obj.Info.Name).Append(' ').Append(obj.Id).Append(';').Append(nl);

            return sb.ToString();
        }

        private string CreateInitMethod(RootObjectNode root, IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("public void InitializeComponent()").Append(nl)
              .Append('{').Append(nl);

            if (ids != null)
                sb.Append(InitMembers(ids)).Append(nl);

            return sb.ToString();
        }

        private string InitMembers(IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var obj in ids)
                sb.Append(obj.Id).Append(" = new ").Append(obj.Info.Name).Append("();").Append(nl);

            return sb.ToString();
        }

    }

}
