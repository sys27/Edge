using System;

namespace Edge.SyntaxNodes
{

    // todo: array or collection
    public class ArrayNode : INode
    {

        private Type arrayType;
        private object[] array;

        public ArrayNode(Type arrayType, object[] array)
        {
            this.arrayType = arrayType;
            this.array = array;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var arr = obj as ArrayNode;
            if (arr == null)
                return false;

            return arrayType.Equals(arr.arrayType) && array.Equals(arr.array); // todo: !!!
        }

        public Type ArrayType
        {
            get
            {
                return arrayType;
            }
        }

        public object[] Array
        {
            get
            {
                return array;
            }
        }

    }

}
