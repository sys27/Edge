// Copyright 2013 - 2015 Dmitry Kischenko
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.SyntaxNodes
{

    public class ObjectNode : INode
    {

        private string type;
        private string id;
        private IEnumerable<IValueNode> ctorArgs;
        private IEnumerable<PropertyNode> properties;

        public ObjectNode(string type, string id)
            : this(type, id, null, null)
        {
        }

        public ObjectNode(string type, string id, IEnumerable<IValueNode> ctorArgs)
            : this(type, id, ctorArgs, null)
        {
        }

        public ObjectNode(string type, string id, IEnumerable<PropertyNode> properties)
            : this(type, id, null, properties)
        {
        }

        public ObjectNode(string type, string id, IEnumerable<IValueNode> ctorArgs, IEnumerable<PropertyNode> properties)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            this.type = type;
            this.id = id;
            this.ctorArgs = ctorArgs;
            this.properties = properties;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(ObjectNode) != obj.GetType())
                return false;

            var o = obj as ObjectNode;

            return type.Equals(o.type) &&
                   id == o.id &&
                   ((ctorArgs == null && o.ctorArgs == null) ||
                    (ctorArgs != null && o.ctorArgs != null && ctorArgs.SequenceEqual(o.ctorArgs))) &&
                   ((properties == null && o.properties == null) ||
                    (properties != null && o.properties != null && properties.SequenceEqual(o.properties)));
        }

        public string Type
        {
            get
            {
                return type;
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
