using System;

namespace Edge.SyntaxNodes
{

    public class EnumNode : INode
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
            return base.Equals(obj);
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
