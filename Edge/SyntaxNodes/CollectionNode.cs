using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.SyntaxNodes
{

    public class CollectionNode : ArrayNode
    {

        private Type collectionType;

        public CollectionNode(Type collectionType, Type elementType, IValueNode[] array)
            : base(elementType, array)
        {
            this.collectionType = collectionType;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var col = obj as CollectionNode;
            if (col == null)
                return false;

            return collectionType.Equals(col.collectionType) &&
                   ElementType.Equals(col.ElementType) &&
                   Array.SequenceEqual(col.Array);
        }

        public Type CollectionType
        {
            get
            {
                return collectionType;
            }
        }

    }

}
