using System;

namespace Edge.SyntaxNodes
{

    // todo: array or collection
    public class ArrayNode<T> : INode
    {

        private Type arrayType;
        private T[] array;

        public ArrayNode(Type arrayType, T[] array)
        {
            this.arrayType = arrayType;
            this.array = array;
        }

        public Type ArrayType
        {
            get
            {
                return arrayType;
            }
        }

        public T[] Array
        {
            get
            {
                return array;
            }
        }

    }

}
