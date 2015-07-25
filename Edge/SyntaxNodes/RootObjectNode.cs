// Copyright 2013 - 2015 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use base file except in compliance with the License.
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

    public class RootObjectNode : ObjectNode
    {

        public RootObjectNode(string typeInfo)
            : base(typeInfo, "this", null, null)
        {
        }

        public RootObjectNode(string typeInfo, IEnumerable<IValueNode> ctorArgs)
            : base(typeInfo, "this", ctorArgs, null)
        {
        }

        public RootObjectNode(string typeInfo, IEnumerable<PropertyNode> properties)
            : base(typeInfo, "this", null, properties)
        {
        }

        public RootObjectNode(string typeInfo, IEnumerable<IValueNode> ctorArgs, IEnumerable<PropertyNode> properties)
            : base(typeInfo, "this", ctorArgs, properties)
        {
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(RootObjectNode) != obj.GetType())
                return false;

            var o = obj as RootObjectNode;

            return Type.Equals(o.Type) &&
                   Id == o.Id &&
                   ((ConstructorArguments == null && o.ConstructorArguments == null) ||
                    (ConstructorArguments != null && o.ConstructorArguments != null && ConstructorArguments.SequenceEqual(o.ConstructorArguments))) &&
                   ((Properties == null && o.Properties == null) ||
                    (Properties != null && o.Properties != null && Properties.SequenceEqual(o.Properties)));
        }
        
    }

}
