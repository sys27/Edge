using System;
using System.Linq;

namespace Edge.SyntaxNodes
{

    // todo: array or collection
    public class ArrayNode : INode
    {

        private Type arrayType;
        private object[] array;

        public ArrayNode(Type arrayType, object[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

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

            return arrayType.Equals(arr.arrayType) && array.SequenceEqual(arr.array);
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
