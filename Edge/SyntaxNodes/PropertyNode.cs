using System;
using System.Reflection;

namespace Edge.SyntaxNodes
{
    
    public class PropertyNode : INode
    {

        private PropertyInfo propertyInfo;
        private object value;

        public PropertyNode(PropertyInfo propertyInfo, object value)
        {
            this.propertyInfo = propertyInfo;
            this.value = value;
        }

        public PropertyInfo Info
        {
            get
            {
                return propertyInfo;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }

    }

}
