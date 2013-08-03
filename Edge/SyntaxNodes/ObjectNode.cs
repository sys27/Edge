using System;

namespace Edge.SyntaxNodes
{
    
    // todo: using different constructors
    public class ObjectNode : INode
    {

        private Type typeInfo;

        public ObjectNode(Type typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public Type Info
        {
            get
            {
                return typeInfo;
            }
        }

    }

}
