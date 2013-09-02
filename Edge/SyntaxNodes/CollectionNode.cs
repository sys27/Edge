using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edge.SyntaxNodes
{

    public class CollectionNode : ArrayNode
    {

        private string collectionType;

        public CollectionNode(string collectionType, string elementType, IValueNode[] array)
            : base(elementType, array)
        {
            this.collectionType = collectionType;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (typeof(CollectionNode) != obj.GetType())
                return false;

            var col = obj as CollectionNode;

            return collectionType.Equals(col.collectionType) &&
                   ElementType.Equals(col.ElementType) &&
                   Array.SequenceEqual(col.Array);
        }

        public string CollectionType
        {
            get
            {
                return collectionType;
            }
        }

    }

}
