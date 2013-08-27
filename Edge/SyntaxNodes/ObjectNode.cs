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

    public class ObjectNode : INode
    {

        private Type typeInfo;
        private string id;
        private IEnumerable<IValueNode> ctorArgs;
        private IEnumerable<PropertyNode> properties;

        public ObjectNode(Type typeInfo, string id)
            : this(typeInfo, id, null, null)
        {
        }

        public ObjectNode(Type typeInfo, string id, IEnumerable<IValueNode> ctorArgs)
            : this(typeInfo, id, ctorArgs, null)
        {
        }

        public ObjectNode(Type typeInfo, string id, IEnumerable<PropertyNode> properties)
            : this(typeInfo, id, null, properties)
        {
        }

        public ObjectNode(Type typeInfo, string id, IEnumerable<IValueNode> ctorArgs, IEnumerable<PropertyNode> properties)
        {
            if (typeInfo == null)
                throw new ArgumentNullException("typeInfo");
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            this.typeInfo = typeInfo;
            this.id = id;
            this.ctorArgs = ctorArgs;
            this.properties = properties;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var o = obj as ObjectNode;
            if (o == null)
                return false;

            return typeInfo.Equals(o.typeInfo) &&
                   id == o.id &&
                   ((ctorArgs == null && o.ctorArgs == null) ||
                    (ctorArgs != null && o.ctorArgs != null && ctorArgs.SequenceEqual(o.ctorArgs))) &&
                   ((properties == null && o.properties == null) ||
                    (properties != null && o.properties != null && properties.SequenceEqual(o.properties)));
        }

        public Type Info
        {
            get
            {
                return typeInfo;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public IEnumerable<IValueNode> ConstructorArguments
        {
            get
            {
                return ctorArgs;
            }
        }

        public IEnumerable<PropertyNode> Properties
        {
            get
            {
                return properties;
            }
        }

    }

}
