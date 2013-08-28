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
using System.Linq;
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

            result += CreateRootObject(tree.Objects);

            return result;
        }

        private string CreateNamespaces(IEnumerable<string> namespaces)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var ns in namespaces)
                sb.Append("using ").Append(ns).Append(';').Append(nl);

            return sb.ToString();
        }

        private string CreateRootObject(IEnumerable<ObjectNode> objects)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("namespace ").Append(nl)
              .Append('{').Append(nl)
              .Append("public partial class ").Append(nl)
              .Append('{').Append(nl);

            if (objects != null)
                sb.Append(CreateMembers(objects)).Append(nl);

            sb.Append(CreateInitMethod(objects));

            sb.Append('}').Append(nl)
              .Append('}');

            return sb.ToString();
        }

        private string CreateMembers(IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var obj in ids)
                if (obj.Id != "this")
                    sb.Append("internal ").Append(obj.Info.Name).Append(' ').Append(obj.Id).Append(';').Append(nl);

            return sb.ToString();
        }

        private string CreateInitMethod(IEnumerable<ObjectNode> objects)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("public void InitializeComponent()").Append(nl)
              .Append('{').Append(nl);

            if (objects != null)
                sb.Append(InitMembers(objects)).Append(nl).Append(CreateProperties(objects)).Append(nl);

            return sb.ToString();
        }

        private string InitMembers(IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var obj in ids)
                if (obj.Id != "this")
                    sb.Append(obj.Id).Append(" = new ").Append(obj.Info.Name).Append("();").Append(nl);

            return sb.ToString();
        }

        private string CreateProperties(IEnumerable<ObjectNode> objects)
        {
            StringBuilder sb = new StringBuilder();

            var objs = new List<ObjectNode>(objects);
            var thisObj = (from obj in objs
                           where obj.Id == "this"
                           select obj).First();
            objs.Remove(thisObj);
            objs.Add(thisObj);

            foreach (var obj in objs)
                if (obj.Properties != null)
                    foreach (var prop in obj.Properties)
                        sb.Append(obj.Id).Append('.').Append(prop.Info.Name).Append(" = ").Append(CreateValue(prop.Value)).Append(';').Append(nl);

            return sb.ToString();
        }

        private string CreateValue(IValueNode value)
        {
            if (value is ReferenceNode)
                return CreateReference((ReferenceNode)value);
            if (value is NumberNode)
                return CreateNumber((NumberNode)value);
            if (value is StringNode)
                return CreateString((StringNode)value);
            if (value is EnumNode)
                return CreateEnum((EnumNode)value);
            if (value is BindingNode)
                return CreateBinding((BindingNode)value);
            else
                // todo: ...!
                throw new Exception();
        }

        private string CreateReference(ReferenceNode reference)
        {
            return reference.Id;
        }

        private string CreateNumber(NumberNode number)
        {
            return number.Number.ToString();
        }

        private string CreateString(StringNode str)
        {
            return "\"" + str.Str + "\"";
        }

        private string CreateEnum(EnumNode e)
        {
            return e.Info.Name + '.' + e.Value;
        }

        private string CreateBinding(BindingNode binding)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("new Binding()").Append(nl)
              .Append('{').Append(nl);

            if (!string.IsNullOrWhiteSpace(binding.ElementName))
                sb.Append("ElementName = ").Append(binding.ElementName).Append(',').Append(nl);

            sb.Append("Path = ").Append(binding.Path).Append(nl)
              .Append('}').Append(nl);

            return sb.ToString();
        }

    }

}
