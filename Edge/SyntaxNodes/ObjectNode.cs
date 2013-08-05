using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.SyntaxNodes
{

    // todo: using different constructors
    public class ObjectNode : INode
    {

        private Type typeInfo;
        private IEnumerable<PropertyNode> properties;

        public ObjectNode(Type typeInfo)
            : this(typeInfo, null)
        {

        }

        public ObjectNode(Type typeInfo, IEnumerable<PropertyNode> properties)
        {
            this.typeInfo = typeInfo;
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

        public IEnumerable<PropertyNode> Properties
        {
            get
            {
                return properties;
            }
        }

    }

}
