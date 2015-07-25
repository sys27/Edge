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

        public string Build(SyntaxTree tree, string @class, string @namespace)
        {
            var result = string.Empty;

            if (tree.Namespaces != null)
                result = CreateNamespaces(tree.Namespaces) + nl;

            result += CreateRootObject(tree, @class, @namespace);

            return result;
        }

        private string CreateNamespaces(IEnumerable<string> namespaces)
        {
            var sb = new StringBuilder();

            foreach (var ns in namespaces)
                sb.AppendFormat("using {0};", ns).Append(nl);

            return sb.ToString();
        }

        private string CreateRootObject(SyntaxTree tree, string @class, string @namespace)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("namespace {0}", @namespace).Append(nl)
              .Append('{').Append(nl).Append(nl)
              .AppendFormat("public partial class {0} : {1}", @class, tree.RootObject.Type).Append(nl)
              .Append('{').Append(nl).Append(nl);

            if (tree != null)
                sb.Append(CreateMembers(tree.Objects)).Append(nl);

            sb.Append(CreateInitMethod(tree.Objects));

            sb.Append('}').Append(nl).Append(nl)
              .Append('}');

            return sb.ToString();
        }

        private string CreateMembers(IEnumerable<ObjectNode> ids)
        {
            var sb = new StringBuilder();

            foreach (var obj in ids)
                if (obj.Id != "this")
                    sb.AppendFormat("internal {0} {1};", obj.Type, obj.Id).Append(nl);

            return sb.ToString();
        }

        private string CreateInitMethod(IEnumerable<ObjectNode> objects)
        {
            var sb = new StringBuilder();

            sb.Append("public void InitializeComponent()").Append(nl)
              .Append('{').Append(nl);

            if (objects != null)
                sb.Append(InitMembers(objects)).Append(nl)
                  .Append(CreateProperties(objects));

            sb.Append('}').Append(nl).Append(nl);

            return sb.ToString();
        }

        private string InitMembers(IEnumerable<ObjectNode> ids)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var obj in ids)
                if (obj.Id != "this")
                    sb.AppendFormat("{0} = new {1}();", obj.Id, obj.Type).Append(nl);

            return sb.ToString();
        }

        private string CreateProperties(IEnumerable<ObjectNode> objects)
        {
            var sb = new StringBuilder();
            var thisObj = objects.First(obj => obj.Id == "this");

            foreach (var obj in objects)
                if (obj.Properties != null && obj.Id != "this")
                    foreach (var prop in obj.Properties)
                        sb.AppendFormat("{0}.{1} = {2};", obj.Id, prop.Property, CreateValue(prop.Value)).Append(nl);

            sb.Append(nl);
            foreach (var prop in thisObj.Properties)
                sb.AppendFormat("{0}.{1} = {2};", thisObj.Id, prop.Property, CreateValue(prop.Value)).Append(nl);

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
            if (value is CollectionNode)
                return CreateCollection((CollectionNode)value);
            if (value is ArrayNode)
                return CreateArray((ArrayNode)value);
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
            return e.Type + '.' + e.Value;
        }

        private string CreateBinding(BindingNode binding)
        {
            var sb = new StringBuilder();

            sb.Append("new Binding()").Append(nl)
              .Append('{').Append(nl);

            if (!string.IsNullOrWhiteSpace(binding.ElementName))
                sb.Append("ElementName = ").Append(binding.ElementName).Append(',').Append(nl);

            sb.Append("Path = ").Append(binding.Path);

            if (binding.Mode != BindingMode.Default)
            {
                sb.Append(',').Append(nl)
                  .Append("Mode = ").Append(binding.Mode);
            }

            sb.Append(nl).Append('}');

            return sb.ToString();
        }

        private string CreateArray(ArrayNode array)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("new {0}[]", array.ElementType).Append(nl)
              .Append('{').Append(nl);

            var i = 0;
            for (; i < array.Array.Length - 1; i++)
                sb.Append(CreateValue(array.Array[i])).Append(',').Append(nl);
            sb.Append(CreateValue(array.Array[i])).Append(nl);

            sb.Append('}');

            return sb.ToString();
        }

        private string CreateCollection(CollectionNode collection)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("new {0}", collection.CollectionType).Append(nl)
              .Append('{').Append(nl);

            var i = 0;
            for (; i < collection.Array.Length - 1; i++)
                sb.Append(CreateValue(collection.Array[i])).Append(',').Append(nl);
            sb.Append(CreateValue(collection.Array[i])).Append(nl);

            sb.Append('}');

            return sb.ToString();
        }

    }

}
