using System;

namespace Edge.SyntaxNodes
{

    public class EnumNode : IValueNode
    {

        private Type type;
        private object value;

        public EnumNode(Type type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var e = obj as EnumNode;
            if (e == null)
                return false;

            return type.Equals(e.type) && value.Equals(e.value);
        }

        public Type Info
        {
            get
            {
                return type;
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
