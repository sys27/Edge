using System;
using System.Reflection;

namespace Edge.SyntaxNodes
{
    
    public class PropertyNode<T> : INode
    {

        private PropertyInfo propertyInfo;
        private T value;

        public PropertyNode(PropertyInfo propertyInfo, T value)
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

        public T Value
        {
            get
            {
                return value;
            }
        }

    }

}
